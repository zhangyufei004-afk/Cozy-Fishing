using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.PlayerCustomization
{
    /// <summary>
    /// Character color customization with sliders (RGB for hat and hair),
    /// apply buttons, and preset color buttons (e.g. 4 fixed colors).
    /// </summary>
    public class CharacterColorCustomizer : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Material hatMaterial;
        [SerializeField] private Material hairMaterial;

        [Header("Default Colors")]
        [SerializeField] private Color defaultHatColor = Color.white;
        [SerializeField] private Color defaultHairColor = Color.white;

        [Header("Hat Sliders (0..1)")]
        [SerializeField] private Slider hatR;
        [SerializeField] private Slider hatG;
        [SerializeField] private Slider hatB;

        [Header("Hair Sliders (0..1)")]
        [SerializeField] private Slider hairR;
        [SerializeField] private Slider hairG;
        [SerializeField] private Slider hairB;

        [Header("Preview & Apply Buttons")]
        [SerializeField] private Button hatApplyButton;
        [SerializeField] private Button hairApplyButton;

        [Header("Preset Buttons")]
        [SerializeField] private Button[] hatPresetButtons; // 4 hat colors
        [SerializeField] private Color[] hatPresetColors;   // Must match button count
        [SerializeField] private Button[] hairPresetButtons; // 4 hair colors
        [SerializeField] private Color[] hairPresetColors;   // Must match button count

        [Header("Preview Character Properties")]
        [SerializeField] PlayerCustomizationProperties customizationProperties;

        private Image _hatBtnImage;
        private Image _hairBtnImage;

        private void Awake()
        {
            if (hatApplyButton) _hatBtnImage = hatApplyButton.GetComponent<Image>();
            if (hairApplyButton) _hairBtnImage = hairApplyButton.GetComponent<Image>();
        }

        private void OnEnable()
        {
            // Hook slider change -> update preview
            if (hatR) hatR.onValueChanged.AddListener(_ => ApplyHatFromSliders());
            if (hatG) hatG.onValueChanged.AddListener(_ => ApplyHatFromSliders());
            if (hatB) hatB.onValueChanged.AddListener(_ => ApplyHatFromSliders());

            if (hairR) hairR.onValueChanged.AddListener(_ => ApplyHairFromSliders());
            if (hairG) hairG.onValueChanged.AddListener(_ => ApplyHairFromSliders());
            if (hairB) hairB.onValueChanged.AddListener(_ => ApplyHairFromSliders());

            // Buttons apply color
            if (hatApplyButton) hatApplyButton.onClick.AddListener(ApplyHatFromSliders);
            if (hairApplyButton) hairApplyButton.onClick.AddListener(ApplyHairFromSliders);

            // Preset buttons
            for (int i = 0; i < hatPresetButtons.Length && i < hatPresetColors.Length; i++)
            {
                int index = i; // local copy
                hatPresetButtons[i].onClick.AddListener(() => ApplyHatPreset(index));

                // Show the color on the button
                var img = hatPresetButtons[i].GetComponent<Image>();
                if (img) img.color = hatPresetColors[i];
            }

            for (int i = 0; i < hairPresetButtons.Length && i < hairPresetColors.Length; i++)
            {
                int index = i;
                hairPresetButtons[i].onClick.AddListener(() => ApplyHairPreset(index));

                var img = hairPresetButtons[i].GetComponent<Image>();
                if (img) img.color = hairPresetColors[i];
            }

            // Init sliders from defaults
            SetHatSliders(defaultHatColor);
            SetHairSliders(defaultHairColor);
        }

        private void OnDisable()
        {
            if (hatR) hatR.onValueChanged.RemoveAllListeners();
            if (hatG) hatG.onValueChanged.RemoveAllListeners();
            if (hatB) hatB.onValueChanged.RemoveAllListeners();

            if (hairR) hairR.onValueChanged.RemoveAllListeners();
            if (hairG) hairG.onValueChanged.RemoveAllListeners();
            if (hairB) hairB.onValueChanged.RemoveAllListeners();

            if (hatApplyButton) hatApplyButton.onClick.RemoveAllListeners();
            if (hairApplyButton) hairApplyButton.onClick.RemoveAllListeners();

            foreach (var btn in hatPresetButtons)
                if (btn) btn.onClick.RemoveAllListeners();

            foreach (var btn in hairPresetButtons)
                if (btn) btn.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// Sets the hat sliders to the specified colour.
        /// </summary>
        public void SetHatSliders(Color c)
        {
            if (hatR) hatR.value = c.r;
            if (hatG) hatG.value = c.g;
            if (hatB) hatB.value = c.b;
        }

        /// <summary>
        /// Sets the hair sliders to the specified colour.
        /// </summary>
        public void SetHairSliders(Color c)
        {
            if (hairR) hairR.value = c.r;
            if (hairG) hairG.value = c.g;
            if (hairB) hairB.value = c.b;
        }

        /// <summary>
        /// Resets all the colours (Hat and Hair) to defaults.
        /// </summary>
        public void ResetAllColors()
        {
            ApplyHatColor(defaultHatColor);
            ApplyHairColor(defaultHairColor);
            SetHatSliders(defaultHatColor);
            SetHairSliders(defaultHairColor);
        }

        private void ApplyHatFromSliders() => ApplyHatColor(GetHatSliderColor());
        private void ApplyHairFromSliders() => ApplyHairColor(GetHairSliderColor());

        private void ApplyHatPreset(int index)
        {
            if (index < 0 || index >= hatPresetColors.Length) return;
            ApplyHatColor(hatPresetColors[index]);
        }

        private void ApplyHairPreset(int index)
        {
            if (index < 0 || index >= hairPresetColors.Length) return;
            ApplyHairColor(hairPresetColors[index]);
        }

        private void UpdateHatPreview()
        {
            if (_hatBtnImage) _hatBtnImage.color = GetHatSliderColor();
            customizationProperties.ForceMaterialUpdate();
        }

        private void UpdateHairPreview()
        {
            if (_hairBtnImage) _hairBtnImage.color = GetHairSliderColor();
            customizationProperties.ForceMaterialUpdate();
        }

        private Color GetHatSliderColor() =>
            new Color(hatR ? hatR.value : 0f, hatG ? hatG.value : 0f, hatB ? hatB.value : 0f, 1f);

        private Color GetHairSliderColor() =>
            new Color(hairR ? hairR.value : 0f, hairG ? hairG.value : 0f, hairB ? hairB.value : 0f, 1f);

        private void ApplyHatColor(Color color)
        {
            Material hatMaterialCopy = ApplyColor(hatMaterial, color);
            PlayerCustomizationProperties.HatMaterial = hatMaterialCopy;
            SetHatSliders(color);
            UpdateHatPreview();
        }
        private void ApplyHairColor(Color color)
        {
            Material hairMaterialCopy = ApplyColor(hairMaterial, color);
            PlayerCustomizationProperties.HairMaterial = hairMaterialCopy;
            SetHairSliders(color);
            UpdateHairPreview();
        }


        /// <summary>
        /// Creates a copy of the material and applies a colour to the given material.
        /// Supports Unity Toon Shader by using _BaseColor and _1st_ShadeColor.
        /// </summary>
        /// <returns>A copy of the original material, to be applied to the player. Can return null if no copy was able to be created</returns>
        private Material ApplyColor(Material mat, Color color)
        {
            Material materialInstance = new Material(mat);
            // UTS base color
            if (materialInstance.HasProperty("_BaseColor"))
            {
                materialInstance.SetColor("_BaseColor", color);
            }
            else if (materialInstance.HasProperty("_Color"))
            {
                materialInstance.SetColor("_Color", color);
            }

            // Sync 1st shade for toon shadow consistency
            if (materialInstance.HasProperty("_1st_ShadeColor"))
            {
                materialInstance.SetColor("_1st_ShadeColor", color * 0.8f);
            }

            return materialInstance;
        }
    }
}


