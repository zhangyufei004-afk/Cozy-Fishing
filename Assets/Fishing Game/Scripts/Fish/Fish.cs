using UnityEngine;

/// <summary>
/// <para>
/// Fish Runtime Object.
/// </para>
/// <para>
/// Stores Details including: Fish SO Base, Length.
/// </para>
/// </summary>
public class Fish
{
    private FishScritableObject _fishBase;

    private float _length;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="newFishBase"> Fish scriptable object </param>
    public Fish(FishScritableObject newFishBase)
    {
        this._fishBase = newFishBase;
        this._length = Random.Range(this._fishBase.MinMaxLength.x, this._fishBase.MinMaxLength.y);
    }

    /// <summary>
    /// Gets Fish Scriptable Object.
    /// </summary>
    /// <returns> Fish Scriptable Object </returns>
    public FishScritableObject GetFishBase()
    {
        return this._fishBase;
    }

    /// <summary>
    /// Gets Fish Length.
    /// </summary>
    /// <returns> Fish length as a float </returns>
    public float GetLength()
    {
        return this._length;
    }
}
