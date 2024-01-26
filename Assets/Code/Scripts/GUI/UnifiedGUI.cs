using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// The unified gui behaviour stores a global gui state and triggers global gui enter and leave events.
/// </summary>
public class UnifiedGUI : MonoBehaviour {
    /// <summary>
    /// Global gui enter event.
    /// </summary>
    public static event Action<bool, bool> GUIEnterEvent;

    /// <summary>
    /// Global gui leave event.
    /// </summary>
    public static event Action GUILeaveEvent;

    /// <summary>
    /// Global gui visibility state.
    /// </summary>
    private bool guiVisible = false;

    void Start() {
        ItemInspectGUI.InspectionGUIEnterEvent += OnGUIEnter;
        ItemInspectGUI.InspectionGUILeaveEvent += OnGUILeave;
        InventoryGUI.InventoryGUIEnterEvent += OnGUIEnter;
        InventoryGUI.InventoryGUILeaveEvent += OnGUILeave;
        PauseMenuGUI.PauseGUIEnterEvent += OnGUIEnter;
        PauseMenuGUI.PauseGUILeaveEvent += OnGUILeave;
        DialogueGUI.DialogueGUIEnterEvent += OnGUIEnter;
        DialogueGUI.DialogueGUILeaveEvent += OnGUILeave;
        GameoverGUI.GameoverGUIEnterEvent += OnGUIEnter;
        GameoverGUI.GameoverGUILeaveEvent += OnGUILeave;
    }

    void OnDestroy() {
        ItemInspectGUI.InspectionGUIEnterEvent -= OnGUIEnter;
        ItemInspectGUI.InspectionGUILeaveEvent -= OnGUILeave;
        InventoryGUI.InventoryGUIEnterEvent -= OnGUIEnter;
        InventoryGUI.InventoryGUILeaveEvent -= OnGUILeave;
        PauseMenuGUI.PauseGUIEnterEvent -= OnGUIEnter;
        PauseMenuGUI.PauseGUILeaveEvent -= OnGUILeave;
        DialogueGUI.DialogueGUIEnterEvent -= OnGUIEnter;
        DialogueGUI.DialogueGUILeaveEvent -= OnGUILeave;
        GameoverGUI.GameoverGUIEnterEvent -= OnGUIEnter;
        GameoverGUI.GameoverGUILeaveEvent -= OnGUILeave;
    }

    /// <summary>
    /// Returns true if any gui is visible.
    /// </summary>
    public bool IsAnyGUIVisible() {
        return guiVisible;
    }

    /// <summary>
    /// Called when any gui is entered.
    /// </summary>
    void OnGUIEnter(bool enableGUIBlur, bool disableMovement) {
        guiVisible = true;
        GUIEnterEvent?.Invoke(enableGUIBlur, disableMovement);
    }

    /// <summary>
    /// Called when any gui is closed.
    /// </summary>
    void OnGUILeave() {
        StartCoroutine(GUILeave()); // Delay leave so that exiting and entering a GUI can not happen in the same frame
    }

    /// <summary>
    /// Coroutine used to invoke a gui leave event after a short time period.
    /// This prevents guis being opened and closed in the same frame.
    /// </summary>
    private IEnumerator GUILeave() {
        yield return new WaitForSeconds(0.1f);
        guiVisible = false;
        GUILeaveEvent?.Invoke();
    }
}