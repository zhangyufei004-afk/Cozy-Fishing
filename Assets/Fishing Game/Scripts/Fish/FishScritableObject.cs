using UnityEngine;

/// <summary>
/// <para>
/// Fish Scriptable Object.
/// </para>
/// <para>
/// Stores Details including: Texture, Min and Max Length, Species name.
/// </para>
/// </summary>
[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing Game/Fish Data")]
public class FishScritableObject : ScriptableObject
{
    public Sprite Texture;
    public Vector2 MinMaxLength;
    public string SpeciesName;
}
