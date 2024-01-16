using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CarInteractable : MonoBehaviour, IInteractable
{
    public string targetSceneName;
    public string targetTransformName;
    public InteractableData data;
    public AudioSource audioSource;
    public ItemData requiredItem1;
    public ItemData requiredItem2;

    public TextMeshProUGUI hintText;


    public void Interact()
    {
        if (!PlayerInventory.Instance.itemExists(requiredItem1))
        {
            hintText.text = "I think my car ran out of gas...";
            StartCoroutine(ClearHint());
        }
        else if (!PlayerInventory.Instance.itemExists(requiredItem2))
        {
            hintText.text = "Ew, why does my car smell like garbage?";
            StartCoroutine(ClearHint());
        }
        else
        {
            if (data.interactSound != null)
            {
                audioSource.PlayOneShot(data.interactSound);
            }
            Debug.Log("Load EndingScene");
            //SceneTransitionManager.Instance.SetNextTransformByName(targetTransformName);
            //SavestateManager.Instance.StoreSaveState();
            //StartCoroutine(Transition());
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
