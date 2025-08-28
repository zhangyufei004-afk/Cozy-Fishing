using UnityEngine;

namespace FishingGame.PlayerCustomization
{
    /// <summary>
    /// Handles color changes for character parts and allows resetting to default colors.
    /// Supports four preset colors per part and a reset option.
    /// </summary>
    public class CharacterColorCustomizer : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] private Material hatMaterial;
        [SerializeField] private Material hairMaterial;

        [Header("Default Colors")]
        [SerializeField] private Color defaultHatColor = Color.white;
        [SerializeField] private Color defaultHairColor = Color.white;

        [Header("Hat Colors")]
        [SerializeField] private Color hatColor1 = Color.red;
        [SerializeField] private Color hatColor2 = Color.green;
        [SerializeField] private Color hatColor3 = Color.blue;
        [SerializeField] private Color hatColor4 = Color.yellow;

        [Header("Hair Colors")]
        [SerializeField] private Color hairColor1 = new Color(1f, 0.5f, 0f); // Orange
        [SerializeField] private Color hairColor2 = Color.cyan;
        [SerializeField] private Color hairColor3 = Color.magenta;
        [SerializeField] private Color hairColor4 = Color.black;

        /// <summary>
        /// Apply a color to the given material.
        /// </summary>
        private void ApplyColor(Material mat, Color color)
        {
            if (mat != null)
            {
                if (mat.HasProperty("_Color"))
                    mat.color = color;
                else if (mat.HasProperty("_Diffuse"))
                    mat.SetColor("_Diffuse", color);
                else
                    Debug.LogWarning($"Material {mat.name} has no _Color or _Diffuse property.");
            }
            else
            {
                Debug.LogWarning("Material is null, cannot apply color.");
            }
        }

        #region Hat Color Methods
        public void SetHatColor1() => ApplyColor(hatMaterial, hatColor1);
        public void SetHatColor2() => ApplyColor(hatMaterial, hatColor2);
        public void SetHatColor3() => ApplyColor(hatMaterial, hatColor3);
        public void SetHatColor4() => ApplyColor(hatMaterial, hatColor4);
        #endregion

        #region Hair Color Methods
        public void SetHairColor1() => ApplyColor(hairMaterial, hairColor1);
        public void SetHairColor2() => ApplyColor(hairMaterial, hairColor2);
        public void SetHairColor3() => ApplyColor(hairMaterial, hairColor3);
        public void SetHairColor4() => ApplyColor(hairMaterial, hairColor4);
        #endregion

        /// <summary>
        /// Reset all customizable parts to their default colors.
        /// </summary>
        public void ResetColors()
        {
            ApplyColor(hatMaterial, defaultHatColor);
            ApplyColor(hairMaterial, defaultHairColor);
        }
    }
}


