using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour {
    public TextMeshProUGUI loadGame;
    public TextMeshProUGUI newGame;
    public TextMeshProUGUI settings;
    public TextMeshProUGUI credits;
    public TextMeshProUGUI quit;
    public TextMeshProUGUI volume;
    public Slider volumeSlider;
    public TextMeshProUGUI brightness;
    public Slider brightnessSlider;
    public TextMeshProUGUI confirm;
    public GameObject buttons;
    public GameObject settingsContainer;
    public GameObject creditsContainer;
    public AudioMixer mixer;

    private int selectedIndexMenu = 0;
    private int selectedIndexSettings = 0;
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

    void ChangeSelectionSettings(int direction) {
        selectedIndexSettings = (selectedIndexSettings + direction + 3) % 3;

        volume.color = (selectedIndexSettings == 0) ? Color.red : Color.white;
        brightness.color = (selectedIndexSettings == 1) ? Color.red : Color.white;
        confirm.color = (selectedIndexSettings == 2) ? Color.red : Color.white;
    }

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

    void HandleSlidersSettings(int direction) {
        if (selectedIndexSettings == 0) {
            volumeSlider.value = Mathf.Clamp(volumeSlider.value + direction * 0.05f, 0.0f, 1.0f);
            mixer.SetFloat("MasterVolume", (volumeSlider.value - 0.8f) * 80.0f);
        } else if(selectedIndexSettings == 1) {
            brightnessSlider.value = Mathf.Clamp(brightnessSlider.value + direction * 0.05f, 0.0f, 1.0f);
            PPFXManager.Instance.UpdateBrightnessDirect(brightnessSlider.value);
        }
    }

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