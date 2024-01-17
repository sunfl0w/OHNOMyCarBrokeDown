using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneChanceInteractable : MonoBehaviour, IInteractable {
    public string targetSceneName;
    public string targetTransformName;
    public InteractableData data;
    public AudioSource audioSource;
    public ItemData requiredItem;
    public TextMeshProUGUI hintText;


    public void Interact() {
        if (PlayerInventory.Instance.itemExists(requiredItem) || requiredItem == null) {
            if (data.interactSound != null) {
                audioSource.PlayOneShot(data.interactSound);
            }
            Debug.Log("Interactable. Switch to scene: " + targetSceneName);
            SceneTransitionManager.Instance.SetNextTransformByName(targetTransformName);
            SavestateManager.Instance.StoreSaveState();
            StartCoroutine(Transition());
        } else {
            hintText.text = "The door is locked...";
            StartCoroutine(ClearHint());
        }
    }

    public Transform GetTransform() {
        return transform;
    }

    public InteractableData GetData() {
        return data;
    }

    IEnumerator Transition() {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
    }

    IEnumerator ClearHint() {
        yield return new WaitForSeconds(1.0f);
        hintText.text = "";
    }
}
