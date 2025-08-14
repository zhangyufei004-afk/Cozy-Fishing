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
public class FishScriptableObject : ScriptableObject
{
    public Sprite Texture;
    public string Id;
    public Vector2 MinMaxLength;
    public string SpeciesName;

    // The difficulty to catch this fish
    // lower the number the easier
    // Note: Delete if not needed here 5/08/2025 - Brayden 
    public int FishCatchDifficulty;
}
