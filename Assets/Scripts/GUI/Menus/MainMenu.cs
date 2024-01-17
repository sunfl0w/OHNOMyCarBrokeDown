using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MainMenu : MonoBehaviour
{

    public TextMeshProUGUI startGame;
    public TextMeshProUGUI credits;
    public TextMeshProUGUI quit;
    public GameObject buttons;

    public GameObject creditsContainer;

    private int selectedIndex = 0;

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
        selectedIndex = (selectedIndex + direction + 3) % 3;

        startGame.color = (selectedIndex == 0) ? Color.red : Color.white;
        credits.color = (selectedIndex == 1) ? Color.red : Color.white;
        quit.color = (selectedIndex == 2) ? Color.red : Color.white;
    }

    void HandleSelection()
    {
        switch (selectedIndex)
        {
            case 0:
                // Start Game and load last active scene
                string currentSceneName = SavestateManager.Instance.GetCurrentSaveState().playerSaveState.currentSceneName;
                if (currentSceneName != String.Empty)
                {
                    SceneTransitionManager.Instance.LoadScene(SavestateManager.Instance.GetCurrentSaveState().playerSaveState.currentSceneName);
                }
                else
                {
                    SceneTransitionManager.Instance.LoadScene("OminousStreetScene");
                }
                break;
            case 1:
                // Credits
                Debug.Log("Credits selected");
                ToggleCredits();
                break;
            case 2:
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
