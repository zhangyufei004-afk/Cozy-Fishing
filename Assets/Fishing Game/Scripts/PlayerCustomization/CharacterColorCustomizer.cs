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
        private Color GetHatSliderColor() => new Color(
            hatR ? hatR.value : 0f,
            hatG ? hatG.value : 0f,
            hatB ? hatB.value : 0f,
            1f);

        private Color GetHairSliderColor() => new Color(
            hairR ? hairR.value : 0f,
            hairG ? hairG.value : 0f,
            hairB ? hairB.value : 0f,
            1f);
        
        [Header("Materials")]
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
        [SerializeField] private Button[] hatPresetButtons;   // 4 hat colors
        [SerializeField] private Color[] hatPresetColors;    // Must match button count
        [SerializeField] private Button[] hairPresetButtons; // 4 hair colors
        [SerializeField] private Color[] hairPresetColors;   // Must match button count

        private Image _hatBtnImage;
        private Image _hairBtnImage;

        private void Awake()
        {
            if (hatApplyButton)  _hatBtnImage  = hatApplyButton.GetComponent<Image>();
            if (hairApplyButton) _hairBtnImage = hairApplyButton.GetComponent<Image>();
        }

        private void OnEnable()
        {
            // Hook slider change -> update preview
            if (hatR) hatR.onValueChanged.AddListener(_ => UpdateHatPreview());
            if (hatG) hatG.onValueChanged.AddListener(_ => UpdateHatPreview());
            if (hatB) hatB.onValueChanged.AddListener(_ => UpdateHatPreview());

            if (hairR) hairR.onValueChanged.AddListener(_ => UpdateHairPreview());
            if (hairG) hairG.onValueChanged.AddListener(_ => UpdateHairPreview());
            if (hairB) hairB.onValueChanged.AddListener(_ => UpdateHairPreview());

            // Buttons apply color
            if (hatApplyButton)  hatApplyButton.onClick.AddListener(ApplyHatFromSliders);
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

            // Initial previews
            UpdateHatPreview();
            UpdateHairPreview();
        }

        private void OnDisable()
        {
            if (hatR) hatR.onValueChanged.RemoveAllListeners();
            if (hatG) hatG.onValueChanged.RemoveAllListeners();
            if (hatB) hatB.onValueChanged.RemoveAllListeners();

            if (hairR) hairR.onValueChanged.RemoveAllListeners();
            if (hairG) hairG.onValueChanged.RemoveAllListeners();
            if (hairB) hairB.onValueChanged.RemoveAllListeners();

            if (hatApplyButton)  hatApplyButton.onClick.RemoveAllListeners();
            if (hairApplyButton) hairApplyButton.onClick.RemoveAllListeners();

            foreach (var btn in hatPresetButtons) if (btn) btn.onClick.RemoveAllListeners();
            foreach (var btn in hairPresetButtons) if (btn) btn.onClick.RemoveAllListeners();
        }
        
        /// <summary>
        /// Sets the hat sliders to the specified colour
        /// </summary>
        /// <param name="colour">The colour to set the sliders to.</param>
        public void SetHatSliders(Color colour)
        {
            if (hatR) hatR.value = colour.r;
            if (hatG) hatG.value = colour.g;
            if (hatB) hatB.value = colour.b;
        }

        /// <summary>
        /// Sets the Hair Sliders to the colour <c>colour</c>
        /// </summary>
        /// <param name="colour">The colour to set the hair sliders to.</param>
        public void SetHairSliders(Color colour)
        {
            if (hairR) hairR.value = colour.r;
            if (hairG) hairG.value = colour.g;
            if (hairB) hairB.value = colour.b;
        }

        /// <summary>
        /// Resets all the colours (Hat and Hair)
        /// </summary>
        public void ResetAllColors()
        {
            ApplyHatColor(defaultHatColor);
            ApplyHairColor(defaultHairColor);

            SetHatSliders(defaultHatColor);
            SetHairSliders(defaultHairColor);

            UpdateHatPreview();
            UpdateHairPreview();
        }

        // === Apply from sliders ===
        private void ApplyHatFromSliders() => ApplyHatColor(GetHatSliderColor());
        private void ApplyHairFromSliders() => ApplyHairColor(GetHairSliderColor());

        // === Apply from presets ===
        private void ApplyHatPreset(int index)
        {
            if (index < 0 || index >= hatPresetColors.Length) return;
            ApplyHatColor(hatPresetColors[index]);
            SetHatSliders(hatPresetColors[index]);
            UpdateHatPreview();
        }

        private void ApplyHairPreset(int index)
        {
            if (index < 0 || index >= hairPresetColors.Length) return;
            ApplyHairColor(hairPresetColors[index]);
            SetHairSliders(hairPresetColors[index]);
            UpdateHairPreview();
        }

        // === Previews ===
        private void UpdateHatPreview() { if (_hatBtnImage) _hatBtnImage.color = GetHatSliderColor(); }
        private void UpdateHairPreview() { if (_hairBtnImage) _hairBtnImage.color = GetHairSliderColor(); }

        private void ApplyHatColor(Color c) => ApplyColor(hatMaterial, c);
        private void ApplyHairColor(Color c) => ApplyColor(hairMaterial, c);

        private void ApplyColor(Material mat, Color color)
        {
            if (!mat) return;
            if (mat.HasProperty("_Color")) mat.color = color;
            else if (mat.HasProperty("_Diffuse")) mat.SetColor("_Diffuse", color);
        }
    }
}

