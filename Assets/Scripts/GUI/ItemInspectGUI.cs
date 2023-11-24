using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ItemInspectGUI : MonoBehaviour {
    public Camera mainCamera;
    public GameObject containerGameObject;
    public TextMeshProUGUI textGUI;

    private GameObject inspectedItem = null;

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

    public void Update () {
        inspectedItem?.transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }

    public void Show(ItemData itemData) {
        Debug.Log("Show item inspect GUI");
        onInspectionGUIEnter?.Invoke();
        containerGameObject.SetActive(true);
        textGUI.text = itemData.interactText;

        inspectedItem = Instantiate(itemData.prefab);
        inspectedItem.layer = LayerMask.NameToLayer("UI");
        inspectedItem.transform.position = mainCamera.transform.position + mainCamera.transform.forward * 1.0f;
        StartCoroutine(HideCoroutine());
    }

    public void Hide() {
        Debug.Log("Hide item inspect GUI");
        onInspectionGUILeave.Invoke();
        containerGameObject.SetActive(false);
        textGUI.text = String.Empty;
        if (inspectedItem != null) {
            Destroy(inspectedItem);
        }
        inspectedItem = null;
    }

    public IEnumerator HideCoroutine() {
        yield return new WaitForSeconds(5.0f);
        Hide();
    }
}
