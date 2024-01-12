using UnityEngine;

public enum ItemCategory {
    Weapon,
    Resource,
    CarPart,
    Normal
}

// Simple item data representation as a scriptable object
[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject {
    public string itemName;
    public string interactText;
    public GameObject prefab;
    public ItemCategory category;

    public ItemData(string itemName, string interactText, GameObject prefab, ItemCategory category) {
        this.itemName = itemName;
        this.interactText = interactText;
        this.prefab = prefab;
        this.category = category;
    }
}
