using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MainMenu : MonoBehaviour
{

    public TextMeshProUGUI loadGame;
    public TextMeshProUGUI newGame;
    public TextMeshProUGUI credits;
    public TextMeshProUGUI quit;
    public GameObject buttons;

    public GameObject creditsContainer;

    private int selectedIndex = 0;
    private bool canLoad = false;

    void Awake()
    {
        string currentSceneName = SavestateManager.Instance.GetCurrentSaveState().playerSaveState.currentSceneName;
        if (currentSceneName != String.Empty)
        {
            canLoad = true;
        }
        else
        {
            canLoad = false;
            loadGame.color = Color.grey;
        }
    }
    void Start()
    {
        ChangeSelection(0);
    }

    void Update()
    {
        if (buttons.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                ChangeSelection(-1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                ChangeSelection(1);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                HandleSelection();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ToggleCredits();
            }
        }

    }

    void ChangeSelection(int direction)
    {
        selectedIndex = (selectedIndex + direction + 4) % 4;
        if (!canLoad && selectedIndex == 0) { selectedIndex = (direction == 1) ? 1 : 3; }

        if (canLoad)
        {
            loadGame.color = (selectedIndex == 0) ? Color.red : Color.white;
        }
        newGame.color = (selectedIndex == 1) ? Color.red : Color.white;
        credits.color = (selectedIndex == 2) ? Color.red : Color.white;
        quit.color = (selectedIndex == 3) ? Color.red : Color.white;
    }

    void HandleSelection()
    {
        switch (selectedIndex)
        {
            case 0:
                // Start Game and load last active scene
                string currentSceneName = SavestateManager.Instance.GetCurrentSaveState().playerSaveState.currentSceneName;
                SceneTransitionManager.Instance.LoadScene(SavestateManager.Instance.GetCurrentSaveState().playerSaveState.currentSceneName);
                break;
            case 1:
                // Start new game 
                SavestateManager.Instance.ClearSaveState();
                SceneTransitionManager.Instance.LoadScene("OminousStreetScene");
                break;
            case 2:
                // Credits
                Debug.Log("Credits selected");
                ToggleCredits();
                break;
            case 3:
                // Quit
                Debug.Log("Quit selected");
                Application.Quit(); // Note: This will only work in a built application, not in the Unity Editor
                break;
        }
    }

    void ToggleCredits()
    {
        if (creditsContainer.activeSelf)
        {
            creditsContainer.SetActive(false);
            buttons.SetActive(true);
        }
        else
        {
            creditsContainer.SetActive(true);
            buttons.SetActive(false);
        }
    }



}
