using System;
using UnityEngine;

/// <summary>
/// Simple item data used to store data for items used in game.
/// This allows for a very simple item class that can be easily serialized and this scriptable
/// object to hold all relevant data not required for serialization.
/// </summary>
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
