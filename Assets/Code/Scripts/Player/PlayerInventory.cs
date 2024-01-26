using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to store the player inventory and equipped item in game.
/// </summary>
public class InventoryData {
    public List<InventorySlot> slots = new List<InventorySlot>();
    public ItemData equippedItem = null;
}

/// <summary>
/// Class used to represent an inventory slot in game.
/// </summary>
public class InventorySlot {
    public ItemData itemData = null;
    public uint amount = 0;
}

/// <summary>
/// Class used to represent the player inventory and equipped item save state.
/// </summary>
[Serializable]
public class InventorySaveState {
    public List<InventorySaveStateSlot> slots = new List<InventorySaveStateSlot>();
    public string equippedItem;
}

/// <summary>
/// Class used to represent an inventory slot save state.
/// </summary>
[Serializable]
public class InventorySaveStateSlot {
    public string itemName;
    public uint amount;
}

/// <summary>
/// The player inventory defines the player character's inventory based on slots and an equipped item.
/// The player inventory is a singleton.
/// </summary>
public class PlayerInventory : MonoBehaviour {
    /// <summary>
    /// Inventory data containing slots and the equipped item.
    /// </summary>
    public InventoryData inventoryData = new InventoryData();

    /// <summary>
    /// A list of all items in the game.
    /// This is used to populate the player's inventory from a save state
    /// </summary>
    public List<ItemData> allItems = new List<ItemData>();

    private static PlayerInventory instance;
    public static PlayerInventory Instance { get { return instance; } }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    /// <summary>
    /// Returns an inventory save state based on the current player inventory.
    /// </summary>
    public InventorySaveState GetInventorySaveState() {
        InventorySaveState saveState = new InventorySaveState();
        foreach (InventorySlot slot in inventoryData.slots) {
            InventorySaveStateSlot saveStateSlot = new InventorySaveStateSlot();
            saveStateSlot.amount = slot.amount;
            saveStateSlot.itemName = slot.itemData.itemName;
            saveState.slots.Add(saveStateSlot);
        }

        if (inventoryData.equippedItem != null) {
            saveState.equippedItem = inventoryData.equippedItem.itemName;
        } else {
            saveState.equippedItem = "NULL";
        }
        return saveState;
    }

    /// <summary>
    /// Populates the player inventory based on a save state.
    /// </summary>
    public void SetInventorySaveState(InventorySaveState saveState) {
        foreach (InventorySaveStateSlot slot in saveState.slots) {
            InventorySlot invSlot = new InventorySlot();
            invSlot.amount = slot.amount;
            invSlot.itemData = GetItemDataByName(slot.itemName);
            if (invSlot.itemData != null) {
                inventoryData.slots.Add(invSlot);
            }
        }
        if (saveState.equippedItem != "" && saveState.equippedItem != "NULL") {
            inventoryData.equippedItem = GetItemDataByName(saveState.equippedItem);
        }
    }

    /// <summary>
    /// Returns item data with respect to the provided item name.
    /// </summary>
    private ItemData GetItemDataByName(string name) {
        foreach (ItemData data in allItems) {
            if (data.itemName == name) {
                return data;
            }
        }
        return null;
    }

    /// <summary>
    /// Adds an item to the player inventory.
    /// </summary>
    public void AddItem(ItemData itemToAdd) {
        int itemExistsIndex = -1;

        // Check if a item with the same name is already in the inventory and increase item amount if it already is
        for (int i = 0; i < inventoryData.slots.Count; i++) {
            InventorySlot slot = inventoryData.slots[i];
            if (slot.itemData.itemName == itemToAdd.itemName) {
                itemExistsIndex = i;
                slot.amount += 1;
                Debug.Log("Copy of existing item " + itemToAdd.name + " added to player inventory. New total is " + slot.amount + ".");
                break;
            }
        }
        // Add item if it is not already in the inventory
        if (itemExistsIndex < 0) {
            InventorySlot newItem = new InventorySlot();
            newItem.itemData = itemToAdd;
            newItem.amount = 1;
            inventoryData.slots.Add(newItem);
            Debug.Log("New item " + itemToAdd.name + " added to player inventory.");
        }
    }

    /// <summary>
    /// Removes an item from the player inventory.
    /// </summary>
    public void RemoveItem(ItemData itemToAdd) {
        for (int i = 0; i < inventoryData.slots.Count; i++) {
            InventorySlot slot = inventoryData.slots[i];
            if (slot.itemData.itemName == itemToAdd.itemName) {
                slot.amount -= 1;
                if (slot.amount <= 0) {
                    inventoryData.slots.RemoveAt(i);
                }
                Debug.Log("Copy of existing item " + itemToAdd.name + " removed from player inventory. New total is " + slot.amount + ".");
                break;
            }
        }
    }

    /// <summary>
    /// Returns true if a given item exists in the player inventory.
    /// </summary>
    public bool CheckItemExists(ItemData item) {
        if (item == null) {
            return false;
        }
        for (int i = 0; i < inventoryData.slots.Count; i++) {
            InventorySlot slot = inventoryData.slots[i];
            if (slot.itemData.itemName == item.itemName) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Equips an item.
    /// </summary>
    public void EquipItem(ItemData item) {
        if (CheckItemExists(item)) {
            inventoryData.equippedItem = item;
        }
    }

    /// <summary>
    /// Unequips an item.
    /// </summary>
    public void UnequipItem() {
        inventoryData.equippedItem = null;
    }

    /// <summary>
    /// Uses an item from the player inventory if possible.
    /// </summary>
    public void UseItem(ItemData itemToUse) {
        int itemIndex = -1;
        for (int i = 0; i < inventoryData.slots.Count; i++) {
            InventorySlot slot = inventoryData.slots[i];
            if (slot.itemData == itemToUse) {
                itemIndex = i;
                if (slot.amount > 1) {
                    slot.amount--;
                    Debug.Log("Used one " + itemToUse.itemName + ". Remaining count: " + slot.amount);
                } else {
                    inventoryData.slots.RemoveAt(itemIndex);
                    Debug.Log("Used the last " + itemToUse.itemName + ". Removed from inventory.");
                    RemoveItem(slot.itemData);
                }
                if (slot.itemData.itemName == "Battery") {
                    PlayerFlashlightManager flashlightManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerFlashlightManager>();
                    flashlightManager.batteryLife = 100.0f;

                }
                if (slot.itemData.itemName == "Soda") {
                    PlayerHealthManager playerHealthManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>();
                    playerHealthManager.AddHealth(2);

                }
                break;
            }
        }
    }

    /// <summary>
    /// Returns true if the currently equipped item is the flashlight.
    /// </summary>
    public bool IsFlashlightEquipped() {
        return inventoryData != null && inventoryData.equippedItem != null && inventoryData.equippedItem.name == "Flashlight";
    }
}
