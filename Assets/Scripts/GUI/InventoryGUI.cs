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

    public TextMeshProUGUI status;
    public TextMeshProUGUI equipButton;

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

            if (inspectedItem != null)
            {
                Destroy(inspectedItem.gameObject);
            }
            inspectedItem = null;
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
        else if (Input.GetKeyDown(KeyCode.E) && canvasActivated && inspectedItem != null)
        {
            if (equipButton.text != "")
            {
                (ItemData item, uint count) tuple = PlayerInventory.Instance.items[currentItemIndex];
                currentItemData = tuple.item;
                PlayerInventory.Instance.equipItem(currentItemData);
                updateUI();
            }
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
            ViewCurrentItem();

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

    private void ViewCurrentItem()
    {
        (ItemData item, uint count) tuple = PlayerInventory.Instance.items[currentItemIndex];
        currentItemData = tuple.item;
        itemName.text = currentItemData.itemName;
        itemDescription.text = currentItemData.interactText;
        if (PlayerInventory.Instance.equipment == currentItemData)
        {
            status.text = "*equipped";
            equipButton.text = "";
        }
        else
        {
            status.text = "";
            equipButton.text = "[E]quip";
        }

        if (inspectedItem != null)
        {
            Destroy(inspectedItem.gameObject);
        }

        inspectedItem = Instantiate(currentItemData.prefab);
        inspectedItem.layer = LayerMask.NameToLayer("UI");
        inspectedItem.transform.position = guiCamera.transform.position + guiCamera.transform.forward * 1.0f;
        inspectedItem.transform.rotation = guiCamera.transform.rotation;
    }




}
