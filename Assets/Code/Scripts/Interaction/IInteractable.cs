using UnityEngine;
using System;

public interface IInteractable {
    public void Interact();
    public Transform GetTransform();
    public InteractableData GetData();
    public bool CanInteract();
    public void UpdateSaveState(InteractableSaveState saveState);
    public string GetUniqueIdentifier();
}

[Serializable]
public class InteractableSaveState {
    public string name = String.Empty;
    public bool interacted = false;
}