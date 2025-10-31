using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace FishingGame.Camera
{
    public class CheckForObstacle : MonoBehaviour
    {
        [Header("Cinemachine Component Properties")]
        [SerializeField] private CinemachineThirdPersonFollow thirdPersonFollow;

        [Header("Fishing References")] 
        [SerializeField] private GameObject hook;
        
        private UnityEngine.Camera _mainCamera;
        private CinemachineBrain _brain;
        private void OnEnable()
        {
            _mainCamera = UnityEngine.Camera.main;
            _brain = _mainCamera?.GetComponent<CinemachineBrain>();
            StartCoroutine(TestForCollision());
        }

        private void OnDisable()
        {
            StopCoroutine(TestForCollision());
        }

        IEnumerator TestForCollision()
        {
            yield return new WaitUntil(() => !_brain.IsBlending);
            if (_brain.ActiveVirtualCamera.Name == this.name && IsObjectInFrustum(hook))
            {
                if (!Physics.Raycast(hook.transform.position,
                        this.transform.position - hook.transform.position, out RaycastHit hit))
                {
                    Debug.LogError(hit.collider.gameObject.name);
                    if (hit.collider.gameObject != this.gameObject)
                    {   // We have a collision
                        thirdPersonFollow.CameraSide = (thirdPersonFollow.CameraSide + 1) % 2;
                    }
                }
            }
        }
        
        private bool IsObjectInFrustum(GameObject inGameObject)
        {
            Vector3 screenPoint = _mainCamera.WorldToScreenPoint(inGameObject.transform.position);
            return screenPoint.x >= 0 && screenPoint.x <= Screen.width && screenPoint.y >= 0 && screenPoint.y <= Screen.height;
        }
    }
}
