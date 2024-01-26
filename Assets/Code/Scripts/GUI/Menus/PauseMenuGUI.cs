using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Pause menu gui.
/// </summary>
public class PauseMenuGUI : MonoBehaviour {
    /// <summary>
    /// Continue button reference.
    /// </summary>
    public TextMeshProUGUI continueButton;

    /// <summary>
    /// Main menu button reference.
    /// </summary>
    public TextMeshProUGUI mainMenuButton;

    /// <summary>
    /// Menu item highlight color.
    /// </summary>
    public Color highlightedColor = Color.red;

    /// <summary>
    /// Pause gui enter event.
    /// </summary>
    public static event Action<bool, bool> PauseGUIEnterEvent;

    /// <summary>
    /// Pause gui leave event.
    /// </summary>
    public static event Action PauseGUILeaveEvent;

    /// <summary>
    /// Reference to the unified gui.
    /// </summary>
    private UnifiedGUI unifiedGUI;

    /// <summary>
    /// Specifies, whether the pause gui is currently visible.
    /// </summary>
    private bool isVisible = false;

    /// <summary>
    /// Currently selected button index.
    /// </summary>
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
            PauseGUIEnterEvent?.Invoke(true, true);
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

    /// <summary>
    /// Show pause menu gui.
    /// </summary>
    void Show() {
        isVisible = true;
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
        }
        // TODO
        InteractGUI.Instance.Hide();

        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// Hide pause menu gui.
    /// </summary>
    void Hide() {
        isVisible = false;
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
        Time.timeScale = 1.0f;
    }
}