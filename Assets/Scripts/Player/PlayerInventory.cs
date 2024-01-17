using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryData {
    public List<InventorySlot> slots = new List<InventorySlot>();
    public ItemData equippedItem = null;
}

[Serializable]
public class InventorySlot {
    public ItemData itemData = null;
    public uint amount = 0;
}

public class PlayerInventory : MonoBehaviour {
    public InventoryData inventoryData = new InventoryData();

    private static PlayerInventory instance;
    public static PlayerInventory Instance { get { return instance; } }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    void Start() {
    }

    public InventoryData GetInventoryData() {
        return inventoryData;
    }

    public void SetInventoryData(InventoryData inventoryData) {
        this.inventoryData = inventoryData;
    }

    // Add an item to the player inventory
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

    // Remove an item to the player inventory
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

    public void printItems() {
        if (inventoryData.slots.Count == 0) {
            Debug.Log("Inventory: empty");
        } else {
            Debug.Log("Inventory:");

            foreach (InventorySlot slot in inventoryData.slots) {
                Debug.Log(slot.itemData.itemName + " - Count: " + slot.amount);
            }
        }
    }

    public bool itemExists(ItemData item) {
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

    public void EquipItem(ItemData item) {
        if (itemExists(item)) {
            inventoryData.equippedItem = item;
        }
    }

    public void UnequipItem() {
        inventoryData.equippedItem = null;
    }

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
                }
                if (slot.itemData.itemName == "Battery") {
                    PlayerFlashlightManager flashlightManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerFlashlightManager>();
                    flashlightManager.batteryLife = 100.0f;
                    RemoveItem(slot.itemData);
                }
                break;
            }
        }
    }

    public ItemData GetEquippedItem() {
        return inventoryData.equippedItem;
    }

    public bool IsFlashlightEquipped() {
        return inventoryData != null && inventoryData.equippedItem != null && inventoryData.equippedItem.name == "Flashlight";
    }
}