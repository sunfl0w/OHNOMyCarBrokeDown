using UnityEngine;
using TMPro;
using System;

/// <summary>
/// The interact gui displays interact information for interactables near player.
/// </summary>
public class InteractGUI : MonoBehaviour {
    private TextMeshProUGUI hintTextGUI;
    private bool isVisible = false;

    public void Start() {
        hintTextGUI = GetComponent<TextMeshProUGUI>();
        Hide();
    }

    public void Show(IInteractable interactable) {
        Debug.Log("Show interact GUI");
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