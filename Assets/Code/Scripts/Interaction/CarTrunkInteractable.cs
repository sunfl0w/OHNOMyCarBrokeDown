using UnityEngine;
using System;

public class CarTrunkInteractable : MonoBehaviour, IInteractable {
    public InteractableData data;
    public AudioSource audioSource;
    public string uniqueIdentifier = String.Empty;

    private InteractableSaveState saveState = new InteractableSaveState();

    public void Start() {
        saveState.name = uniqueIdentifier;
        if (saveState.interacted) {
            this.gameObject.SetActive(false);
        }
    }

    public void Interact() {
        if (data.interactSound != null) {
            audioSource.PlayOneShot(data.interactSound);
        }
        Debug.Log("Interacting with car trunk and adding flashlight to player inventory.");
        PlayerInventory playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        playerInventory.GetInventory().AddItem(new Item("Flashlight"));
        playerInventory.GetInventory().AddItem(new Item("Battery"));
        saveState.interacted = true;
        SavestateManager.Instance.UpdateInteractable(saveState);
    }

    public Transform GetTransform() {
        return transform;
    }

    public InteractableData GetData() {
        return data;
    }

    public bool CanInteract() {
        return !saveState.interacted;
    }

    public void UpdateSaveState(InteractableSaveState saveState) {
        this.saveState = saveState;
    }

    public string GetUniqueIdentifier() {
        return uniqueIdentifier;
    }
}
