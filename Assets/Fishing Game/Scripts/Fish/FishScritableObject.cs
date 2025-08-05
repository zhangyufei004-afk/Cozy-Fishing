using UnityEngine;

/// <summary>
/// <para>
/// Fish Scritable Object.
/// </para>
/// <para>
/// Stores Details including: Texture, Min and Max Length, Species name.
/// </para>
/// </summary>
public class FishScritableObject : ScriptableObject
{
    public Sprite Texture;
    public Vector2 MinMaxLength;
    public string SpeciesName;

    // The difficulty to catch this fish
    // lower the number the easier
    // Note: Delete if not needed here 5/08/2025 - Brayden 
    public int FishCatchDifficulty;
}
