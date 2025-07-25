using UnityEngine;

public class Fish
{
    private FishScritableObject _fishBase;

    private float _length;

    public Fish(FishScritableObject _newFishBase)
    {
        this._fishBase = _newFishBase;
        this._length = Random.Range(this._fishBase.MinMaxLength.x, this._fishBase.MinMaxLength.y);
    }
    public FishScritableObject GetFishBase()
    {
        return this._fishBase;
    }
    public float GetLength()
    {
        return this._length;
    }
}
