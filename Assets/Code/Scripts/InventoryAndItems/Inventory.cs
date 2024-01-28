using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple slot class containing an item and the amount of it contained in the slot.
/// </summary>
[Serializable]
public class Slot {
    [SerializeField] private Item item = null;
    [SerializeField] private uint amount = 0;

    public Slot() { }

    public Slot(Item item) {
        this.item = item;
        amount = 1;
    }

    public Item GetItem() {
        return item;
    }

    public uint GetAmount() {
        return amount;
    }

    public void AddItem() {
        amount += 1;
    }

    public void UseItem() {
        item.Use();
        amount -= 1;
    }
}

/// <summary>
/// Simple inventory class defining item slots and an equipped item.
/// </summary>
[Serializable]
public class Inventory {
    [SerializeField] private List<Slot> slots = new List<Slot>();
    [SerializeField] private Item equippedItem = null;

    public Inventory() { }

    public Item GetItemBySlotIndex(int slotIndex) {
        return slots[slotIndex].GetItem();
    }

    public Slot GetSlotByIndex(int slotIndex) {
        return slots[slotIndex];
    }

    public Item GetEquippedItem() {
        return equippedItem;
    }

    public bool ItemExists(string itemName) {
        return slots.Find(a => a.GetItem().GetName() == itemName) != null;
    }

    public bool IsEmpty() {
        return slots.Count <= 0;
    }

    public int GetSlotCount() {
        return slots.Count;
    }

    public void AddItem(Item item) {
        if (item == null) {
            return;
        }

        Slot slot = slots.Find(a => a.GetItem().GetName() == item.GetName());
        if (slot != null) { // Increase amount
            slot.AddItem();
        } else { // Add new slot
            slots.Add(new Slot(item));
        }
    }

    public void UseItem(string itemName) {
        Slot slot = slots.Find(a => a.GetItem().GetName() == itemName);
        if (slot != null) {
            slot.UseItem();
            if (slot.GetAmount() <= 0) {
                slots.Remove(slot);
                if (itemName == equippedItem.GetName()) {
                    equippedItem = null;
                }
            }
        }
    }

    public void EquipItem(string itemName) {
        Slot slot = slots.Find(a => a.GetItem().GetName() == itemName);
        if (slot != null) {
            equippedItem = slot.GetItem();
        }
    }

    public void UnequipItem() {
        equippedItem = null;
    }
}