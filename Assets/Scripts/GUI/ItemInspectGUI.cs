using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ItemInspectGUI : MonoBehaviour {
    public Camera guiCamera;
    public GameObject containerGameObject;
    public TextMeshProUGUI textGUI;

    private GameObject inspectedItem = null;
    private Vector3 rotationSpeed = Vector3.zero;

    public static event Action onInspectionGUIEnter;
    public static event Action onInspectionGUILeave;

    private static ItemInspectGUI instance;
    public static ItemInspectGUI Instance { get { return instance; } }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    private void Start() {
        Hide();
    }

    public void FixedUpdate () {
        if (inspectedItem != null) {
            rotationSpeed.y += Input.GetAxisRaw("Horizontal") * 3.5f * Time.deltaTime - Mathf.Sign(rotationSpeed.y) * rotationSpeed.magnitude * 2.0f * Time.deltaTime;
            rotationSpeed.x += Input.GetAxisRaw("Vertical") * 3.5f * Time.deltaTime - Mathf.Sign(rotationSpeed.x) * rotationSpeed.magnitude * 2.0f * Time.deltaTime;
            inspectedItem.transform.RotateAround(inspectedItem.transform.position, guiCamera.transform.up, rotationSpeed.y);
            inspectedItem.transform.RotateAround(inspectedItem.transform.position, guiCamera.transform.right, rotationSpeed.x);

            if (Input.GetKeyDown(KeyCode.E)) {
                Hide();
            }
        }
    }

    public void Show(ItemData itemData) {
        Debug.Log("Show item inspect GUI");
        onInspectionGUIEnter?.Invoke();
        containerGameObject.SetActive(true);
        textGUI.text = itemData.interactText + "\nExit with [E]. Move item with [WASD].";

        inspectedItem = Instantiate(itemData.prefab);
        inspectedItem.layer = LayerMask.NameToLayer("UI");
        inspectedItem.transform.position = guiCamera.transform.position + guiCamera.transform.forward * 1.0f;
        inspectedItem.transform.rotation = guiCamera.transform.rotation;
        //StartCoroutine(HideCoroutine());
    }

    public void Hide() {
        Debug.Log("Hide item inspect GUI");
        onInspectionGUILeave.Invoke();
        containerGameObject.SetActive(false);
        textGUI.text = String.Empty;
        rotationSpeed = Vector3.zero;
        if (inspectedItem != null) {
            Destroy(inspectedItem);
        }
        inspectedItem = null;
    }
}
