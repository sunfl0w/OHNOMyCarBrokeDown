using UnityEngine;
using TMPro;
using System;


public class InventoryGUI : MonoBehaviour
{
    public Camera guiCamera;
    private bool isVisible = false;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;

    public TextMeshProUGUI leftArrow;
    public TextMeshProUGUI rightArrow;

    public TextMeshProUGUI state;
    public TextMeshProUGUI usageButton;

    public static event Action<bool, bool> InventoryGUIEnterEvent;
    public static event Action InventoryGUILeaveEvent;

    private UnifiedGUI unifiedGUI;
    private int currentItemIndex = 0;
    private ItemData currentItemData;
    private Vector3 rotationSpeed = Vector3.zero;

    private GameObject inspectedItem = null;

    private static InventoryGUI instance;
    public static InventoryGUI Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        unifiedGUI = GameObject.FindGameObjectWithTag("UnifiedGUI").GetComponent<UnifiedGUI>();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetButtonDown("Inventory") || Input.GetButtonDown("Cancel")) && isVisible)
        {
            InventoryGUILeaveEvent?.Invoke();
            DestroyInspectedItem();
            rotationSpeed = Vector3.zero;
            isVisible = false;
            Hide();
        }
        else if (Input.GetButtonDown("Inventory") && !isVisible && !unifiedGUI.IsAnyGUIVisible())
        {
            InventoryGUIEnterEvent?.Invoke(true, true);
            isVisible = true;
            currentItemIndex = 0;
            PlayerInventory.Instance.printItems();
            updateUI();
            Show();
        }

        if ((Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.LeftArrow)) && isVisible)
        {
            if (leftArrow.color == Color.white)
            {
                currentItemIndex -= 1;
                updateUI();
            }
        }
        else if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.RightArrow)) && isVisible)
        {
            if (rightArrow.color == Color.white)
            {
                currentItemIndex += 1;
                updateUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && isVisible && usageButton.text == "[E]quip")
        {
            InventorySlot slot = PlayerInventory.Instance.inventoryData.slots[currentItemIndex];
            currentItemData = slot.itemData;
            if (usageButton.color == Color.white)
            {
                PlayerInventory.Instance.EquipItem(currentItemData);
            }
            else if (usageButton.color == Color.green)
            {
                PlayerInventory.Instance.UnequipItem();
            }
            updateUI();

        }
        else if (Input.GetKeyDown(KeyCode.U) && isVisible && usageButton.text == "[U]se")
        {
            InventorySlot slot = PlayerInventory.Instance.inventoryData.slots[currentItemIndex];
            currentItemData = slot.itemData;
            if (usageButton.color == Color.white)
            {
                PlayerInventory.Instance.UseItem(currentItemData);
                if (!PlayerInventory.Instance.itemExists(currentItemData))
                {
                    currentItemIndex = 0;
                }
                updateUI();
            }
        }
    }

    void Show()
    {
        itemNameText.enabled = true;
        itemDescriptionText.enabled = true;
        leftArrow.enabled = true;
        rightArrow.enabled = true;
        state.enabled = true;
        usageButton.enabled = true;

        // TODO
        InteractGUI.Instance.Hide();
    }

    void Hide()
    {
        itemNameText.enabled = false;
        itemDescriptionText.enabled = false;
        leftArrow.enabled = false;
        rightArrow.enabled = false;
        state.enabled = false;
        usageButton.enabled = false;
    }

    void updateUI()
    {
        if (PlayerInventory.Instance.inventoryData.slots.Count == 0)
        {
            itemNameText.text = "No item in inventory.";
            itemDescriptionText.text = "";
            leftArrow.color = Color.grey;
            rightArrow.color = Color.grey;
            state.text = "";
            usageButton.text = "";
            DestroyInspectedItem();

        }
        else
        {
            if (currentItemIndex >= 0 && (currentItemIndex + 1) < PlayerInventory.Instance.inventoryData.slots.Count)
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

            InventorySlot slot = PlayerInventory.Instance.inventoryData.slots[currentItemIndex];
            currentItemData = slot.itemData;
            ViewCurrentItem(currentItemData, slot.amount);
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

    private void ViewCurrentItem(ItemData currentItemData, uint amount)
    {
        itemNameText.text = currentItemData.itemName + "(" + amount + ")";
        itemDescriptionText.text = currentItemData.interactText;

        DestroyInspectedItem();

        inspectedItem = Instantiate(currentItemData.prefab);
        MeshRenderer meshRenderer = inspectedItem.GetComponent<MeshRenderer>();
        meshRenderer.material = currentItemData.inspectMaterial;
        for (int i = 0; i < meshRenderer.materials.Length; i++) {
            meshRenderer.materials[i] = currentItemData.inspectMaterial;
        }
        inspectedItem.layer = LayerMask.NameToLayer("UI");
        inspectedItem.transform.position = guiCamera.transform.position + guiCamera.transform.forward * 1.0f;
        inspectedItem.transform.rotation = Quaternion.LookRotation(inspectedItem.transform.position - guiCamera.transform.position, Vector3.up) * Quaternion.Euler(90, 0, 0);
    }

    private void ShowItemUsage(ItemData currentItemData)
    {
        if (PlayerInventory.Instance.inventoryData.equippedItem == currentItemData)
        {
            state.text = "*equipped";
            usageButton.text = "[E]quip";
            usageButton.color = Color.green;
        }
        else if (currentItemData.category == ItemCategory.Weapon)
        {
            state.text = "";
            usageButton.text = "[E]quip";
            usageButton.color = Color.white;
        }
        else if (currentItemData.category == ItemCategory.Resource)
        {
            state.text = "";
            usageButton.text = "[U]se";
            if (currentItemData.itemName == "Battery" && PlayerInventory.Instance.inventoryData.equippedItem == null)
            {
                usageButton.color = Color.grey;
            }
            else
            {
                usageButton.color = Color.white;
            }


        }
        else if (currentItemData.category == ItemCategory.Normal)
        {
            state.text = "";
            usageButton.text = "";
            usageButton.color = Color.white;
        }
        else if (currentItemData.category == ItemCategory.CarPart)
        {
            state.text = "";
            usageButton.text = "";
            usageButton.color = Color.white;
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

    public bool IsVisible()
    {
        return false;
    }
}