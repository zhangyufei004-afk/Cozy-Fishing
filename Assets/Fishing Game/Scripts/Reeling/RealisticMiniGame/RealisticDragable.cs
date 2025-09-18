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
        private int _uILayer;

        private PlayerInput _playerInput;
        private InputAction _mouseInput;
        private InputAction _realisticStickAction;

        private bool _isClockWise;
        private float _angleTotal;
        private float _directionChangeTracker;
        private Vector2 _lastPosition;

        [SerializeField]
        private Image centerOfUI;

        [SerializeField]
        [Tooltip("The lowest possible value of the accumalted angle value")]
        private int lowestAngleTotalValue;

        [SerializeField]
        [Tooltip("The highest possible value of the accumulated angle value")]
        private int highestAngleTotalValue;


        private void OnEnable()
        {
            _uILayer = LayerMask.NameToLayer("UI");


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
                SetPositionToMouse();
                Vector2 mousePosition = _realisticStickAction.ReadValue<Vector2>();

                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    transform.parent as RectTransform,
                    mousePosition,
                    null,
                    out Vector2 localMousePos
                );

                
                Vector2 center = ((RectTransform)centerOfUI.transform).anchoredPosition;
                Vector2 previousDirection = (_lastPosition - center).normalized;
                Vector2 newDirection = (localMousePos - center).normalized;

                if (_lastPosition != Vector2.zero)
                {
                    float angleDelta = Vector2.SignedAngle(previousDirection, newDirection);
                    _angleTotal += angleDelta;
                    _isClockWise = _angleTotal < 0;
                    CheckForDirectionChange(angleDelta);
                }

                
                _lastPosition = localMousePos;
                
                Debug.Log($"Total Angle: {_angleTotal}, Clockwise: {_isClockWise}");
            }
        }

        private void CheckForDirectionChange(float valueToAddToTotal)
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
                if (IsPointerOverUIElement(GetEventSystemRaycastResults()))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if raycast from mouse touches a ui element taged as "RealisticGoal"
        /// </summary>
        private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == _uILayer && curRaysastResult.gameObject.CompareTag("RealisticGoal"))
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
