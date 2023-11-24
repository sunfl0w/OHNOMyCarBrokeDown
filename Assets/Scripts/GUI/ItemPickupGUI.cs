using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ItemPickupGUI : MonoBehaviour {
    public GameObject containerGameObject;
    public TextMeshProUGUI textGUI;

    private static ItemPickupGUI instance;
    public static ItemPickupGUI Instance { get { return instance; } }

    private Item currentItem = null;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
        Hide();
    }

    public void Show(Item item) {
        // Do not repeatedly activate GUI for a single item
        if(currentItem == null || currentItem.itemData.itemName != item.itemData.itemName) {
            Debug.Log("Show item pickup GUI");
            currentItem = item;
            containerGameObject.SetActive(true);
            textGUI.text = "Pickup [E] " + currentItem.itemData.itemName;
        }
    }

    public void Hide() {
        containerGameObject.SetActive(false);
        textGUI.text = String.Empty;
        currentItem = null;
    }
}
