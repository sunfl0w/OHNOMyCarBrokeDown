using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<(ItemData, uint)> items = new List<(ItemData, uint)>();
    public ItemData equipment = null;

    private static PlayerInventory instance;
    public static PlayerInventory Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Add an item to the player inventory
    public void AddItem(ItemData itemToAdd)
    {
        int itemExistsIndex = -1;

        // Check if a item with the same name is already in the inventory and increase item amount if it already is
        for (int i = 0; i < items.Count; i++)
        {
            (ItemData item, uint count) tuple = items[i];
            if (tuple.item.itemName == itemToAdd.itemName)
            {
                itemExistsIndex = i;
                items[i] = (tuple.item, tuple.count + 1);
                tuple.count++;
                Debug.Log("Copy of existing item " + itemToAdd.name + " added to player inventory. New total is " + tuple.count + ".");
                break;
            }
        }
        // Add item if it is not already in the inventory
        if (itemExistsIndex < 0)
        {
            items.Add((itemToAdd, 1));
            Debug.Log("New item " + itemToAdd.name + " added to player inventory.");
        }
    }

    public void printItems()
    {
        if (items.Count == 0)
        {
            Debug.Log("Inventory: empty");
        }
        else
        {
            Debug.Log("Inventory:");

            foreach ((ItemData item, uint count) tuple in items)
            {
                Debug.Log(tuple.item.itemName + " - Count: " + tuple.count);
            }
        }
    }

    public bool itemExists(ItemData item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            (ItemData item, uint count) tuple = items[i];
            if (tuple.item.itemName == item.itemName)
            {
                return true;
            }
        }

        return false;
    }

    public void EquipItem(ItemData item)
    {
        if (itemExists(item))
        {
            equipment = item;
            if (item.itemName == "Flashlight")
            {
                //FlashlightManager.Instance.EquipFlashlight();
            }

        }
    }

    public void UnequipItem()
    {
        equipment = null;
        //FlashlightManager.Instance.UnequipFlashlight();

    }

    public void UseItem(ItemData itemToUse)
    {
        int itemIndex = -1;

        for (int i = 0; i < items.Count; i++)
        {
            (ItemData item, uint count) tuple = items[i];
            if (tuple.item == itemToUse)
            {
                itemIndex = i;
                if (tuple.count > 1)
                {
                    items[i] = (tuple.item, tuple.count - 1);
                    tuple.count--;
                    Debug.Log("Used one " + itemToUse.itemName + ". Remaining count: " + tuple.count);
                }
                else
                {
                    items.RemoveAt(itemIndex);
                    Debug.Log("Used the last " + itemToUse.itemName + ". Removed from inventory.");
                }
                if (tuple.item.itemName == "Battery")
                {
                    //FlashlightManager.Instance.ChangeBattery();
                }
                break;
            }
        }
    }

    public ItemData GetEquippedItem() {
        return equipment;
    }

    public bool IsFlashlightEquipped() {
        return equipment != null && equipment.name == "Flashlight"; 
    }
}