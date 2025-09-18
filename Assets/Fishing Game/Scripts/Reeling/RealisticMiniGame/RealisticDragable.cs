using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FishingGame
{
    public class RealisticDragable : MonoBehaviour
    {
        private int _uILayer;

        private PlayerInput _playerInput;
        private InputAction _mouseInput;
        private InputAction _realisticStickAction;

        private float _angleTotal;
        private Vector2 _lastPosition;


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
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                _angleTotal += Vector2.SignedAngle(_lastPosition, mousePosition);

                SetPositionToMouse(mousePosition);

                bool isClockWise = _angleTotal > 0;
                Debug.Log(isClockWise);
            }
        }

        private void SetPositionToMouse(Vector2 newMousePosition)
        {
            _lastPosition = newMousePosition;
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
