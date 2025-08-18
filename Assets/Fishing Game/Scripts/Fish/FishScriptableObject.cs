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
    public Vector2 MinMaxSizeKg;

    public string SpeciesName;
    public int FishCatchDifficulty;
    public string FishBio;
    public string LocationsFound;
    public string TimeOfDayFound;
}
