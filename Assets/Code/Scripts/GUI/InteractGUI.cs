using UnityEngine;
using TMPro;
using System;

/// <summary>
/// The interact gui displays interact information for interactables in range.
/// The interact gui is a singleton.
/// </summary>
public class InteractGUI : MonoBehaviour {
    /// <summary>
    /// Hint gui text reference.
    /// </summary>
    private TextMeshProUGUI hintTextGUI;

    /// <summary>
    /// Current visibility status of the interact gui.
    /// </summary>
    private bool isVisible = false;

    /// <summary>
    /// Current interactable.
    /// </summary>
    private IInteractable currentInteractable;

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

    /// <summary>
    /// Show interact gui.
    /// </summary>
    public void Show(IInteractable interactable) {
        Debug.Log("Show interact GUI");
        hintTextGUI.text = interactable.GetData().interactText;
        isVisible = true;
        currentInteractable = interactable;
    }

    /// <summary>
    /// Hide interact gui.
    /// </summary>
    public void Hide() {
        hintTextGUI.text = String.Empty;
        isVisible = false;
        currentInteractable = null;
    }

    /// <summary>
    /// Returns true if the interact gui is visible.
    /// </summary>
    public bool IsVisible() {
        return isVisible;
    }

    /// <summary>
    /// Returns the current interactable displayed by the gui.
    /// </summary>
    public IInteractable GetCurrentInteractable() {
        return currentInteractable;
    }
}