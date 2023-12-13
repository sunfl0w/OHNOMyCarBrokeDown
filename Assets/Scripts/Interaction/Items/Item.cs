using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable {
    public ItemData itemData;
    public InteractableData interactableData;

    public void Interact() {
        AddToPlayerInventroy(); // Add item to player inventory
        Destroy(this.gameObject); // Remove item from scene

        // Show item inspection GUI
        ItemInspectGUI.Instance.Show(itemData);
    }

    public Transform GetTransform() {
        return transform;
    }

    public void AddToPlayerInventroy() {
        Debug.Log("Player picked up a new item.");
        PlayerInventory.Instance.AddItem(itemData);
    }

    public InteractableData GetData() {
        return interactableData;
    }
}
