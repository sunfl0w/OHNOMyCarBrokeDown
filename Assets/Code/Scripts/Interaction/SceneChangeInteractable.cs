using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class SceneChanceInteractable : MonoBehaviour, IInteractable
{
    public string targetSceneName;
    public string targetTransformName;
    public InteractableData data;
    public AudioSource audioSource;
    public ItemData requiredItem;
    public TextMeshProUGUI hintText;


    public void Interact()
    {
        if (PlayerInventory.Instance.CheckItemExists(requiredItem) || requiredItem == null)
        {
            if (data.interactSound != null)
            {
                audioSource.PlayOneShot(data.interactSound);
            }
            Debug.Log("Interactable. Switch to scene: " + targetSceneName);

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StartTransition();
            StartCoroutine(Transition());
        }
        else
        {
            if (requiredItem.itemName == "Key")
            {
                DialogueGUI.Instance.Show(new DialogueData("Of couese it's locked. How I wish that I had the key..."));
            }
            else
            {
                hintText.text = "The door is locked...";
                StartCoroutine(ClearHint());
            }


        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public InteractableData GetData()
    {
        return data;
    }

    IEnumerator Transition()
    {
        yield return new WaitForSeconds(1.0f);
        SavestateManager.Instance.StoreSaveState();
        SceneTransitionManager.Instance.LoadSceneWithNextPlayerTransform(targetSceneName, targetTransformName);
    }

    IEnumerator ClearHint()
    {
        yield return new WaitForSeconds(1.0f);
        hintText.text = "";
    }

    public bool CanInteract()
    {
        return true;
    }

    public void UpdateSaveState(InteractableSaveState saveState)
    {
    }

    public string GetUniqueIdentifier()
    {
        return String.Empty;
    }
}
