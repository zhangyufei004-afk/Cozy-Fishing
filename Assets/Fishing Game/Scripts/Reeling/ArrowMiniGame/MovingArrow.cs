using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
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

        [Header("Minigame Data")]

        private ArrowSpawner _spawner;

        private bool _isActive = false;
        private bool _canBePressed = false;
        private float _speedScalar = 1f;
        [SerializeField]
        private ArrowGoalPoints _goalPoint;
        private Image _arrowImage;
        private float _acceptanceDistance = 250f;

        private EMovementDirection _typeOfArrow;
        private EMovementDirection _inputedAction;

        private InputAction _directionAction;
        private float _targetAlpha;
        [SerializeField]
        [Tooltip("The rate at which this fades out")]
        private float fadeRate;



        private void OnEnable()
        {
            _arrowImage = GetComponent<Image>();

            int enumValueCount = System.Enum.GetValues(typeof(EMovementDirection)).Length;
            _typeOfArrow = (EMovementDirection)Random.Range(0, enumValueCount + 1);
            DetermineArrowSprite();

            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap uiActionMap = inputActions.FindActionMap("UI");
            uiActionMap.Enable();
            _directionAction = uiActionMap.FindAction("ArrowMiniGame");

            
        }

        private void Update()
        {
            if (_isActive)
            {
                Vector3 currentPosition = transform.position;
                Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y - _speedScalar * Time.deltaTime, currentPosition.z);

                transform.position = newPosition;

                if (_goalPoint.CheckUIOverlap(_acceptanceDistance, gameObject))
                {
                    _canBePressed = true;
                    _arrowImage.color = Color.green;
                }
                else
                {
                    _canBePressed = false;
                    _arrowImage.color = Color.white;
                }

                if (_canBePressed)
                {
                    if (_directionAction.WasPressedThisFrame())
                    {
                        Vector2 direction = _directionAction.ReadValue<Vector2>();
                        if (GetMovementDirection(direction) == _typeOfArrow)
                        {
                            ArrowSuccsessfullyPressed();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Actives the arrow and sets its speed to the inputed parameter
        /// </summary>
        /// <param name="speedScalar">The speed to set</param>
        public void ActivateArrow(float speedScalar)
        {
            _isActive = true;
            _speedScalar = speedScalar;
        }

        /// <summary>
        /// Changes the speed of the arrow based on what is inputed
        /// </summary>
        /// <param name="speedScalar">The new speed to set</param>
        public void ChangeSpeed(float speedScalar)
        {
            _speedScalar = speedScalar;
        }

        /// <summary>
        /// Assigns this arrow to the spawner inputed, this should generally be the spawner that spawned this arrow
        /// </summary>
        /// <param name="spawner">The spawner to be assigned to</param>
        public void SetSpawner(ArrowSpawner spawner)
        {
            _spawner = spawner;
            _goalPoint = _spawner.GetArrowGoal();
        }

        /// <summary>
        /// Runs a switch on the type of arrow enum,
        /// will set its sprite based on that value
        /// If the enum has been modified for some reason with no new value set
        /// this will default to making it a downArrowSprite
        /// </summary>
        private void DetermineArrowSprite()
        {
            switch ( _typeOfArrow )
            {
                case EMovementDirection.Left:
                    _arrowImage.sprite = leftArrowSprite;
                    break;
                case EMovementDirection.Right:
                    _arrowImage.sprite = rightArrowSprite;
                    break;
                case EMovementDirection.Up:
                    _arrowImage.sprite = upArrowSprite;
                    break;
                default:
                    _arrowImage.sprite = downArrowSprite;
                    break;
            }
        }

        /// <summary>
        /// Returns an EMovementDirection enum value based on the inputed Vector2
        /// </summary>
        /// /// <param name="directionValue">The Vector2 input from the action</param>
        private EMovementDirection GetMovementDirection(Vector2 directionValue)
        {
            if (directionValue.x > 0) { return EMovementDirection.Right; }
            if (directionValue.x < 0) { return EMovementDirection.Left; }
            if (directionValue.y > 0) { return EMovementDirection.Up; }
            return EMovementDirection.Down;
        }

        /// <summary>
        /// Run when an arrow is sucsessfully pressed
        /// </summary>
        private void ArrowSuccsessfullyPressed()
        {
            _isActive = false;
            ArrowMiniGameMaster masterScript = _spawner.GetMasterScript();
            masterScript.ArrowSuccsessfullyPressed(this);
            StartFadeAway(true);
        }

        /// <summary>
        /// Run when an arrow reaches the failpoint
        /// </summary>
        private void ArrowFailed()
        {
            _isActive = false;
            ArrowMiniGameMaster masterScript = _spawner.GetMasterScript();
            masterScript.ArrowFailedToBePressed(this);
        }

        /// <summary>
        /// Sets the arrow color based on if it was failed or a succsess then starts the fade away coroutine
        /// </summary>
        /// <param name="wasASuccsess">Input true if this arrow was pressed properly, otherwise false</param>
        private void StartFadeAway(bool wasASuccsess)
        {
            if (wasASuccsess) { _arrowImage.color = Color.grey; }
            else { _arrowImage.color = Color.red; }

            StartCoroutine(FadeAway());
        }

        /// <summary>
        /// Slowly fades away the arrow
        /// </summary>
        /// <returns>Arrow fades away</returns>
        private IEnumerator FadeAway()
        {
            _targetAlpha = 0.1f;
            Color currentColor = _arrowImage.color;
            while(Mathf.Abs(currentColor.a - _targetAlpha) > 0.0001f)
            {
                currentColor.a = Mathf.Lerp(currentColor.a, _targetAlpha, fadeRate * Time.deltaTime);
                _arrowImage.color = currentColor;
                yield return null;
            }

            Destroy(gameObject);
        }

    }
}
