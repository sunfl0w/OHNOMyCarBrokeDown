using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CarTrunkInteractable : MonoBehaviour, IInteractable {
    public InteractableData data;
    public AudioSource audioSource;
    public ItemData flashlightData;
    public TextMeshProUGUI hintText;

    public void Interact() {
        if (data.interactSound != null) {
            audioSource.PlayOneShot(data.interactSound);
        }
        Debug.Log("Interacting with car trunk and adding flashlight to player inventory.");
        PlayerInventory.Instance.AddItem(flashlightData);
    }

    public Transform GetTransform() {
        return transform;
    }

    public InteractableData GetData() {
        return data;
    }
}
