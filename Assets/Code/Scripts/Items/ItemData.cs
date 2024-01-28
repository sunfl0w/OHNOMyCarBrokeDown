using System;
using UnityEngine;

// Simple item data representation as a scriptable object]
[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject {
    [SerializeField] private string itemName = String.Empty;
    [SerializeField] private string interactText = String.Empty;
    [SerializeField] private ItemCategory category = ItemCategory.INVALID;
    [SerializeField] private GameObject prefab = null;

    public string GetName() {
        return itemName;
    }

    public string GetInteractText() {
        return interactText;
    }

    public ItemCategory GetItemCategory() {
        return category;
    }

    public GameObject GetPrefab() {
        return prefab;
    }

    public void Use() {
        if(category == ItemCategory.FOOD) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>().AddHealth(2);
        } else if (category == ItemCategory.BATTERY) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerFlashlightManager>().batteryLife = 100.0f;
        }
    }
}
