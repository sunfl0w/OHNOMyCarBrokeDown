using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player interact is normally attached to the player character game object.
/// The script enables player interaction with interactables in range
/// </summary>
public class PlayerInteract : MonoBehaviour {
    /// <summary>
    /// Reference to the unified gui.
    /// </summary>
    private UnifiedGUI unifiedGUI;

    /// <summary>
    /// Maximum interaction range.
    /// </summary>
    private static float MAX_INTERACT_RANGE = 1.0f;

    /// <summary>
    /// Y offset between player character position and character model center.
    /// </summary>
    private static float YOFFSET = 1.0f;

    private void Start() {
        unifiedGUI = GameObject.FindGameObjectWithTag("UnifiedGUI").GetComponent<UnifiedGUI>();
    }

    private void Update() {
        IInteractable closestInteractable = GetClosestInteractable(); // Search for interactable in range

        // Open and close interact gui based on player input and gui state
        if (closestInteractable != null) {
            IInteractable currentInteractable = InteractGUI.Instance.GetCurrentInteractable();
            if (currentInteractable != null && currentInteractable.GetData().name != closestInteractable.GetData().name) {
                Debug.Log("Interactable in range");
                InteractGUI.Instance.Show(closestInteractable);
            } else if (!unifiedGUI.IsAnyGUIVisible() && !InteractGUI.Instance.IsVisible()) {
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

    /// <summary>
    /// Returns the closest interactable in range.
    /// </summary>
    public IInteractable GetClosestInteractable() {
        List<IInteractable> interactables = new List<IInteractable>();
        Collider[] colliderArray = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * YOFFSET, MAX_INTERACT_RANGE, LayerMask.GetMask("Interactable"));
        foreach (Collider collider in colliderArray) {
            if (collider.TryGetComponent(out IInteractable interactable)) {
                if (interactable.CanInteract()) {
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