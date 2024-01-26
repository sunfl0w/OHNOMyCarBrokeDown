using System;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable {
    public ItemData itemData;
    public InteractableData interactableData;
    public string uniqueIdentifier = String.Empty;

    private InteractableSaveState saveState = new InteractableSaveState();

    public void Start() {
        saveState.name = uniqueIdentifier;
    }

    public void Interact() {
        // Update global save state when interacting with item
        saveState.interacted = true;
        SavestateManager.Instance.UpdateInteractable(saveState);

        AddToPlayerInventroy(); // Add item to player inventory
        Destroy(this.gameObject); // Remove item from scene

        // Show item inspection GUI
        ItemInspectGUI.Instance.Show(itemData);
    }

    public void Update() {
        if (saveState.interacted) {
            Destroy(this.gameObject);
            //this.GetComponent<MeshRenderer>().enabled = false;
            //this.enabled = false;
        }
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

    public bool CanInteract() {
        return true;
    }

    public void UpdateSaveState(InteractableSaveState saveState) {
        this.saveState = saveState;
    }

    public string GetUniqueIdentifier() {
        return uniqueIdentifier;
    }
}
