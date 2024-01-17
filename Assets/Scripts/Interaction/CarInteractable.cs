using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class CarInteractable : MonoBehaviour, IInteractable {
    public string targetSceneName;
    public string targetTransformName;
    public InteractableData data;
    public AudioSource audioSource;
    public ItemData requiredItem1;
    public ItemData requiredItem2;

    public TextMeshProUGUI hintText;


    public void Interact() {
        if (!PlayerInventory.Instance.itemExists(requiredItem1)) {
            hintText.text = "I think my car ran out of gas...";
            StartCoroutine(ClearHint());
        } else if (!PlayerInventory.Instance.itemExists(requiredItem2)) {
            hintText.text = "Ew, why does my car smell like garbage?";
            StartCoroutine(ClearHint());
        } else {
            if (data.interactSound != null) {
                audioSource.PlayOneShot(data.interactSound);
            }
            Debug.Log("Load EndingScene");
            SceneTransitionManager.Instance.LoadScene("MainMenuScene");
        }
    }

    public Transform GetTransform() {
        return transform;
    }

    public InteractableData GetData() {
        return data;
    }

    IEnumerator ClearHint() {
        yield return new WaitForSeconds(2.0f);
        InteractGUI.Instance.Hide();
    }

    public bool CanInteract() {
        return true;
    }

    public void UpdateSaveState(InteractableSaveState saveState) {
    }

    public string GetUniqueIdentifier() {
        return String.Empty;
    }
}