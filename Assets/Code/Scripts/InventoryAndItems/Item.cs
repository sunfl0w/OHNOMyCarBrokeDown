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

    public void Use() {
        ItemData data = ItemDatabase.Instance.GetItemData(itemName);
        if (data == null) {
            return;
        }

        if (data.GetItemCategory() == ItemCategory.FOOD) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>().AddHealth(2);
        } else if (data.GetItemCategory() == ItemCategory.BATTERY) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerFlashlightManager>().batteryLife = 100.0f;
        }
    }
}