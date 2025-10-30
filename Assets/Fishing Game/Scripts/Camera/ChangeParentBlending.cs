using FishingGame.GameManagement;
using Unity.Cinemachine;
using UnityEngine;

namespace FishingGame.Camera
{
    /// <summary>
    /// This class checks whether the Cinemachine camera is blending and then changes the parent to be null, when the camera
    /// is re-enabled, it re-parents itself to the original parent.
    /// </summary>
    [RequireComponent(typeof(CinemachineCamera))]
    public class ChangeParentBlending : MonoBehaviour
    {
        private Transform _originalParent;
        private CinemachineBrain _brain;
        private CinemachineCamera _camera;

        private bool _hasChangedParent;
        private Vector3 _originalLocalPosition;
        private Quaternion _originalLocalRotation;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _originalParent = transform.parent;
            _brain = UnityEngine.Camera.main?.GetComponent<CinemachineBrain>();
            _camera = GetComponent<CinemachineCamera>();
            _originalLocalPosition = transform.localPosition;
            _originalLocalRotation = transform.localRotation;
        }

        private void OnEnable()
        {
            transform.SetParent(_originalParent, false);
            transform.localPosition = _originalLocalPosition;
            transform.localRotation = _originalLocalRotation;
            GameManager.Instance.GameEvents.OnCameraChangeParent += ChangeCameraParent;
        }

        private void OnDisable()
        {
            GameManager.Instance.GameEvents.OnCameraChangeParent -= ChangeCameraParent;
        }

        private void ChangeCameraParent(string cameraName)
        {
            if (_camera.name == cameraName)
            {
                transform.SetParent(null, true);
            }
        }
    }
}
