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
        private InGameTime _gameTime;
        private float _workTime;
        private float _hobbyTime;
        private float _homeTime;

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
            
            SetData("Work", workplaceLocation);
            SetData("Home", homeLocation);
            SetData("Hobby", hobbyLocation);
        }

        public override ETreeNodeState RunNode()
        {
            State = ETreeNodeState.Failure;
            float dayLength = _gameTime.DayLength;
            float currentTime = _gameTime.CurrentTimeOfDay;
            
            // Check for moving to Work
            float workTime = _workTime * dayLength;
            Vector3 workLocation = (Vector3)GetData("Work");
            if (currentTime > workTime && Vector3.Distance(_npcTransform.position, workLocation) > 0.1f)
            {
                SetData("Destination", workLocation);
                return ETreeNodeState.Success;
            }
            
            // Check for moving to Hobby
            float hobbyTime = _hobbyTime * dayLength;
            Vector3 hobbyLocation = (Vector3)GetData("Hobby");
            if (currentTime > hobbyTime && Vector3.Distance(_npcTransform.position, hobbyLocation) > 0.1f)
            {
                SetData("Destination", hobbyLocation);
                return ETreeNodeState.Success;
            }
            
            // Check for moving to Home
            float homeTime = _homeTime * dayLength;
            Vector3 homeLocation = (Vector3)GetData("Home");
            if (currentTime > homeTime && Vector3.Distance(_npcTransform.position, homeLocation) > 0.1f)
            {
                SetData("Destination", homeLocation);
                return ETreeNodeState.Success;
            }
            
            
            if (Vector3.Distance(_npcTransform.position, _currentDestinationWorld) < 0.1f)
            {
                State = ETreeNodeState.Success;
            }
            return State;
        }
    }
}