using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// The gameover gui is displayed when the player character died.
/// The gameover gui is a singleton.
/// </summary>
public class GameoverGUI : MonoBehaviour {
    /// <summary>
    /// Reference to the unified gui.
    /// </summary>
    private UnifiedGUI unifiedGUI;

    /// <summary>
    /// Gameover gui enter event.
    /// </summary>
    public static event Action<bool, bool> GameoverGUIEnterEvent;

    /// <summary>
    /// Gameover gui leave event.
    /// </summary>
    public static event Action GameoverGUILeaveEvent;

    private static GameoverGUI instance;
    public static GameoverGUI Instance { get { return instance; } }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    void Start() {
        unifiedGUI = GameObject.FindGameObjectWithTag("UnifiedGUI").GetComponent<UnifiedGUI>();
        Hide();
    }

    /// <summary>
    /// Hide gameover gui.
    /// </summary>
    void Hide() {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Show gameover gui.
    /// </summary>
    public void Show() {
        GameoverGUIEnterEvent?.Invoke(false, true);
        this.gameObject.SetActive(true);

        StartCoroutine(BackToMainMenu());
    }

    /// <summary>
    /// Coroutine to facilitate the transition back to the main menu after player character death.
    /// </summary>
    private IEnumerator BackToMainMenu() {
        yield return new WaitForSeconds(3f);
        GameoverGUILeaveEvent?.Invoke();
        SavestateManager.Instance.ClearSaveState();
        SceneTransitionManager.Instance.LoadScene("MainMenuScene");
    }
}