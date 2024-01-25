using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using TMPro;
using System;
using System.Collections.Generic;

public class CarInteractable : MonoBehaviour, IInteractable
{
    public string targetSceneName;
    public string targetTransformName;
    public InteractableData data;
    public AudioSource audioSource;
    public List<ItemData> requiredItems;

    public TextMeshProUGUI hintText;
    public PlayableDirector playableDirector;

    private void Start()
    {
        // Subscribe to the played event
        playableDirector.stopped += OnTimelineFinished;
    }

    // Event handler for the timeline completion
    private void OnTimelineFinished(PlayableDirector aDirector)
    {
        if (aDirector == playableDirector)
        {
            // Unsubscribe to prevent multiple calls
            playableDirector.stopped -= OnTimelineFinished;

            // Load the next scene after the timeline has finished playing
            SavestateManager.Instance.ClearSaveState();
            SceneTransitionManager.Instance.LoadScene("MainMenuScene");
        }
    }


    public void Interact()
    {
        if (!PlayerInventory.Instance.itemExists(requiredItems[2]))
        {
            InteractGUI.Instance.Hide();
            DialogueGUI.Instance.Show(new DialogueData("I think my car ran out of gas..."));
        }
        else if (!PlayerInventory.Instance.itemExists(requiredItems[0]))
        {
            InteractGUI.Instance.Hide();
            DialogueGUI.Instance.Show(new DialogueData("The car wont start but at least it is fueled now. I think my car ran out of SubBlue..."));
        }
        else if (!PlayerInventory.Instance.itemExists(requiredItems[4]))
        {
            InteractGUI.Instance.Hide();
            DialogueGUI.Instance.Show(new DialogueData("Still not working. I need some tools to repair the car..."));
        }
        else if (!PlayerInventory.Instance.itemExists(requiredItems[1]))
        {
            InteractGUI.Instance.Hide();
            DialogueGUI.Instance.Show(new DialogueData("Ew, why does my car smell like garbage? I need some air refreshener..."));
        }
        else if (!PlayerInventory.Instance.itemExists(requiredItems[3]))
        {
            InteractGUI.Instance.Hide();
            DialogueGUI.Instance.Show(new DialogueData("I need some good music to start driving again..."));
        }
        else
        {
            if (data.interactSound != null)
            {
                audioSource.PlayOneShot(data.interactSound);
            }
            Debug.Log("Load EndingScene");
            hintText.text = "";
            HealthIndicatorGUI.Instance.Hide();
            playableDirector.Play();
            //SceneTransitionManager.Instance.LoadScene("MainMenuScene");
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

    IEnumerator ClearHint()
    {
        yield return new WaitForSeconds(2.0f);
        InteractGUI.Instance.Hide();
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
