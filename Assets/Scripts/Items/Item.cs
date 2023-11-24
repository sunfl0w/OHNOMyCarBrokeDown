using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public ItemData itemData;

    public void AddToPlayerInventroy() {
        Debug.Log("Player picked up a new item.");
        PlayerInventory.Instance.AddItem(itemData);
    }

    public void Destroy() {
        Destroy(this.gameObject);
    }
}
