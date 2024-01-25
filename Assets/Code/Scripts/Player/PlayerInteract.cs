using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour {
    private UnifiedGUI unifiedGUI;

    private void Start() {
        unifiedGUI = GameObject.FindGameObjectWithTag("UnifiedGUI").GetComponent<UnifiedGUI>();
    }

    private void Update() {
        IInteractable closestInteractable = GetClosestInteractable();

        if (closestInteractable != null) {
            IInteractable currentInteractable = InteractGUI.Instance.GetCurrentInteractable();
            if (currentInteractable != null && currentInteractable.GetData().name != closestInteractable.GetData().name) {
                Debug.Log("Interactable in range");
                InteractGUI.Instance.Show(closestInteractable);
            } else if(!unifiedGUI.IsAnyGUIVisible() && !InteractGUI.Instance.IsVisible()) {
                Debug.Log("Interactable in range");
                InteractGUI.Instance.Show(closestInteractable);
            }
        } else {
            InteractGUI.Instance.Hide();
        }

        if (InteractGUI.Instance.IsVisible() && Input.GetButtonDown("Interact")) {
            closestInteractable.Interact();
        }
    }

    public IInteractable GetClosestInteractable() {
        List<IInteractable> interactables = new List<IInteractable>();
        float interactRange = 1f;
        Collider[] colliderArray = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * 2.0f, interactRange, LayerMask.GetMask("Interactable"));
        foreach (Collider collider in colliderArray) {
            if (collider.TryGetComponent(out IInteractable interactable)) {
                if(interactable.CanInteract()) {
                    interactables.Add(interactable);
                }
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