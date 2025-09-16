using System.Collections.Generic;
using FishingGame.GameTime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace FishingGame.AI.NPC.Nodes
{
    /// <summary>
    /// Tree Node which checks if the current destination is correct, and sets it if it isn't.
    /// </summary>
    public class CheckDestination : TreeNode
    {
        private readonly Transform _npcTransform;
        private Vector3 _currentDestinationWorld;
        private readonly InGameTime _gameTime;
        private readonly float _workTime;
        private readonly float _hobbyTime;
        private readonly float _homeTime;
        private readonly Vector3 _homeLocation;
        private readonly Vector3 _workplaceLocation;
        private readonly Vector3 _hobbyLocation;

        public CheckDestination(
            Transform npcTransform, 
            Vector3 workplaceLocation,
            Vector3 homeLocation,
            Vector3 hobbyLocation,
            InGameTime gameTime,
            float workTimeRatio = 0.292f,
            float hobbyTimeRatio = 0.708f,
            float homeTimeRatio = 0.792f)
        {
            _npcTransform = npcTransform;
            _gameTime = gameTime;

            _workTime = workTimeRatio;
            _hobbyTime = hobbyTimeRatio;
            _homeTime = homeTimeRatio;
            
            _homeLocation = homeLocation;
            _workplaceLocation = workplaceLocation;
            _hobbyLocation = hobbyLocation;
        }

        public override void Initialize()
        {
            SetData("Work", _workplaceLocation);
            SetData("Home", _homeLocation);
            SetData("Hobby", _hobbyLocation);
        }

        public override ETreeNodeState RunNode()
        {
            State = ETreeNodeState.Failure;
            float dayLength = _gameTime.DayLength;
            float currentTime = _gameTime.CurrentTimeOfDay;
            
            float workTime = _workTime * dayLength;
            float hobbyTime = _hobbyTime * dayLength;
            float homeTime = _homeTime * dayLength;
            
            // Check for moving to Work
            Vector3 workLocation = (Vector3)GetData("Work");
            Vector3 hobbyLocation = (Vector3)GetData("Hobby");
            Vector3 homeLocation = (Vector3)GetData("Home");

            if (!IsAtCorrectPosition(_npcTransform.position, workLocation, currentTime, 
                    workTime, hobbyTime) 
                || !IsAtCorrectPosition(_npcTransform.position, hobbyLocation, currentTime, 
                    hobbyTime, homeTime) 
                || !IsAtCorrectPosition(_npcTransform.position, homeLocation, currentTime, 
                    homeTime, dayLength))
            {   // If we arent at any of the correct positions, return Success
                State = ETreeNodeState.Success;
            }
            return State;
        }

        private bool IsAtCorrectPosition(Vector3 currentPosition, Vector3 positionToCheckAgainst, float currentTime,
            float minTimeToMove, float maxTimeToMove)
        {
            if (currentTime < maxTimeToMove && currentTime > minTimeToMove &&
                Vector3.Distance(currentPosition, positionToCheckAgainst) < 0.1f && 
                _currentDestinationWorld == positionToCheckAgainst)
            {   // If we are at the position and the times are correct for this position
                return true;
            }
            SetData("Destination", positionToCheckAgainst);
            _currentDestinationWorld = positionToCheckAgainst;
            return false;
        }
    }
}