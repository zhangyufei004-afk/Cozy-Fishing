using ReelingMasterScript;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PrototypeFishingMechanics
{
    public class ReelingInitiation : MonoBehaviour
    {
        #region Public Variables

        public ReelingMaster ReelingMasterScript;


        #endregion

        #region Private Fields



        private Vector3 _aimPoint;
        private Vector3 _initialPosition;
        private Vector3 _CastDirection;
        private bool _isCharging = false;
        private float _chargePower = 0;
        private float _maxCharge = 10;

        [SerializeField]
        private LineRenderer _playerTrajectoryLine;

        [SerializeField]
        private int ChargeScalar;

        [SerializeField]
        private GameObject RodEndPoint;

        [SerializeField]
        private GameObject RodBobber;

        [SerializeField]
        private FishingBobber RodBobberScript;


        #endregion

        public void Update()
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                RightClickHeld();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                LeftClick();
            }
        }


        private void ChargeLine()
        {
            
        }

        private void LeftClick()
        {
            if (_isCharging == true)
            {
                _isCharging = false;
                _playerTrajectoryLine.enabled = false;
                RodBobberScript.CanCatchFish = true;

                ThrowLine(_chargePower);

                ReelingMasterScript.TEMPSTART();
                
            }
            else
            {

            }
                
        }

        private void RightClickUsed()
        {
            if (_isCharging != true)
            {
                _isCharging = true;
                _playerTrajectoryLine.enabled = true;
            }
        }

        private void RightClickHeld()
        {
            _isCharging = true;
            _chargePower += Time.deltaTime * ChargeScalar;

            _initialPosition = RodEndPoint.transform.position;
            _CastDirection = gameObject.transform.forward;
            Vector3 castVelocity = (_CastDirection + _CastDirection).normalized * Mathf.Min(_chargePower, _maxCharge);
            ShowTrajectory(_initialPosition + _CastDirection, castVelocity);
        }

        private void ThrowLine(float force)
        {

        }


        public void ShowTrajectory(Vector3 origin, Vector3 speed)
        {
            Vector3[] points = new Vector3[10];
            _playerTrajectoryLine.positionCount = points.Length;
            for (int i = 0; i < points.Length; i++)
            {
                float time = i * 0.1f;
                points[i] = origin + speed * time + 0.5f * Physics.gravity * time * time;

                // This here is currently how I cut down how far the rod goes
                // And try to somewhat accuratley place the bobber
                // TODO: Figure out how to replace this with something better
                if (points[i].y < this.transform.position.y - 2)
                {
                    points[i] = points[i-1];
                }
            }

            RodBobber.transform.position = points[points.Length - 1];

            _playerTrajectoryLine.SetPositions(points);
        }



    }
}
