using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour {
    private void Update() {
        IInteractable closestInteractable = GetClosestInteractable();

        if (closestInteractable != null && !InventoryGUI.Instance.IsVisible()) {
            // Show interact GUI
            bool test = InteractGUI.Instance.IsVisible();
            if (!InteractGUI.Instance.IsVisible()) {
                Debug.Log("Interactable in range");
                InteractGUI.Instance.Show(closestInteractable);
            }

            if (Input.GetButtonDown("Interact")) {
                closestInteractable.Interact();
            }
        } else {
            // Hide interact GUI
            InteractGUI.Instance.Hide();
        }
    }

    public IInteractable GetClosestInteractable() {
        List<IInteractable> interactables = new List<IInteractable>();
        float interactRange = 1f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange, LayerMask.GetMask("Interactable"));
        foreach (Collider collider in colliderArray) {
            if (collider.TryGetComponent(out IInteractable interactable)) {
                interactables.Add(interactable);
            }
        }

        IInteractable closestInteractable = null;
        foreach (IInteractable interactable in interactables) {
            if (closestInteractable == null) {
                closestInteractable = interactable;
            } else {
                if (Vector3.Distance(transform.position, interactable.GetTransform().position) < Vector3.Distance(transform.position, closestInteractable.GetTransform().position)) {
                    closestInteractable = interactable;
                }
            }
        }
        return closestInteractable;
    }
}