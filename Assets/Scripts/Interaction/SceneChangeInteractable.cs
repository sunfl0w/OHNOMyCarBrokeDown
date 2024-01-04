using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
        if (PlayerInventory.Instance.itemExists(requiredItem))
        {
            audioSource.PlayOneShot(data.interactSound);
            Debug.Log("Interactable. Switch to scene: " + targetSceneName);
            SceneTransitionManager.Instance.SetNextTransformByName(targetTransformName);
            StartCoroutine(Transition());
        }
        else
        {
            hintText.text = "The door is locked...\n(Hint: Press [I] to toggle the inventory)";
            StartCoroutine(ClearHint());

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
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
    }

    IEnumerator ClearHint()
    {
        yield return new WaitForSeconds(1.0f);
        hintText.text = "";
    }
}
