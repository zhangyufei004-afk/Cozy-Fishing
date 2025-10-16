using FishingGame.Reeling;
using UnityEngine;

namespace FishingGame
{
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField]
        private ReelingMaster reelingMaster;
        [SerializeField]
        private ReelingInitiation reelingInitiation;
        [SerializeField]
        private AudioSource castSound;
        [SerializeField]
        private AudioSource lineCastSound;
        [SerializeField]
        private AudioSource splashesSound;
        [SerializeField]
        private AudioSource reelingSound;
        [SerializeField]
        private AudioClip[] reelingSounds;

        [SerializeField]
        private GroundCheck groundCheck;
        [SerializeField]
        private AudioSource grassFootstep;
        [SerializeField]
        private AudioSource woodFootstep;
        [SerializeField]
        private AudioClip[] grassFootsteps;
        [SerializeField]
        private AudioClip[] woodFootsteps;

        [SerializeField]
        private SliderMiniGame sliderMiniGame;
        [SerializeField]
        private GameObject sliderCanvas;

        [SerializeField]
        private RealisticMiniGameMaster realisticMiniGame;
        [SerializeField]
        private GameObject realisticCanvas;

        [SerializeField]
        private Animator playerAnimator;


        private bool _reelPlaying = false;
        private bool _splashesPlaying = false;
        private int _grassFootstepNum = 0;
        private int _woodFootstepNum = 0;
        private AudioClip _currentClip;



        private void Start()
        {
            reelingSound.clip = reelingSounds[1];
        }
        private void Update()
        {
            MiniGameSounds();

            if (lineCastSound.isPlaying && reelingInitiation.StageOne)
            {
                lineCastSound.Stop();
            }
            else if (lineCastSound.isPlaying && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                lineCastSound.Stop();
            }

            if (reelingInitiation.StageOne)
            {
                reelingInitiation.StageOne = false;
            }

            if (reelingInitiation.IsFishAtHook() && !_splashesPlaying)
            {
                _splashesPlaying = true;
                splashesSound.Play();
            }

            if (!reelingInitiation.IsFishAtHook())
                _splashesPlaying = false;

        }

        /// <summary>
        /// is called from animation trigger, plays cast sound
        /// </summary>
        public void CastSound()
        {
            castSound.Play();

            lineCastSound.Play();
        }

        /// <summary>
        /// is called from animation trigger, plays random footstep sound based on what player is walking on
        /// </summary>
        public void FootStep()
        {
            if (groundCheck.OnWood)
            {
                if (_woodFootstepNum == woodFootsteps.Length)
                    _woodFootstepNum = 0;

                woodFootstep.clip = woodFootsteps[_woodFootstepNum];
                _woodFootstepNum++;

                woodFootstep.Play();
            }
            else
            {
                if (_grassFootstepNum == grassFootsteps.Length)
                    _grassFootstepNum = 0;

                grassFootstep.clip = grassFootsteps[_grassFootstepNum];
                _grassFootstepNum++;

                grassFootstep.Play();
            }
        }

        /// <summary>
        /// Handles minigame reeling sounds
        /// </summary>
        private void MiniGameSounds()
        {
            if (reelingMaster.IsFishing && !_reelPlaying)
                reelingSound.Play();

            else if (!reelingMaster.IsFishing && _reelPlaying)
                reelingSound.Stop();

            _reelPlaying = reelingMaster.IsFishing;

            AudioClip newClip = null;

            if (sliderCanvas.activeSelf)
            {
                newClip = sliderMiniGame.GetInput() ? reelingSounds[2] : reelingSounds[0];
            }
            else if (realisticCanvas.activeSelf)
            {
                newClip = realisticMiniGame.IsProgressing() ? reelingSounds[2] : reelingSounds[0];
            }
            else
            {
                newClip = reelingSounds[1];
            }

            if (newClip != _currentClip)
            {
                _currentClip = newClip;
                reelingSound.clip = _currentClip;

                if (_reelPlaying)
                    reelingSound.Play();
            }
        }
    }
}
