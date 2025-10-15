using FishingGame.Reeling;
using UnityEngine;

namespace FishingGame
{
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField]
        private ReelingMaster reelingMaster;

        [SerializeField]
        private Transform playerPos;
        [SerializeField]
        private Terrain terrain;

        [SerializeField]
        private AudioSource castSound;
        [SerializeField]
        private AudioSource reelingSound;
        [SerializeField]
        private AudioClip[] reelingSounds;
        [SerializeField]
        private float reelingPitch;

        [SerializeField]
        private AudioSource footsteps;
        [SerializeField]
        private AudioClip[] grassFootsteps;

        [SerializeField]
        private SliderMiniGame sliderMiniGame;
        [SerializeField]
        private GameObject sliderCanvas;

        [SerializeField]
        private RealisticMiniGameMaster realisticMiniGame;
        [SerializeField]
        private GameObject realisticCanvas;


        private bool _reelPlaying = false;
        private int _grassFootstepNum = 0;
        private AudioClip _currentClip;

        /*private enum SurfaceType { Grass, Sand, Water, Path}
        [SerializeField]
        private SurfaceType[] textureToSurfaceMap;
        private Vector3 _terrainPos;
        private Vector3 _terrainSize;*/

        private void Start()
        {
            //_terrainPos = terrain.transform.position;
            //_terrainSize = terrain.terrainData.size;
            reelingSound.clip = reelingSounds[1];
        }
        private void Update()
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

            if(newClip != _currentClip)
            {
                _currentClip = newClip;
                reelingSound.clip = _currentClip;

                if (_reelPlaying)
                    reelingSound.Play();
            }

        }

        /// <summary>
        /// is called from animation trigger, plays cast sound
        /// </summary>
        public void CastSound()
        {
            castSound.Play();
        }

        /// <summary>
        /// is called from animation trigger, plays random footstep sound based on what player is walking on
        /// </summary>
        public void FootStep()
        {
            if (_grassFootstepNum == grassFootsteps.Length) 
                _grassFootstepNum = 0;

            footsteps.clip = grassFootsteps[_grassFootstepNum];
            _grassFootstepNum++;

            footsteps.Play();

            //SurfaceType surface = GetSurfaceAtPosition(playerPos.position);
            //Debug.Log(surface);
        }

        /// <summary>
        /// Determines what surface the player is standing on by converting player world position to terrain coordinates
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        /*private SurfaceType GetSurfaceAtPosition(Vector3 worldPos)
        {
            float normX = (worldPos.x - _terrainPos.x) / _terrainSize.x;
            float normZ = (worldPos.z - _terrainSize.z) / _terrainSize.z;

            normX = Mathf.Clamp01(normX);
            normZ = Mathf.Clamp01(normZ);

            int mapX = Mathf.FloorToInt(normX * terrain.terrainData.alphamapWidth);
            int mapZ = Mathf.FloorToInt(normZ * terrain.terrainData.alphamapHeight);

            float[,,] splatMapData = terrain.terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

            int maxIndex = 0;
            float maxMix = 0;

            string weights = "Weights: ";
            for (int i = 0; i < splatMapData.GetLength(2); i++)
            {
                weights += $"[{i}]={splatMapData[0, 0, i]:F2} ";
            }
            Debug.Log(weights);

            for (int i = 0; i < splatMapData.GetLength(2); i++)
            {
                if (splatMapData[0,0,i] > maxMix)
                {
                    maxIndex = i;
                    maxMix = splatMapData[0,0,i];
                }
            }

            if (maxIndex < textureToSurfaceMap.Length)
                return textureToSurfaceMap[maxIndex];
            else
                return SurfaceType.Grass; 
        }*/
    }
}
