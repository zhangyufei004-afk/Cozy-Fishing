using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventory System - Stores caught fish.
/// </summary>
public class InventorySystem : MonoBehaviour
{
    private List<Fish> _fishInventory = new List<Fish>();

    /// <summary>
    /// Adds a new fish to the inventory.
    /// </summary>
    /// <param name="newFish">Fish instance</param>
    public void AddFish(Fish newFish)
    {
        _fishInventory.Add(newFish);
    }

    /// <summary>
    /// Returns the full list of caught fish.
    /// </summary>
    public List<Fish> GetFishInventory()
    {
        return _fishInventory;
    }

    /// <summary>
    /// Clears all caught fish.
    /// </summary>
    public void ClearInventory()
    {
        _fishInventory.Clear();
    }
}
