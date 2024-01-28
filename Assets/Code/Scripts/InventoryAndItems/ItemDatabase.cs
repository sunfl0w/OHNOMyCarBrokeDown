using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The item database stores a list of all item data used in the game.
/// The item database is a singleton.
/// </summary>
public class ItemDatabase : MonoBehaviour {
    [SerializeField] private List<ItemData> itemData = new List<ItemData>();

    private static ItemDatabase instance;
    public static ItemDatabase Instance { get { return instance; } }

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    public ItemData GetItemData(string itemName) {
        return itemData.Find(a => a.GetName() == itemName);
    }
}