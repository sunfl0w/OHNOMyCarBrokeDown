using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuGUI : MonoBehaviour {
    public TextMeshProUGUI continueButton;
    public TextMeshProUGUI mainMenuButton;
    public Color highlightedColor = Color.red;

    public static event Action PauseGUIEnterEvent;
    public static event Action PauseGUILeaveEvent;

    private UnifiedGUI unifiedGUI;
    private bool isVisible = false;
    private int selectedButtonIndex = 0;

    void Start() {
        unifiedGUI = GameObject.FindGameObjectWithTag("UnifiedGUI").GetComponent<UnifiedGUI>();
        Hide();
    }

    void Update() {
        if (Input.GetButtonDown("Cancel") && isVisible) {
            PauseGUILeaveEvent?.Invoke();
            Hide();
        } else if (Input.GetButtonDown("Cancel") && !isVisible && !unifiedGUI.IsAnyGUIVisible()) {
            PauseGUIEnterEvent?.Invoke();
            Show();
        }

        if (isVisible) {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
                selectedButtonIndex = Math.Abs((selectedButtonIndex - 1) % 2);
            } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
                selectedButtonIndex = Math.Abs((selectedButtonIndex + 1) % 2);
            }

            if (selectedButtonIndex == 0) {
                continueButton.color = highlightedColor;
                mainMenuButton.color = Color.white;
                if (Input.GetKeyDown(KeyCode.Return)) {
                    PauseGUILeaveEvent?.Invoke();
                    Hide();
                }
            } else if (selectedButtonIndex == 1) {
                continueButton.color = Color.white;
                mainMenuButton.color = highlightedColor;
                if (Input.GetKeyDown(KeyCode.Return)) {
                    PauseGUILeaveEvent?.Invoke();
                    Hide();
                    SavestateManager.Instance.StoreSaveState();
                    SceneTransitionManager.Instance.LoadScene("MainMenuScene");
                }
            }
        }
    }

    void Show() {
        isVisible = true;
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
        }
        // TODO
        InteractGUI.Instance.Hide();
        
        Time.timeScale = 0.0f;
    }

    void Hide() {
        isVisible = false;
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
        Time.timeScale = 1.0f;
    }
}
