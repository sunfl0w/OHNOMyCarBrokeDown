using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryData {
    public List<InventorySlot> slots = new List<InventorySlot>();
    public ItemData equippedItem = null;
}

public class InventorySlot {
    public ItemData itemData = null;
    public uint amount = 0;
}

[Serializable]
public class InventorySaveState {
    public List<InventorySaveStateSlot> slots = new List<InventorySaveStateSlot>();
    public string equippedItem;
}

[Serializable]
public class InventorySaveStateSlot {
    public string itemName;
    public uint amount;
}

public class PlayerInventory : MonoBehaviour {
    public InventoryData inventoryData = new InventoryData();
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

    void Start() {
    }

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

    public void SetInventorySaveState(InventorySaveState saveState) {
        foreach (InventorySaveStateSlot slot in saveState.slots) {
            InventorySlot invSlot = new InventorySlot();
            invSlot.amount = slot.amount;
            invSlot.itemData = GetitemDataByName(slot.itemName);
            if (invSlot.itemData != null) {
                inventoryData.slots.Add(invSlot);
            }
        }
        if (saveState.equippedItem != "" && saveState.equippedItem != "NULL") {
            inventoryData.equippedItem = GetitemDataByName(saveState.equippedItem);
        }
    }

    private ItemData GetitemDataByName(string name) {
        foreach (ItemData data in allItems) {
            if (data.itemName == name) {
                return data;
            }
        }
        return null;
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

    public ItemData GetEquippedItem() {
        return inventoryData.equippedItem;
    }

    public bool IsFlashlightEquipped() {
        return inventoryData != null && inventoryData.equippedItem != null && inventoryData.equippedItem.name == "Flashlight";
    }
}