using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Reeling
{
    public class MovingArrow : MonoBehaviour
    {
        [Header("Sprites used")]

        [SerializeField]
        [Tooltip("The up arrow sprite")]
        private Sprite upArrowSprite;

        [SerializeField]
        [Tooltip("The down arrow sprite")]
        private Sprite downArrowSprite;

        [SerializeField]
        [Tooltip("The left arrow sprite")]
        private Sprite leftArrowSprite;

        [SerializeField]
        [Tooltip("The right arrow sprite")]
        private Sprite rightArrowSprite;

        private bool _isActive = false;
        private float _speedScalar = 1f;

        private EMovementDirection _typeOfArrow;


        private void OnEnable()
        {
            int enumValueCount = System.Enum.GetValues(typeof(EMovementDirection)).Length;
            _typeOfArrow = (EMovementDirection)Random.Range(0, enumValueCount + 1);
            DetermineArrowSprite();
        }

        private void Update()
        {
            if (_isActive)
            {
                Vector3 currentPosition = transform.position;
                Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y - _speedScalar * Time.deltaTime, currentPosition.z);

                transform.position = newPosition;
            }
        }

        public void ActivateArrow(float speedScalar)
        {
            _isActive = true;
            _speedScalar = speedScalar;
        }

        public void ChangeSpeed(float speedScalar)
        {
            _speedScalar = speedScalar;
        }

        private void DetermineArrowSprite()
        {
            switch ( _typeOfArrow )
            {
                case EMovementDirection.Left:
                    GetComponent<Image>().sprite = leftArrowSprite;
                    break;
                case EMovementDirection.Right:
                    GetComponent<Image>().sprite = rightArrowSprite;
                    break;
                case EMovementDirection.Up:
                    GetComponent<Image>().sprite = upArrowSprite;
                    break;
                default:
                    GetComponent<Image>().sprite = downArrowSprite;
                    break;
            }
        }

    }
}
