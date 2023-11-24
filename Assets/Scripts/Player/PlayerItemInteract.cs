using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemInteract : MonoBehaviour {
    private Item closestInteractableItem = null;

    private void Update() {
        closestInteractableItem = GetClosesInteractableItem();

        if (closestInteractableItem != null) {
            // Show item pickup GUI
            ItemPickupGUI.Instance.Show(closestInteractableItem);

            if (Input.GetKeyDown(KeyCode.E)) { // Pickup item
                closestInteractableItem.AddToPlayerInventroy(); // Add item to player inventory

                closestInteractableItem.Destroy(); // Remove item from scene

                // Show item inspection GUI for a short amount of time
                ItemInspectGUI.Instance.Show(closestInteractableItem.itemData);
                //StartCoroutine(ItemInteractGUI.Instance.HideCoroutine());
            }
        } else {

            // Hide item pickup GUI
            ItemPickupGUI.Instance.Hide();
        }
    }

    public Item GetClosesInteractableItem() {
        List<Item> interactableItems = new List<Item>();
        float interactRange = 1f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray) {
            if (collider.TryGetComponent(out Item item)) {
                interactableItems.Add(item);
            }
        }

        Item closestItem = null;
        foreach (Item item in interactableItems) {
            if (closestItem == null) {
                closestItem = item;
            } else {
                if (Vector3.Distance(transform.position, item.transform.position) < Vector3.Distance(transform.position, closestItem.transform.position)) {
                    closestItem = item;
                }
            }
        }
        return closestItem;
    }
}
