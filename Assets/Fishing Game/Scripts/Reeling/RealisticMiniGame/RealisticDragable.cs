using FishingGame.Reeling;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This script controls the logic and behaviour behind the dragable component of the realistic minigame
    /// It also contains a reference to the minigame master
    /// NOTE: THIS IS A WIP, it functions but there is a lot of work to still go into this minigame
    /// Several inefficient functions currently present to get this working in a low amount of time
    /// </summary>
    public class RealisticDragable : MonoBehaviour
    {
        [Header("Script refrences")]

        [SerializeField]
        [Tooltip("The master script for this minigame")]
        private RealisticMiniGameMaster minigameMaster;

        private Image _centerImage;

        private int _uILayer;
        private int _dragAbleLayer;
        private string _realisticTag;
        private string _dragAbleTag;

        private PlayerInput _playerInput;
        private InputAction _mouseInput;
        private InputAction _realisticStickAction;

        private ERealisticDirection _currentDirection;

        private bool _isMoving;
        private float _angleTotal;
        private float _directionChangeTracker;
        private Vector2 _lastPosition;
        private float _currentSpeed;
        [Header("Game Data")]

        [SerializeField]
        [Tooltip("The lowest possible value of the accumalted angle value")]
        private int lowestAngleTotalValue;

        [SerializeField]
        [Tooltip("The highest possible value of the accumulated angle value")]
        private int highestAngleTotalValue;


        private void OnEnable()
        {
            _uILayer = LayerMask.NameToLayer("UI");
            _dragAbleLayer = LayerMask.NameToLayer("Dragable");

            _dragAbleTag = "DragableUI";
            _realisticTag = "RealisticGoal";

            _centerImage = minigameMaster.GetCentreImage();


            InputActionAsset inputAsset = InputSystem.actions;
            InputActionMap uiActionMap = inputAsset.FindActionMap("UI");
            uiActionMap.Enable();
            _realisticStickAction = uiActionMap.FindAction("RealisticStickMovement");
            _mouseInput = uiActionMap.FindAction("Click");
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            if (CheckIfMouseIsHolding())
            {
                // TODO: This needs to be made much cleaner later on with it properly being a circle bounds instead of a scuffed square
                if (CheckIfMouseIsWithinCircle(_dragAbleLayer, _dragAbleTag) != true)
                {
                    _currentSpeed = 0;
                    _isMoving = false;
                    return;
                }
                _isMoving = true;
                SetPositionToMouse();
                Vector2 mousePosition = _realisticStickAction.ReadValue<Vector2>();

                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    transform.parent as RectTransform,
                    mousePosition,
                    null,
                    out Vector2 localMousePos
                );

                
                Vector2 center = ((RectTransform)_centerImage.transform).anchoredPosition;
                Vector2 previousDirection = (_lastPosition - center).normalized;
                Vector2 newDirection = (localMousePos - center).normalized;

                if (_lastPosition != Vector2.zero)
                {
                    _currentSpeed = Vector2.SignedAngle(previousDirection, newDirection);
                    _angleTotal += _currentSpeed;
                    DetermineCurrentDirection();
                    DirectionChangeLogic(_currentSpeed);
                }

                _lastPosition = localMousePos;
                
            }
            else 
            { 
                _isMoving = false;
                _currentDirection = ERealisticDirection.Stop; 
            }
        }

        #region Public Functions

        /// <summary>
        /// Returns the current direction enum as an int value
        /// </summary>
        /// <returns>Current direction enum as an int</returns>
        public int GetCurrentDirectionAsInt()
        {
            return (int)_currentDirection;
        }

        /// <summary>
        /// Returns a bool that says if the dragable is currently moving or not
        /// true = moving, false = not moving
        /// </summary>
        /// <returns>True if moving, false if not moving</returns>
        public bool GetIsMovingValue()
        {
            return _isMoving;
        }

        /// <summary>
        /// Returns a float value representing the current speed
        /// </summary>
        /// <returns>The current speed value of the dragable</returns>
        public float GetSpeed()
        {
            return _currentSpeed;
        }

        #endregion

        #region MovementLogic

        /// <summary>
        /// This function can be called to check if the direction the player is spinning in has just changed
        /// This allows the _angleTotal value to continuely build up and be used to track how fast the player is spinning
        /// Then if the player suddenly changed direction this function will kick in and reset the total amount of angle that had
        /// accumulated.
        /// </summary>
        /// <param name="valueToAddToTotal">The angle amount being added</param>
        private void DirectionChangeLogic(float valueToAddToTotal)
        {
            _directionChangeTracker += valueToAddToTotal;
            ClampAngleTotal();

            switch (_currentDirection)
            {
                case ERealisticDirection.Clockwise:
                    if (_directionChangeTracker > 0)
                    {
                        _angleTotal = 0;
                    }
                    break;
                case ERealisticDirection.AntiClockwise:
                    if (_directionChangeTracker < 0)
                    {
                        _angleTotal = 0;
                    }
                    break;
                case ERealisticDirection.Stop:
                    break;
                default:
                    if (_directionChangeTracker > 0)
                    {
                        _angleTotal = 0;
                    }
                    throw new InvalidOperationException("Waring: ECurrentDirection Enum was not set to an aproipreate value, has defaulted to clockwise! This happened to object: " + gameObject.name);
            }
        }

        /// <summary>
        /// Contains logic to figure out what direction this is currently moving in, then sets the enum value to that
        /// </summary>
        private void DetermineCurrentDirection()
        {
            if (_angleTotal < 0)
            {
                _currentDirection = ERealisticDirection.Clockwise;
            }
            else if (_angleTotal > 0)
            {
                _currentDirection = ERealisticDirection.AntiClockwise;
            }
            else
            {
                _currentDirection = ERealisticDirection.Stop;
            }
        }

        /// <summary>
        /// Checks if the mouse is currently within the reeling circle, returns true if so otherwise false
        /// Takes parameters for the layer to check and tag to check
        /// </summary>
        /// <param name="layerToCheck">The layer of the ui object being checked</param>
        /// <param name="tagToCheck">The tag of the ui object being checked</param>
        /// <returns>True if mouse is within circle otherwise false</returns>
        private bool CheckIfMouseIsWithinCircle(int layerToCheck, string tagToCheck)
        {
            if (IsPointerOverUIElement(GetEventSystemRaycastResults(), layerToCheck, tagToCheck))
            {
                return true;
            }
            else
            { return false; }
        }

        /// <summary>
        /// Sets the position of the mouse to the input action
        /// </summary>
        private void SetPositionToMouse()
        {
            Vector2 newMousePosition = _realisticStickAction.ReadValue<Vector2>();
            transform.position = newMousePosition;
        }

        /// <summary>
        /// Clamps the direction change tracker, this is so the value dosen't build up to extremly high values,
        /// this allows the direction changing method to work
        /// </summary>
        private void ClampAngleTotal()
        {
            _directionChangeTracker = Mathf.Clamp(_directionChangeTracker, lowestAngleTotalValue, highestAngleTotalValue);
        }

        /// <summary>
        /// Returns true if mouse is pressed and over the dragable object
        /// </summary>
        /// <returns>True if mouse is pressed and over the dragable object otherwise false</returns>
        private bool CheckIfMouseIsHolding()
        {
            if (_mouseInput.IsPressed())
            {
                if (IsPointerOverUIElement(GetEventSystemRaycastResults(), _uILayer, _realisticTag))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if raycast from mouse touches a ui element matching the layer and tag inputed
        /// </summary>
        /// <param name="eventSystemRaysastResults">A raycast result</param>
        /// <param name="layerToCheck">The layer that the UI object should have</param>
        /// <param name="tagToCheck">The tag that the UI object should have</param>
        /// <returns></returns>
        private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults, int layerToCheck, string tagToCheck)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == layerToCheck && curRaysastResult.gameObject.CompareTag(tagToCheck))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all event system raycast results of current mouse or touch position.
        /// </summary>
        /// <returns>The raycast results</returns>
        private static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = UnityEngine.Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            return raycastResults;
        }
        #endregion
    }
}
