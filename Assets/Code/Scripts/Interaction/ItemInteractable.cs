using System;
using UnityEngine;

public class ItemInteractable : MonoBehaviour, IInteractable {
    public Item item;
    public InteractableData interactableData;
    public string uniqueIdentifier = String.Empty;

    private InteractableSaveState saveState = new InteractableSaveState();
    private PlayerInventory playerInventory = null;

    public void Start() {
        saveState.name = uniqueIdentifier;
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
    }

    public void Interact() {
        // Update global save state when interacting with item
        saveState.interacted = true;
        SavestateManager.Instance.UpdateInteractable(saveState);

        AddToPlayerInventroy(); // Add item to player inventory
        Destroy(this.gameObject); // Remove item from scene

        // Show item inspection GUI
        ItemInspectGUI.Instance.Show(item);
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
        Debug.Log("Player picked up a new item");
        playerInventory.GetInventory().AddItem(item);
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
