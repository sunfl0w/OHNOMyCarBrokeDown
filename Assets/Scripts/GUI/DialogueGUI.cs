using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class DialogueGUI : MonoBehaviour {
    private TextMeshProUGUI textGUI = null;
    private DialogueData dialogueData = null;
    private bool isVisible = false;

    private UnifiedGUI unifiedGUI;

    public static event Action<bool, bool> DialogueGUIEnterEvent;
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

    public void Show(DialogueData dialogueData) {
        if (!unifiedGUI.IsAnyGUIVisible() || (isVisible && this.dialogueData.text != dialogueData.text)) {
            Debug.Log("Show dialogue GUI");
            StopAllCoroutines();
            textGUI.text = String.Empty;
            isVisible = true;
            this.dialogueData = dialogueData;
            if (dialogueData != null) {
                StartCoroutine(DisplayDialogue());
            }
            DialogueGUIEnterEvent?.Invoke(false, false);
        }
    }

    public void Hide() {
        textGUI.text = String.Empty;
        isVisible = false;
        StopAllCoroutines();
        DialogueGUILeaveEvent?.Invoke();
    }

    public bool IsVisible() {
        return isVisible;
    }

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
