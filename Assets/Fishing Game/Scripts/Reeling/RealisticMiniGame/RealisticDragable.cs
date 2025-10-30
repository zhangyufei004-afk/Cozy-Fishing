using FishingGame.Fishing_Game.Scripts.GameManagement;
using FishingGame.GameManagement;
using FishingGame.Reeling;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This script controls the logic and behaviour behind the dragable component of the realistic minigame
    /// It also contains a reference to the minigame master
    /// </summary>
    public class RealisticDragable : MonoBehaviour
    {
        [Header("Script refrences")]

        [SerializeField]
        [Tooltip("The master script for this minigame")]
        private RealisticMiniGameMaster minigameMaster;

        [SerializeField]
        [Tooltip("The game manager")]
        private GameManager gameManager;

        [SerializeField]
        [Tooltip("The speed that scales how fast the image rotates")]
        private float rotationSpeed;

        [SerializeField]
        [Tooltip("This variable scales down the controller speed variable to try and emulate the same as the mouse input")]
        private float controllerSpeedDescalar;

        private UnityEngine.UI.Image _centerImage;

        private InputAction _realisticStickAction;
        private InputAction _realisticMouseAction;

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

        private Vector2 _lastStickDirection;
        private float _currentAngle;

        private void OnEnable()
        {
            _centerImage = minigameMaster.GetCentreImage();


            InputActionAsset inputAsset = InputSystem.actions;
            InputActionMap uiActionMap = inputAsset.FindActionMap("RealisticMiniGame");
            uiActionMap.Enable();

            _realisticStickAction = uiActionMap.FindAction("RealisticController");
            _realisticMouseAction = uiActionMap.FindAction("RealisticMouse");
            
        }

        private void Update()
        {
            if (gameManager?.GetCurrentControlScheme() is Gamepad) { MovementIfController(); }
            else { MovementIfMouse(); } 
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
        /// Contains the movement logic for if a mouse is being used
        /// </summary>
        private void MovementIfMouse()
        {
            Debug.Log("Mousemove");

            Vector2 mousePosition = _realisticMouseAction.ReadValue<Vector2>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent as RectTransform,
                mousePosition,
                null,
                out Vector2 localMousePos
            );

            Vector2 center = ((RectTransform)_centerImage.transform).anchoredPosition;
            Vector2 previousDirection = (_lastPosition - center).normalized;
            Vector2 newDirection = (localMousePos - center).normalized;


            Vector2 direction = localMousePos - ((RectTransform)_centerImage.transform).anchoredPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (_lastPosition != Vector2.zero)
            {
                _currentSpeed = Vector2.SignedAngle(previousDirection, newDirection);
                _angleTotal += _currentSpeed;
                DetermineCurrentDirection();
                DirectionChangeLogic(_currentSpeed);
            }

            if (_lastPosition == localMousePos)
            {
                _currentDirection = ERealisticDirection.Stop;
            }

            _lastPosition = localMousePos;
            gameObject.GetComponent<UnityEngine.UI.Image>().rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        }

        /// <summary>
        /// Contains the movement logic for if a controller is being used
        /// </summary>
        private void MovementIfController()
        {
            Debug.Log("ControllerMove");
            Vector2 stickInput = _realisticStickAction.ReadValue<Vector2>();

            if (stickInput.magnitude < 0.2f)
                return;

            float stickAngle = Mathf.Atan2(stickInput.y, stickInput.x) * Mathf.Rad2Deg;

            if (_lastStickDirection != Vector2.zero)
            {
                _currentSpeed = Vector2.SignedAngle(_lastStickDirection, stickInput);
                _angleTotal += _currentSpeed;
                DetermineCurrentDirection();
                DirectionChangeLogic(_currentSpeed);
            }
            else { _currentDirection = ERealisticDirection.Stop; }

            gameObject.GetComponent<UnityEngine.UI.Image>().rectTransform.rotation = Quaternion.Euler(0, 0, stickAngle);
            _lastStickDirection = stickInput;
        }

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
        /// Sets the position of the mouse to the input action
        /// </summary>
        private void SetPositionToMouse()
        {
            Vector2 newMousePosition = _realisticStickAction.ReadValue<Vector2>();
            Vector2 closestPoint = minigameMaster.GetClosestPoint(newMousePosition);
            transform.position = closestPoint;
        }

        /// <summary>
        /// Clamps the direction change tracker, this is so the value dosen't build up to extremly high values,
        /// this allows the direction changing method to work
        /// </summary>
        private void ClampAngleTotal()
        {
            _directionChangeTracker = Mathf.Clamp(_directionChangeTracker, lowestAngleTotalValue, highestAngleTotalValue);
        }


        #endregion
    }
}
