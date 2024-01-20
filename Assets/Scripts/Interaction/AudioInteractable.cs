using UnityEngine;
using System;

public class AudioInteractable : MonoBehaviour, IInteractable {
    public InteractableData data;
    public string uniqueIdentifier = String.Empty;

    private InteractableSaveState saveState = new InteractableSaveState();
    private AudioSource audioSource;

    public void Start() {
        saveState.name = uniqueIdentifier;
        if (saveState.interacted) {
            this.gameObject.SetActive(false);
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact() {
        if (data.interactSound != null) {
            audioSource.PlayOneShot(data.interactSound);
        }
        saveState.interacted = true;
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
