using UnityEngine;
using TMPro;
using System;

public class InteractGUI : MonoBehaviour {
    private TextMeshProUGUI hintTextGUI;
    private bool isVisible = false;

    private static InteractGUI instance;
    public static InteractGUI Instance { get { return instance; } }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    public void Start() {
        hintTextGUI = GetComponent<TextMeshProUGUI>();
        Hide();
    }

    public void Show(IInteractable interactable) {
        Debug.Log("Show item pickup GUI");
        hintTextGUI.text = interactable.GetData().interactText;
        isVisible = true;
    }

    public void Hide() {
        hintTextGUI.text = String.Empty;
        isVisible = false;
    }

    public bool IsVisible() {
        return isVisible;
    }
}
