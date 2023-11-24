using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    public List<(ItemData, uint)> items = new List<(ItemData, uint)>();

    private static PlayerInventory instance;
    public static PlayerInventory Instance { get { return instance; } }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    // Add an item to the player inventory
    public void AddItem(ItemData itemToAdd) {
        int itemExistsIndex = -1;

        // Check if a item with the same name is already in the inventory and increase item amount if it already is
        for (int i = 0; i < items.Count; i++) {
            (ItemData item, uint count) tuple = items[i];
            if (tuple.item.name == itemToAdd.itemName) {
                itemExistsIndex = i;
                tuple.count++;
                Debug.Log("Copy of existing item " + itemToAdd.name + " added to player inventory. New total is " + tuple.count + ".");
                break;
            }
        }
        // Add item if it is not already in the inventory
        if (itemExistsIndex < 0) {
            items.Add((itemToAdd, 1));
            Debug.Log("New item " + itemToAdd.name + " added to player inventory.");
        }
    }
}