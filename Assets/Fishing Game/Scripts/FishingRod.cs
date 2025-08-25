using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PrototypeFishingMechanics
{
    public class FishingRod : MonoBehaviour
    {
        #region Private Properties

        private bool IsFishInRange => _currentFishInRange != null;

        #endregion
        
        #region Private Fields

        #region Serialized Fields

        [SerializeField] private Animation anim;
        [SerializeField] private List<AnimationClip> animationClips;

        #endregion
        
        private GameObject _currentFishInRange;
        private bool _isRodExtended;

        #endregion

        private void Start()
        {
            _isRodExtended = false;
        }

        /// <summary>
        /// OnTriggerEnter is a Unity Event Function, invoked when another collider enters a trigger. 
        /// </summary>
        /// <param name="other">The Other Collider that has entered the Trigger</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Fish"))
            {
                _currentFishInRange = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Fish") && _currentFishInRange == other.gameObject)
            {
                _currentFishInRange = null;
            }
        }

        private void Update()
        {
            // Flipped If Statement for early return, if we dont have mouse input, dont extend the rod or catch a fish
            if (!Input.GetMouseButtonDown(0)) return;
            
            if (!_isRodExtended)
            {
                _isRodExtended = anim.Play(animationClips[0].name);
                return;
            }

            if (IsFishInRange)
            {
                _currentFishInRange.transform.SetParent(this.transform, true);
                _currentFishInRange.transform.localPosition = Vector3.zero;
                _currentFishInRange.transform.localRotation = Quaternion.identity;
                _currentFishInRange.transform.localScale = new Vector3(
                    _currentFishInRange.transform.localScale.x * 4, 
                    _currentFishInRange.transform.localScale.y, 
                    _currentFishInRange.transform.localScale.z
                ); 
                
                FishAI fishAI = _currentFishInRange.GetComponent<FishAI>();
                fishAI.ShouldPath = false;
                StartCoroutine(DestroyCaughtFish());
            }
            // TODO: ADD A SCAPERING MECHANIC (if we choose this mechanic)
            _isRodExtended = !anim.Play(animationClips[1].name);
        }

        IEnumerator DestroyCaughtFish()
        {
            yield return new WaitForSeconds(5f);
            Destroy(_currentFishInRange);
            _currentFishInRange = null;
        }
    }
}