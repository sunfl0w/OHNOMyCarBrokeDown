using UnityEngine;
using TMPro;
using System;

/// <summary>
/// The item inspect gui displays an interacted item in front of the camera.
/// The item can be rotated using WASD.
/// </summary>
public class ItemInspectGUI : MonoBehaviour {
    public Camera guiCamera;
    //public GameObject containerGameObject;
    private TextMeshProUGUI hintTextGUI;
    private bool isVisible = false;

    private GameObject inspectedItem = null;
    private Vector3 rotationSpeed = Vector3.zero;

    public static event Action<bool, bool> InspectionGUIEnterEvent;
    public static event Action InspectionGUILeaveEvent;

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
        hintTextGUI = GetComponent<TextMeshProUGUI>();
        Hide();
    }

    public void Update() {
        if (inspectedItem != null && Input.GetButtonDown("Cancel")) {
            Hide();
        }
    }

    public void FixedUpdate() {
        if (inspectedItem != null) {
            rotationSpeed.y += Input.GetAxisRaw("Horizontal") * 3.5f * Time.deltaTime - Mathf.Sign(rotationSpeed.y) * rotationSpeed.magnitude * 2.0f * Time.deltaTime;
            rotationSpeed.x += Input.GetAxisRaw("Vertical") * 3.5f * Time.deltaTime - Mathf.Sign(rotationSpeed.x) * rotationSpeed.magnitude * 2.0f * Time.deltaTime;
            inspectedItem.transform.RotateAround(inspectedItem.transform.position, guiCamera.transform.up, rotationSpeed.y);
            inspectedItem.transform.RotateAround(inspectedItem.transform.position, guiCamera.transform.right, rotationSpeed.x);
        }
    }

    /// <summary>
    /// Show item inspect gui
    /// </summary>
    public void Show(ItemData itemData) {
        Debug.Log("Show item inspect GUI");
        InspectionGUIEnterEvent?.Invoke(true, true);
        hintTextGUI.text = itemData.interactText + "\nExit with [ESC]. Move item with [WASD].";

        inspectedItem = Instantiate(itemData.prefab);
        MeshRenderer meshRenderer = inspectedItem.GetComponent<MeshRenderer>();
        meshRenderer.material = itemData.inspectMaterial;
        for (int i = 0; i < meshRenderer.materials.Length; i++) {
            meshRenderer.materials[i] = itemData.inspectMaterial;
        }
        inspectedItem.layer = LayerMask.NameToLayer("UI");
        inspectedItem.transform.position = guiCamera.transform.position + guiCamera.transform.forward * 1.0f;
        inspectedItem.transform.rotation = Quaternion.LookRotation(inspectedItem.transform.position - guiCamera.transform.position, Vector3.up) * Quaternion.Euler(90, 0, 0);
        isVisible = true;

        // TODO
        InteractGUI.Instance.Hide();
    }

    /// <summary>
    /// Hide item inspect gui
    /// </summary>
    public void Hide() {
        Debug.Log("Hide item inspect GUI");
        InspectionGUILeaveEvent?.Invoke();
        hintTextGUI.text = String.Empty;
        rotationSpeed = Vector3.zero;
        if (inspectedItem != null) {
            Destroy(inspectedItem);
        }
        inspectedItem = null;
        isVisible = false;
    }

    /// <summary>
    /// Returns true if the item inspect gui is visible.
    /// </summary>
    public bool IsVisible() {
        return isVisible;
    }
}