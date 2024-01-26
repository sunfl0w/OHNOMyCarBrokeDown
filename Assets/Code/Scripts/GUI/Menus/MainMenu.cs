using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// Main menu.
/// </summary>
public class MainMenu : MonoBehaviour {
    /// <summary>
    /// Reference to the load game button.
    /// </summary>
    public TextMeshProUGUI loadGame;

    /// <summary>
    /// Reference to the new game button.
    /// </summary>
    public TextMeshProUGUI newGame;

    /// <summary>
    /// Reference to the settings button.
    /// </summary>
    public TextMeshProUGUI settings;

    /// <summary>
    /// Reference to the credits button.
    /// </summary>
    public TextMeshProUGUI credits;

    /// <summary>
    /// Reference to the quit game button.
    /// </summary>
    public TextMeshProUGUI quit;

    /// <summary>
    /// Reference to the volume setting text.
    /// </summary>
    public TextMeshProUGUI volume;

    /// <summary>
    /// Reference to the volume slider.
    /// </summary>
    public Slider volumeSlider;

    /// <summary>
    /// Reference to the brightness setting text.
    /// </summary>
    public TextMeshProUGUI brightness;

    /// <summary>
    /// Reference to the brightness slider.
    /// </summary>
    public Slider brightnessSlider;

    /// <summary>
    /// Reference to the setting confirm button.
    /// </summary>
    public TextMeshProUGUI confirm;

    /// <summary>
    /// Reference to the game object holding all main menu related objects (except credits and settings).
    /// </summary>
    public GameObject buttons;

    /// <summary>
    /// Reference to the game object holding all settings related objects.
    /// </summary>
    public GameObject settingsContainer;

    /// <summary>
    /// Reference to the game object holding all credits related objects.
    /// </summary>
    public GameObject creditsContainer;

    /// <summary>
    /// Reference to the global audio mixer.
    /// </summary>
    public AudioMixer mixer;

    /// <summary>
    /// Selected menu index to identify the currently selected main menu button.
    /// </summary>
    private int selectedIndexMenu = 0;

    /// <summary>
    /// Selected setting index to identify the currently selected setting menu button.
    /// </summary>
    private int selectedIndexSettings = 0;

    /// <summary>
    /// Flag specifying, whether a save state can be loaded from disk.
    /// </summary>
    private bool canLoad = false;

    void Start() {
        string currentSceneName = SavestateManager.Instance.GetCurrentSaveState().playerSaveState.currentSceneName;
        if (currentSceneName != String.Empty) {
            canLoad = true;
        } else {
            canLoad = false;
            loadGame.color = Color.grey;
        }
        settingsContainer.SetActive(false);
        ChangeSelectionMenu(0);
        ChangeSelectionSettings(0);
        volumeSlider.value = Mathf.Clamp(PlayerPrefs.GetFloat("Volume", 0.6f), 0.0f, 1.0f);
        brightnessSlider.value = Mathf.Clamp(PlayerPrefs.GetFloat("Brightness", 0.5f), 0.0f, 1.0f);
        mixer.SetFloat("MasterVolume", (volumeSlider.value - 0.8f) * 80.0f);
    }

    void Update() {
        if (buttons.activeSelf) {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
                ChangeSelectionMenu(-1);
            } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
                ChangeSelectionMenu(1);
            }

            if (Input.GetKeyDown(KeyCode.Return)) {
                HandleSelectionMenu();
            }
        } else if (settingsContainer.activeSelf) {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
                ChangeSelectionSettings(-1);
            } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
                ChangeSelectionSettings(1);
            }

            if (Input.GetKeyDown(KeyCode.Return)) {
                HandleSelectionSettings();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
                HandleSlidersSettings(-1);
            } else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
                HandleSlidersSettings(1);
            }
        } else {
            if (Input.GetKeyDown(KeyCode.Return)) {
                ToggleCredits();
            }
        }

    }

    /// <summary>
    /// Change selection in main menu.
    /// </summary>
    void ChangeSelectionMenu(int direction) {
        selectedIndexMenu = (selectedIndexMenu + direction + 5) % 5;
        if (!canLoad && selectedIndexMenu == 0) { selectedIndexMenu = (direction == 1) ? 1 : 4; }

        if (canLoad) {
            loadGame.color = (selectedIndexMenu == 0) ? Color.red : Color.white;
        }
        newGame.color = (selectedIndexMenu == 1) ? Color.red : Color.white;
        settings.color = (selectedIndexMenu == 2) ? Color.red : Color.white;
        credits.color = (selectedIndexMenu == 3) ? Color.red : Color.white;
        quit.color = (selectedIndexMenu == 4) ? Color.red : Color.white;
    }

    /// <summary>
    /// Change selection in settings menu.
    /// </summary>
    void ChangeSelectionSettings(int direction) {
        selectedIndexSettings = (selectedIndexSettings + direction + 3) % 3;

        volume.color = (selectedIndexSettings == 0) ? Color.red : Color.white;
        brightness.color = (selectedIndexSettings == 1) ? Color.red : Color.white;
        confirm.color = (selectedIndexSettings == 2) ? Color.red : Color.white;
    }

    /// <summary>
    /// Handle selection in main menu.
    /// </summary>
    void HandleSelectionMenu() {
        switch (selectedIndexMenu) {
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
                buttons.SetActive(false);
                settingsContainer.SetActive(true);
                break;
            case 3:
                // Credits
                Debug.Log("Credits selected");
                ToggleCredits();
                break;
            case 4:
                // Quit
                Debug.Log("Quit selected");
                Application.Quit(); // Note: This will only work in a built application, not in the Unity Editor
                break;
        }
    }

    /// <summary>
    /// Handle selection in settings menu.
    /// </summary>
    void HandleSelectionSettings() {
        switch (selectedIndexSettings) {
            case 2:
                PlayerPrefs.SetFloat("Volume", volumeSlider.value);
                PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);
                mixer.SetFloat("MasterVolume", (volumeSlider.value - 0.8f) * 80.0f);
                PPFXManager.Instance.UpdateBrightness();
                buttons.SetActive(true);
                settingsContainer.SetActive(false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Handle sliders settings in settings menu.
    /// </summary>
    void HandleSlidersSettings(int direction) {
        if (selectedIndexSettings == 0) {
            volumeSlider.value = Mathf.Clamp(volumeSlider.value + direction * 0.05f, 0.0f, 1.0f);
            mixer.SetFloat("MasterVolume", (volumeSlider.value - 0.8f) * 80.0f);
        } else if (selectedIndexSettings == 1) {
            brightnessSlider.value = Mathf.Clamp(brightnessSlider.value + direction * 0.05f, 0.0f, 1.0f);
            PPFXManager.Instance.UpdateBrightnessDirect(brightnessSlider.value);
        }
    }

    /// <summary>
    /// Toggles credits.
    /// </summary>
    void ToggleCredits() {
        if (creditsContainer.activeSelf) {
            creditsContainer.SetActive(false);
            buttons.SetActive(true);
        } else {
            creditsContainer.SetActive(true);
            buttons.SetActive(false);
        }
    }
}