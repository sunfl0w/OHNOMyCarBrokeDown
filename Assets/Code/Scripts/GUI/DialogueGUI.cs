using UnityEngine;
using TMPro;
using System;
using System.Collections;

/// <summary>
/// The dialogue gui displays dialogue texts.
/// The dialogue gui is a singleton.
/// </summary>
public class DialogueGUI : MonoBehaviour {
    /// <summary>
    /// Gui text to display dialogue.
    /// </summary>
    private TextMeshProUGUI textGUI = null;

    /// <summary>
    /// Current dialogue data.
    /// </summary>
    private DialogueData dialogueData = null;

    /// <summary>
    /// Current gui visibility state.
    /// </summary>
    private bool isVisible = false;

    /// <summary>
    /// Reference to the unified gui.
    /// </summary>
    private UnifiedGUI unifiedGUI;

    /// <summary>
    /// Dialogue gui enter event.
    /// </summary>
    public static event Action<bool, bool> DialogueGUIEnterEvent;

    /// <summary>
    /// Dialogue gui leave event.
    /// </summary>
    public static event Action DialogueGUILeaveEvent;

    private static DialogueGUI instance;
    public static DialogueGUI Instance { get { return instance; } }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    public void Start() {
        unifiedGUI = GameObject.FindGameObjectWithTag("UnifiedGUI").GetComponent<UnifiedGUI>();
        textGUI = GetComponent<TextMeshProUGUI>();
        Hide();
    }

    public void Update() {
        if (isVisible && Input.GetButtonDown("Cancel")) {
            Hide();
        }
    }

    /// <summary>
    /// Show dialogue gui.
    /// </summary>
    public void Show(DialogueData dialogueData) {
        if (!unifiedGUI.IsAnyGUIVisible() || (isVisible && this.dialogueData.text != dialogueData.text)) {
            Debug.Log("Show dialogue GUI");
            StopAllCoroutines();
            // TODO
            InteractGUI.Instance.Hide();
            textGUI.text = String.Empty;
            isVisible = true;
            this.dialogueData = dialogueData;
            if (dialogueData != null) {
                StartCoroutine(DisplayDialogue());
            }
            DialogueGUIEnterEvent?.Invoke(false, false);
        }
    }

    /// <summary>
    /// Hide dialogue gui.
    /// </summary>
    public void Hide() {
        textGUI.text = String.Empty;
        isVisible = false;
        StopAllCoroutines();
        DialogueGUILeaveEvent?.Invoke();
    }

    /// <summary>
    /// Returns true if gui is visible.
    /// </summary>
    public bool IsVisible() {
        return isVisible;
    }

    /// <summary>
    /// Coroutine to display dialogue text character by character asychronously.
    /// </summary>
    private IEnumerator DisplayDialogue() {
        for (int i = 0; i < dialogueData.text.Length; i++) {
            if (dialogueData != null) {
                textGUI.text += dialogueData.text[i];
            }
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(3.0f);
        Hide();
    }
}