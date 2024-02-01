using System;
using UnityEngine;

[Serializable]
public enum ItemCategory {
    FLASHLIGHT,
    FOOD,
    KEY,
    BATTERY,
    CAR_PART,
    INVALID
}

/// <summary>
/// Simple item representation
/// </summary>
[Serializable]
public class Item {
    [SerializeField] private string itemName = String.Empty;

    public Item() { }

    public Item(string name) {
        itemName = name;
    }

    public string GetName() {
        return itemName;
    }

    public string GetInteractText() {
        ItemData data = ItemDatabase.Instance.GetItemData(itemName);
        if (data == null) {
            return null;
        }
        return data.GetInteractText();
    }

    public ItemCategory GetItemCategory() {
        ItemData data = ItemDatabase.Instance.GetItemData(itemName);
        if (data == null) {
            return ItemCategory.INVALID;
        }
        return data.GetItemCategory();
    }

    public GameObject GetPrefab() {
        ItemData data = ItemDatabase.Instance.GetItemData(itemName);
        if (data == null) {
            return null;
        }
        return data.GetPrefab();
    }

    public bool Equipable() {
        ItemData data = ItemDatabase.Instance.GetItemData(itemName);
        if (data == null) {
            return false;
        }
        
        return data.GetItemCategory() == ItemCategory.FLASHLIGHT;
    }

    public bool Usable() {
        ItemData data = ItemDatabase.Instance.GetItemData(itemName);
        if (data == null) {
            return false;
        }

        if (data.GetItemCategory() == ItemCategory.FOOD) {
            return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>().CanHeal();
        } else if (data.GetItemCategory() == ItemCategory.BATTERY) {
            return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerFlashlightManager>().CanUseBattery();
        }
        return false;
    }

    public void Use() {
        ItemData data = ItemDatabase.Instance.GetItemData(itemName);
        if (data == null) {
            return;
        }

        if (data.GetItemCategory() == ItemCategory.FOOD) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>().AddHealth(2);
        } else if (data.GetItemCategory() == ItemCategory.BATTERY) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerFlashlightManager>().UseBattery();
        }
    }
}