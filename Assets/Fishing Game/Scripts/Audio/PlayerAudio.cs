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
        private FishingRod fishingRod;
        [SerializeField]
        private AudioSource castSound;
        [SerializeField]
        private AudioSource perfectCastSound;
        [SerializeField]
        private AudioSource lineCastSound;

        [SerializeField]
        private AudioSource splashesSound;
        [SerializeField]
        private AudioSource realisticReelingSound;
        [SerializeField]
        private AudioSource sliderReelingSound;

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


        private bool _realisticReelPlaying = false;
        private bool _sliderReelPlaying = false;
        private bool _splashesPlaying = false;
        private int _grassFootstepNum = 0;
        private int _woodFootstepNum = 0;


        private void Update()
        {
            RealisticMiniGameAudio();
            SliderMiniGame();

            if (lineCastSound.isPlaying && reelingInitiation.IsStageOne())
            {
                lineCastSound.Stop();
            }
            else if (lineCastSound.isPlaying && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                lineCastSound.Stop();
            }

            if (reelingInitiation.IsFishAtHook() && !_splashesPlaying)
            {
                _splashesPlaying = true;
                splashesSound.Play();
            }

            if (!reelingInitiation.IsFishAtHook())
            {
                _splashesPlaying = false;
                splashesSound.Stop();
            }
                

        }

        /// <summary>
        /// is called from animation trigger, plays cast sound
        /// </summary>
        public void CastSound()
        {
            if(fishingRod.GetChargeLevel() == 1)
            {
                castSound.pitch = 1.25f;
                castSound.volume = 0.5f;
                castSound.Play();
            }
            else if (fishingRod.GetChargeLevel() == 2)
            {
                castSound.pitch = 1f;
                castSound.volume = 0.8f;
                castSound.Play();
            }
            else if (fishingRod.GetChargeLevel() == 3)
            {
                castSound.pitch = 0.85f;
                castSound.volume = 1;
                castSound.Play();
                perfectCastSound.Play();
            }

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
        /// Handles realistic minigame reeling sounds
        /// </summary>
        private void RealisticMiniGameAudio()
        {
            if (realisticCanvas.activeSelf)
            {
                bool isProgressing = realisticMiniGame.IsProgressing();

                if (isProgressing)
                {
                    if (!_realisticReelPlaying)
                    {
                        realisticReelingSound.Play();
                        _realisticReelPlaying = true;
                    }
                }
                else if (_realisticReelPlaying)
                {
                    realisticReelingSound.Stop();
                    _realisticReelPlaying = false;
                }
            }
            else if (_realisticReelPlaying)
            {
                realisticReelingSound.Stop();
                _realisticReelPlaying = false;
            }
        }

        /// <summary>
        /// Handles slider mini game audio
        /// </summary>
        private void SliderMiniGame()
        {
            if (sliderCanvas.activeSelf)
            {
                bool input = sliderMiniGame.GetInput();

                if (input)
                {
                    if (!_sliderReelPlaying)
                    {
                        sliderReelingSound.volume = 1f;
                        sliderReelingSound.Play();
                        _sliderReelPlaying = true;
                    }

                    sliderReelingSound.volume = Mathf.MoveTowards(sliderReelingSound.volume, 1f, Time.deltaTime * 10f);
                }
                else if (_sliderReelPlaying)
                {
                    sliderReelingSound.volume = Mathf.MoveTowards(sliderReelingSound.volume, 0f, Time.deltaTime * 10f);

                    if (sliderReelingSound.volume <= 0.01f)
                    {
                        sliderReelingSound.Stop();
                        _sliderReelPlaying = false;
                    }
                }
            }
            else if (_sliderReelPlaying)
            {
                sliderReelingSound.volume = Mathf.MoveTowards(sliderReelingSound.volume, 0f, Time.deltaTime * 10f);

                if (sliderReelingSound.volume <= 0.01f)
                {
                    sliderReelingSound.Stop();
                    _sliderReelPlaying = false;
                }
            }
        }
    }
}
