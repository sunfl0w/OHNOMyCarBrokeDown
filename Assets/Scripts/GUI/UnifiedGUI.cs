using UnityEngine;
using System;

public class UnifiedGUI : MonoBehaviour {
    public static event Action<bool, bool> GUIEnterEvent;
    public static event Action GUILeaveEvent;

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
    }

    public bool IsAnyGUIVisible() {
        return guiVisible;
    }

    void OnGUIEnter(bool enableGUIBlur, bool disableMovement) {
        guiVisible = true;
        GUIEnterEvent?.Invoke(enableGUIBlur, disableMovement);
    }

    void OnGUILeave() {
        guiVisible = false;
        GUILeaveEvent?.Invoke();
    }
}
