using UnityEngine;
using System;
using System.Collections;

public class UnifiedGUI : MonoBehaviour
{
    public static event Action<bool, bool> GUIEnterEvent;
    public static event Action GUILeaveEvent;

    private bool guiVisible = false;

    void Start()
    {
        ItemInspectGUI.InspectionGUIEnterEvent += OnGUIEnter;
        ItemInspectGUI.InspectionGUILeaveEvent += OnGUILeave;
        InventoryGUI.InventoryGUIEnterEvent += OnGUIEnter;
        InventoryGUI.InventoryGUILeaveEvent += OnGUILeave;
        PauseMenuGUI.PauseGUIEnterEvent += OnGUIEnter;
        PauseMenuGUI.PauseGUILeaveEvent += OnGUILeave;
        DialogueGUI.DialogueGUIEnterEvent += OnGUIEnter;
        DialogueGUI.DialogueGUILeaveEvent += OnGUILeave;
        Gameover.GameoverEnterEvent += OnGUIEnter;
        Gameover.GameoverLeaveEvent += OnGUILeave;
    }

    void OnDestroy()
    {
        ItemInspectGUI.InspectionGUIEnterEvent -= OnGUIEnter;
        ItemInspectGUI.InspectionGUILeaveEvent -= OnGUILeave;
        InventoryGUI.InventoryGUIEnterEvent -= OnGUIEnter;
        InventoryGUI.InventoryGUILeaveEvent -= OnGUILeave;
        PauseMenuGUI.PauseGUIEnterEvent -= OnGUIEnter;
        PauseMenuGUI.PauseGUILeaveEvent -= OnGUILeave;
        DialogueGUI.DialogueGUIEnterEvent -= OnGUIEnter;
        DialogueGUI.DialogueGUILeaveEvent -= OnGUILeave;
        Gameover.GameoverEnterEvent -= OnGUIEnter;
        Gameover.GameoverLeaveEvent -= OnGUILeave;
    }

    public bool IsAnyGUIVisible()
    {
        return guiVisible;
    }

    void OnGUIEnter(bool enableGUIBlur, bool disableMovement)
    {
        guiVisible = true;
        GUIEnterEvent?.Invoke(enableGUIBlur, disableMovement);
    }

    void OnGUILeave()
    {
        StartCoroutine(GUILeave()); // Delay leave so that exiting and entering a GUI can not happen in the same frame
    }

    private IEnumerator GUILeave()
    {
        yield return new WaitForSeconds(0.1f);
        guiVisible = false;
        GUILeaveEvent?.Invoke();
    }
}
