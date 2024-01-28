using System;
using UnityEngine;

/// <summary>
/// The player inventory defines the player character's inventory based on slots and an equipped item.
/// </summary>
[Serializable]
public class PlayerInventory : MonoBehaviour {
    [SerializeField] private Inventory inventory = new Inventory();

    public Inventory GetInventory() {
        return inventory;
    }

    public void SetInventory(Inventory inventory) {
        this.inventory = inventory;
    }
}
