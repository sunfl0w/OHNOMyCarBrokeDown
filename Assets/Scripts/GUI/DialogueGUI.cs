using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class DialogueGUI : MonoBehaviour {
    private TextMeshProUGUI textGUI = null;
    private DialogueData dialogueData = null;
    private bool isVisible = false;

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
        textGUI = GetComponent<TextMeshProUGUI>();
        Hide();
    }

    public void Update() {
        if (Input.GetButtonDown("Cancel")) {
            Hide();
        }
    }

    public void Show(DialogueData dialogueData) {
        Debug.Log("Show interact GUI");
        isVisible = true;
        this.dialogueData = dialogueData;
        if (dialogueData != null) {
            StartCoroutine(DisplayDialogue());
        }
        DialogueGUIEnterEvent?.Invoke(false, false);
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
        for(int i = 0; i < dialogueData.text.Length; i++) {
            if (dialogueData != null) {
                textGUI.text += dialogueData.text[i];
            }
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(3.0f);
        Hide();
    }
}
