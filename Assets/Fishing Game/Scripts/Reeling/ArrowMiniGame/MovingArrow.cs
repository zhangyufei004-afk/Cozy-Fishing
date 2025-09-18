using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This class is used for the moving arrow objects during the arrow minigame
    /// This contains data for its speed, how it looks and logic to make it move
    /// </summary>
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

        [SerializeField]
        [Tooltip("The rate at which this fades out")]
        private float fadeRate;

        private ArrowSpawner _spawner;
        private ArrowGoalPoints _goalPoint;

        private bool _isActive = false;
        private bool _canBePressed = false;
        private bool _closestArrow = false;
        private float _speedScalar = 1f;
        
        private Image _arrowImage;
        private float _acceptanceDistance = 250f;

        private EMovementDirection _typeOfArrow;
        private float _targetAlpha;

        private void OnEnable()
        {
            _arrowImage = GetComponent<Image>();
        }

        private void Update()
        {
            if (_isActive)
            {
                MovementLogicAndChecks();
                CheckIfFailed();
            }
        }

        private void OnDisable()
        {
            Destroy(gameObject);
        }

        #region Public Functions
        /// <summary>
        /// Sets the tpye of arrow this is
        /// The type of arrow is based on the enum EMovementDirection
        /// It is inputed as a integer here then that integer is converted into the enum
        /// </summary>
        /// <param name="typeOfArrowAsInt">The integer value of the enum</param>
        public void SetTypeOfArrow(int typeOfArrowAsInt)
        {
            _typeOfArrow = (EMovementDirection)typeOfArrowAsInt;
            DetermineArrowSprite();
        }

        /// <summary>
        /// Run when an arrow reaches the failpoint
        /// </summary>
        public void ArrowFailed()
        {
            _isActive = false;
            _spawner.GetMasterScript().RemoveArrowFromPressList(this);
            StartFadeAway(false);
            ArrowMiniGameMaster masterScript = _spawner.GetMasterScript();
            masterScript.ArrowFailedToBePressed(this);
        }

        /// <summary>
        /// Sets the arrow color based on if it was failed or a succsess then starts the fade away coroutine
        /// </summary>
        /// <param name="wasASuccsess">Input true if this arrow was pressed properly, otherwise false</param>
        public void StartFadeAway(bool wasASuccsess)
        {
            _spawner.RemoveActiveArrow(this);
            if (wasASuccsess) { _arrowImage.color = Color.grey; }
            else { _arrowImage.color = Color.red; }

            StartCoroutine(FadeAway());
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
        /// Deactivates the arrow
        /// </summary>
        public void DeactivateArrow()
        {
            _isActive = false;
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
        /// Returns this arrowws EMovementDirection as its int value
        /// </summary>
        /// <returns>Int value of the movementdirection enum</returns>
        public int GetDirectionEnumAsInt()
        {
            return (int)_typeOfArrow;
        }

        #endregion

        #region GameTimeData

        /// <summary>
        /// Updates the location of the arrow
        /// Checks if it is range of the goal point and if so changes its color
        /// and updates its _canBePressed value
        /// </summary>
        private void MovementLogicAndChecks()
        {
            Vector3 currentPosition = transform.position;
            Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y - _speedScalar * Time.deltaTime, currentPosition.z);

            transform.position = newPosition;

            if (_goalPoint.CheckIfObjectIsInRange(_acceptanceDistance, gameObject))
            {
                _canBePressed = true;
                _arrowImage.color = Color.green;
                if (_spawner.GetMasterScript().DoesThisContainArrow(this) == false) { _spawner.GetMasterScript().AddArrowToPressList(this); }
            }
            else
            {
                _canBePressed = false;
                _arrowImage.color = Color.white;
                if (_spawner.GetMasterScript().DoesThisContainArrow(this) == true) { _spawner.GetMasterScript().RemoveArrowFromPressList(this); }
            }
        }

        /// <summary>
        /// Runs a switch on the type of arrow enum,
        /// will set its sprite based on that value
        /// If the enum has been modified for some reason with no new value set
        /// this will default to making it a downArrowSprite
        /// </summary>
        private void DetermineArrowSprite()
        {
            switch (_typeOfArrow)
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
                case EMovementDirection.Down:
                    _arrowImage.sprite = downArrowSprite;
                    break;
            }
        }

        /// <summary>
        /// Checks if the arrow has gone through the fail spot or not
        /// If so runs arrowfailed
        /// </summary>
        private void CheckIfFailed()
        {
            if (_goalPoint.CheckIfFailSpot(_acceptanceDistance, gameObject))
            {
                ArrowFailed();
            }
        }

        #endregion

        #region Timer

        /// <summary>
        /// Slowly fades away the arrow
        /// </summary>
        /// <returns>Arrow fades away</returns>
        private IEnumerator FadeAway()
        {
            _targetAlpha = 0.1f;
            Color currentColor = _arrowImage.color;
            while (Mathf.Abs(currentColor.a - _targetAlpha) > 0.0001f)
            {
                currentColor.a = Mathf.Lerp(currentColor.a, _targetAlpha, fadeRate * Time.deltaTime);
                _arrowImage.color = currentColor;
                yield return null;
            }

            Destroy(gameObject);
        }

        #endregion
    }
}
