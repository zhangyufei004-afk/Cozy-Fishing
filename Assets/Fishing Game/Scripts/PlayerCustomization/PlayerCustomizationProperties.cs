using UnityEngine;

namespace FishingGame.PlayerCustomization
{
    /// <summary>
    /// Internal class to store all the material properties the player has customised. Applies the materials to 
    /// the player when the player's mesh is enabled. 
    /// </summary>
    internal class PlayerCustomizationProperties : MonoBehaviour
    {
        public static Material HatMaterial { get; set; }
        public static Material HairMaterial { get; set; }

        [Header("Mesh Renderers")]
        [SerializeField] private SkinnedMeshRenderer hatRenderer;
        [SerializeField] private SkinnedMeshRenderer hairRenderer;
        
        void OnEnable()
        {
            ForceMaterialUpdate();
        }

        internal void ForceMaterialUpdate()
        {
            if (HatMaterial != null)
            {
                hatRenderer.sharedMaterial = HatMaterial;
            }
            if (HairMaterial != null)
            {
                hairRenderer.sharedMaterial = HairMaterial;
            }
        }
    }
}
