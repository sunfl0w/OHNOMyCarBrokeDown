using UnityEngine;
using System;

public class UnifiedGUI : MonoBehaviour {
    public static event Action GUIEnterEvent;
    public static event Action GUILeaveEvent;

    private bool guiVisible = false;

    void Start() {
        ItemInspectGUI.InspectionGUIEnterEvent += OnGUIEnter;
        ItemInspectGUI.InspectionGUILeaveEvent += OnGUILeave;
        InventoryGUI.InventoryGUIEnterEvent += OnGUIEnter;
        InventoryGUI.InventoryGUILeaveEvent += OnGUILeave;
        PauseMenuGUI.PauseGUIEnterEvent += OnGUIEnter;
        PauseMenuGUI.PauseGUILeaveEvent += OnGUILeave;
    }

    void OnDestroy() {
        ItemInspectGUI.InspectionGUIEnterEvent -= OnGUIEnter;
        ItemInspectGUI.InspectionGUILeaveEvent -= OnGUILeave;
        InventoryGUI.InventoryGUIEnterEvent -= OnGUIEnter;
        InventoryGUI.InventoryGUILeaveEvent -= OnGUILeave;
        PauseMenuGUI.PauseGUIEnterEvent -= OnGUIEnter;
        PauseMenuGUI.PauseGUILeaveEvent -= OnGUILeave;
    }

    public bool IsAnyGUIVisible() {
        return guiVisible;
    }

    void OnGUIEnter() {
        guiVisible = true;
        GUIEnterEvent?.Invoke();
    }

    void OnGUILeave() {
        guiVisible = false;
        GUILeaveEvent?.Invoke();
    }
}
