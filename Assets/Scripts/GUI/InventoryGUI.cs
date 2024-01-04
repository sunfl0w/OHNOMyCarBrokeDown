using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


public class InventoryGUI : MonoBehaviour
{

    public Camera guiCamera;
    public GameObject InventoryCanvas;
    private bool canvasActivated;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;

    public TextMeshProUGUI leftArrow;
    public TextMeshProUGUI rightArrow;

    public TextMeshProUGUI state;
    public TextMeshProUGUI usageButton;

    public static event Action onInspectionGUIEnter;
    public static event Action onInspectionGUILeave;

    private int currentItemIndex = 0;
    private ItemData currentItemData;
    private Vector3 rotationSpeed = Vector3.zero;

    private GameObject inspectedItem = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory") && canvasActivated)
        {
            onInspectionGUILeave?.Invoke();

            DestroyInspectedItem();
            rotationSpeed = Vector3.zero;
            InventoryCanvas.SetActive(false);
            canvasActivated = false;
        }
        else if (Input.GetButtonDown("Inventory") && !canvasActivated)
        {
            onInspectionGUIEnter?.Invoke();
            InventoryCanvas.SetActive(true);
            canvasActivated = true;
            currentItemIndex = 0;
            PlayerInventory.Instance.printItems();
            updateUI();
        }
        else if ((Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.LeftArrow)) && canvasActivated)
        {

            if (leftArrow.color == Color.white)
            {
                currentItemIndex -= 1;
                updateUI();
            }

        }
        else if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.RightArrow)) && canvasActivated)
        {
            if (rightArrow.color == Color.white)
            {
                currentItemIndex += 1;
                updateUI();
            }

        }
        else if (Input.GetKeyDown(KeyCode.E) && canvasActivated && usageButton.text == "[E]quip")
        {
            (ItemData item, uint count) tuple = PlayerInventory.Instance.items[currentItemIndex];
            currentItemData = tuple.item;
            PlayerInventory.Instance.EquipItem(currentItemData);
            updateUI();

        }
        else if (Input.GetKeyDown(KeyCode.U) && canvasActivated && usageButton.text == "[U]se")
        {
            (ItemData item, uint count) tuple = PlayerInventory.Instance.items[currentItemIndex];
            currentItemData = tuple.item;
            PlayerInventory.Instance.UseItem(currentItemData);
            if (!PlayerInventory.Instance.itemExists(currentItemData))
            {
                currentItemIndex = 0;
            }
            updateUI();
        }


    }

    void updateUI()
    {
        if (PlayerInventory.Instance.items.Count == 0)
        {
            itemName.text = "No item in inventory.";
            itemDescription.text = "";
            leftArrow.color = Color.grey;
            rightArrow.color = Color.grey;
            state.text = "";
            usageButton.text = "";
            DestroyInspectedItem();

        }
        else
        {
            if (currentItemIndex >= 0 && (currentItemIndex + 1) < PlayerInventory.Instance.items.Count)
            {
                rightArrow.color = Color.white;
            }
            else
            {
                rightArrow.color = Color.grey;
            }
            if (currentItemIndex != 0)
            {
                leftArrow.color = Color.white;
            }
            else
            {
                leftArrow.color = Color.grey;
            }

            (ItemData item, uint count) tuple = PlayerInventory.Instance.items[currentItemIndex];
            currentItemData = tuple.item;
            ViewCurrentItem(currentItemData);
            ShowItemUsage(currentItemData);

        }
    }

    public void FixedUpdate()
    {
        if (inspectedItem != null)
        {
            rotationSpeed.y += Input.GetAxisRaw("Horizontal") * 3.5f * Time.deltaTime - Mathf.Sign(rotationSpeed.y) * rotationSpeed.magnitude * 2.0f * Time.deltaTime;
            rotationSpeed.x += Input.GetAxisRaw("Vertical") * 3.5f * Time.deltaTime - Mathf.Sign(rotationSpeed.x) * rotationSpeed.magnitude * 2.0f * Time.deltaTime;
            inspectedItem.transform.RotateAround(inspectedItem.transform.position, guiCamera.transform.up, rotationSpeed.y);
            inspectedItem.transform.RotateAround(inspectedItem.transform.position, guiCamera.transform.right, rotationSpeed.x);

        }
    }

    private void ViewCurrentItem(ItemData currentItemData)
    {

        itemName.text = currentItemData.itemName;
        itemDescription.text = currentItemData.interactText;

        DestroyInspectedItem();

        inspectedItem = Instantiate(currentItemData.prefab);
        inspectedItem.layer = LayerMask.NameToLayer("UI");
        inspectedItem.transform.position = guiCamera.transform.position + guiCamera.transform.forward * 1.0f;
        inspectedItem.transform.rotation = guiCamera.transform.rotation;
    }

    private void ShowItemUsage(ItemData currentItemData)
    {
        if (PlayerInventory.Instance.equipment == currentItemData)
        {
            state.text = "*equipped";
            usageButton.text = "";
        }
        else if (currentItemData.category == ItemCategory.Weapon)
        {
            state.text = "";
            usageButton.text = "[E]quip";
        }
        else if (currentItemData.category == ItemCategory.Resource)
        {
            state.text = "";
            usageButton.text = "[U]se";
        }
        else if (currentItemData.category == ItemCategory.Normal)
        {
            state.text = "";
            usageButton.text = "";
        }
    }

    private void DestroyInspectedItem()
    {
        if (inspectedItem != null)
        {
            Destroy(inspectedItem.gameObject);
        }
        inspectedItem = null;
    }



}
