using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public TextMeshProUGUI startGame;
    public TextMeshProUGUI credits;
    public TextMeshProUGUI quit;
    public GameObject canvas;

    private int selectedIndex = 0;

    void Start()
    {

        ChangeSelection(0);
    }

    void Update()
    {
        if (canvas.activeSelf)
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
                // Start Game
                SceneManager.LoadScene("VillageScene");
                break;
            case 1:
                // Credits
                Debug.Log("Credits selected");
                break;
            case 2:
                // Quit
                Debug.Log("Quit selected");
                Application.Quit(); // Note: This will only work in a built application, not in the Unity Editor
                break;
        }
    }



}
