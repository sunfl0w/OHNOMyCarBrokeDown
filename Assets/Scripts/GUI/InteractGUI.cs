using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class InteractGUI : MonoBehaviour {
    public GameObject containerGameObject;
    public TextMeshProUGUI textGUI;

    private static InteractGUI instance;
    public static InteractGUI Instance { get { return instance; } }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
        Hide();
    }

    public void Show(IInteractable interactable) {
        Debug.Log("Show item pickup GUI");
        containerGameObject.SetActive(true);
        textGUI.text = interactable.GetData().interactText;
    }

    public void Hide() {
        containerGameObject.SetActive(false);
        textGUI.text = String.Empty;
    }

    public bool IsVisible() {
        return containerGameObject.activeSelf;
    }
}
