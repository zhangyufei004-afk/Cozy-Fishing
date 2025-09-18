using FishingGame.Reeling;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FishingGame
{
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

        private bool _isClockWise;
        private bool _isMoving;
        private float _angleTotal;
        private float _directionChangeTracker;
        private Vector2 _lastPosition;
        private float _currentSpeed;

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
                    _isClockWise = _angleTotal < 0;
                    DirectionChangeLogic(_currentSpeed);
                }
                _lastPosition = localMousePos;
                
            }
            else { _isMoving = false; }
        }

        public bool GetCurrentDirection()
        {
            return _isClockWise;
        }

        public bool GetIsMovingValue()
        {
            return _isMoving;
        }

        public float GetSpeed()
        {
            return _currentSpeed;
        }

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
            if (_isClockWise)
            {
                if (_directionChangeTracker > 0)
                {
                    _angleTotal = 0;
                    return;
                }
            }
            else
            {
                if (_directionChangeTracker < 0)
                {
                    _angleTotal = 0;
                    return;
                }
            }
        }

        private void ClampAngleTotal()
        {
            _directionChangeTracker = Mathf.Clamp(_directionChangeTracker, lowestAngleTotalValue, highestAngleTotalValue);
        }

        private void SetPositionToMouse()
        {
            Vector2 newMousePosition = _realisticStickAction.ReadValue<Vector2>();
            transform.position = newMousePosition;
        }

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
        /// Returns true if raycast from mouse touches a ui element taged as "RealisticGoal"
        /// </summary>
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
        private static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = UnityEngine.Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            return raycastResults;
        }
    }
}
