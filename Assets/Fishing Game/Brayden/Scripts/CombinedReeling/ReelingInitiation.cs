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
            }

            RodBobber.transform.position = points[points.Length - 1];

            Vector3 rodBobberModifyY = new Vector3(RodBobber.transform.position.x, 0, RodBobber.transform.position.z);

            _playerTrajectoryLine.SetPositions(points);
        }



    }
}
