using System;
using UnityEngine;

[Serializable]
public class ItemSaveState {
    public string name = String.Empty;
    public bool collected = false;
}

public class Item : MonoBehaviour, IInteractable {
    public ItemData itemData;
    public InteractableData interactableData;
    public String uniqueIdentifier = String.Empty;

    private ItemSaveState saveState = new ItemSaveState();

    public void Start() {
        saveState.name = uniqueIdentifier;
        if (saveState.collected) {
            this.gameObject.SetActive(false);
        }
    }

    public void Interact() {
        // Update global save state when interacting with item
        saveState.collected = true;
        SavestateManager.Instance.UpdateItem(saveState);

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

    public bool CanInteract() {
        return true;
    }

    public void UpdateSaveState(ItemSaveState saveState) {
        this.saveState = saveState;
    }
}
