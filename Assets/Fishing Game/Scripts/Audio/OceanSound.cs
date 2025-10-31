using UnityEngine;

namespace FishingGame
{
    public class OceanSound : MonoBehaviour
    {
        [SerializeField]
        private Transform player;
        [SerializeField]
        private AudioSource sound;
        [SerializeField]
        private AudioLowPassFilter lpf;

        [SerializeField]
        private float islandSizeX;
        [SerializeField] 
        private float islandSizeZ;

        [SerializeField]
        private float lpfMin;
        [SerializeField]
        private float lpfMax;


        private void Update()
        {
            Vector3 localPos = player.position - transform.position;

            float distanceX = Mathf.Clamp01(Mathf.Abs(localPos.x) / islandSizeX);
            float distanceZ = Mathf.Clamp01(Mathf.Abs(localPos.z) / islandSizeZ);

            float distance = Mathf.Max(distanceX, distanceZ);

            float volume = distance;
            sound.volume = Mathf.Lerp(sound.volume, distance, Time.deltaTime * 5f);

            float lpsCutoff = Mathf.Lerp(lpfMin, lpfMax, distance * distance * distance);
            lpf.cutoffFrequency = Mathf.Lerp(lpf.cutoffFrequency, lpsCutoff, Time.deltaTime * 5f);
        }
    }
}
