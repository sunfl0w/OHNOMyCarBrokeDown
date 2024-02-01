using UnityEngine;
using TMPro;
using System;

/// <summary>
/// The item inspect gui displays an item in front of the camera.
/// The player can then rotate this item to inspect it from all sides.
/// </summary>
public class ItemInspectGUI : MonoBehaviour {
    public Camera guiCamera;
    public float rotationSpeed = 3.5f;
    public float rotationDampening = 2.0f;
    private TextMeshProUGUI hintTextGUI;
    private bool isVisible = false;

    private GameObject inspectedItem = null;
    private Vector3 currentRotationSpeed = Vector3.zero;

    public static event Action<bool, bool> InspectionGUIEnterEvent;
    public static event Action InspectionGUILeaveEvent;

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
            RotateInspectedItem();
        }
    }

    public void Show(Item item) {
        Debug.Log("Show item inspect GUI");
        InspectionGUIEnterEvent?.Invoke(true, true);
        hintTextGUI.text = item.GetInteractText() + "\nExit with [ESC]. Move item with [WASD].";

        inspectedItem = Instantiate(item.GetPrefab());
        MeshRenderer meshRenderer = inspectedItem.GetComponent<MeshRenderer>();
        // TODO
        /*meshRenderer.material = item.inspectMaterial;
        for (int i = 0; i < meshRenderer.materials.Length; i++) {
            meshRenderer.materials[i] = item.inspectMaterial;
        }*/
        inspectedItem.layer = LayerMask.NameToLayer("UI");
        inspectedItem.transform.position = guiCamera.transform.position + guiCamera.transform.forward * 1.0f;
        inspectedItem.transform.rotation = Quaternion.LookRotation(inspectedItem.transform.position - guiCamera.transform.position, Vector3.up) * Quaternion.Euler(90, 0, 0);
        isVisible = true;

        // TODO
        //InteractGUI.Instance.Hide();
    }

    public void Hide() {
        Debug.Log("Hide item inspect GUI");
        InspectionGUILeaveEvent?.Invoke();
        hintTextGUI.text = String.Empty;
        currentRotationSpeed = Vector3.zero;
        if (inspectedItem != null) {
            Destroy(inspectedItem);
        }
        inspectedItem = null;
        isVisible = false;
    }

    public bool IsVisible() {
        return isVisible;
    }

    private void RotateInspectedItem() {
        currentRotationSpeed.y += Input.GetAxisRaw("Horizontal") * rotationSpeed * Time.deltaTime - Mathf.Sign(currentRotationSpeed.y) * currentRotationSpeed.magnitude * rotationDampening * Time.deltaTime;
        currentRotationSpeed.x += Input.GetAxisRaw("Vertical") * rotationSpeed * Time.deltaTime - Mathf.Sign(currentRotationSpeed.x) * currentRotationSpeed.magnitude * rotationDampening * Time.deltaTime;
        inspectedItem.transform.RotateAround(inspectedItem.transform.position, guiCamera.transform.up, currentRotationSpeed.y);
        inspectedItem.transform.RotateAround(inspectedItem.transform.position, guiCamera.transform.right, currentRotationSpeed.x);
    }
}