using UnityEngine;

/// <summary>
/// Controls a rotating and pulsing visual indicator effect for Grapple Zones or interactable areas.
/// </summary>
public class VisualIndicatorEffect : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private Renderer indicatorRenderer;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseIntensity = 2f;
    
    private Material _material;
    private Color _baseEmission;

    private void Start()
    {
        if (indicatorRenderer == null)
            indicatorRenderer = GetComponent<Renderer>();

        _material = indicatorRenderer.material;
        _baseEmission = _material.GetColor("_EmissionColor");
    }

    private void Update()
    {
        // Rotate slowly
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));

        // Pulse emission
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        _material.SetColor("_EmissionColor", _baseEmission * (0.5f + pulse * pulseIntensity));
    }
}
