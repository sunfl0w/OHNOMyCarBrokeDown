using UnityEngine;
using TMPro;
using System;

/// <summary>
/// The inventory displays the player character's inventory and the equipped item.
/// The inventory gui is a singleton.
/// </summary>
public class InventoryGUI : MonoBehaviour {
    /// <summary>
    /// Reference to the gui camera.
    /// </summary>
    public Camera guiCamera;

    /// <summary>
    /// Item name text.
    /// </summary>
    public TextMeshProUGUI itemNameText;

    /// <summary>
    /// Item description text.
    /// </summary>
    public TextMeshProUGUI itemDescriptionText;

    /// <summary>
    /// Inventory move left text.
    /// </summary>
    public TextMeshProUGUI leftArrow;

    /// <summary>
    /// Inventory move right text.
    /// </summary>
    public TextMeshProUGUI rightArrow;

    /// <summary>
    /// Item state text.
    /// </summary>
    public TextMeshProUGUI state;

    /// <summary>
    /// Item usage button.
    /// </summary>
    public TextMeshProUGUI usageButton;

    /// <summary>
    /// Inventory gui enter event.
    /// </summary>
    public static event Action<bool, bool> InventoryGUIEnterEvent;

    /// <summary>
    /// Inventory gui leave event.
    /// </summary>
    public static event Action InventoryGUILeaveEvent;

    /// <summary>
    /// Flag representing current gui visibility status.
    /// </summary>
    private bool isVisible = false;

    /// <summary>
    /// Reference to the unified gui.
    /// </summary>
    private UnifiedGUI unifiedGUI;

    /// <summary>
    /// Current item index.
    /// </summary>
    private int currentItemIndex = 0;

    /// <summary>
    /// Current item data.
    /// </summary>
    private ItemData currentItemData;

    /// <summary>
    /// Current inspected item rotation speed.
    /// </summary>
    private Vector3 rotationSpeed = Vector3.zero;

    /// <summary>
    /// Currently inspected item game object.
    /// </summary>
    private GameObject inspectedItem = null;

    private static InventoryGUI instance;
    public static InventoryGUI Instance { get { return instance; } }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    void Start() {
        unifiedGUI = GameObject.FindGameObjectWithTag("UnifiedGUI").GetComponent<UnifiedGUI>();
        Hide();
    }

    void Update() {
        if ((Input.GetButtonDown("Inventory") || Input.GetButtonDown("Cancel")) && isVisible) {
            InventoryGUILeaveEvent?.Invoke();
            DestroyInspectedItem();
            rotationSpeed = Vector3.zero;
            isVisible = false;
            Hide();
        } else if (Input.GetButtonDown("Inventory") && !isVisible && !unifiedGUI.IsAnyGUIVisible()) {
            InventoryGUIEnterEvent?.Invoke(true, true);
            isVisible = true;
            currentItemIndex = 0;
            UpdateUI();
            Show();
        }

        if ((Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.LeftArrow)) && isVisible) {
            if (leftArrow.color == Color.white) {
                currentItemIndex -= 1;
                UpdateUI();
            }
        } else if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.RightArrow)) && isVisible) {
            if (rightArrow.color == Color.white) {
                currentItemIndex += 1;
                UpdateUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && isVisible && usageButton.text == "[E]quip") {
            InventorySlot slot = PlayerInventory.Instance.inventoryData.slots[currentItemIndex];
            currentItemData = slot.itemData;
            if (usageButton.color == Color.white) {
                PlayerInventory.Instance.EquipItem(currentItemData);
            } else if (usageButton.color == Color.green) {
                PlayerInventory.Instance.UnequipItem();
            }
            UpdateUI();

        } else if (Input.GetKeyDown(KeyCode.U) && isVisible && usageButton.text == "[U]se") {
            InventorySlot slot = PlayerInventory.Instance.inventoryData.slots[currentItemIndex];
            currentItemData = slot.itemData;
            if (usageButton.color == Color.white) {
                PlayerInventory.Instance.UseItem(currentItemData);
                if (!PlayerInventory.Instance.CheckItemExists(currentItemData)) {
                    currentItemIndex = 0;
                }
                UpdateUI();
            }
        }
    }

    /// <summary>
    /// Show inventory gui.
    /// </summary>
    void Show() {
        itemNameText.enabled = true;
        itemDescriptionText.enabled = true;
        leftArrow.enabled = true;
        rightArrow.enabled = true;
        state.enabled = true;
        usageButton.enabled = true;

        // TODO
        InteractGUI.Instance.Hide();
    }

    /// <summary>
    /// Hide inventory gui.
    /// </summary>
    void Hide() {
        itemNameText.enabled = false;
        itemDescriptionText.enabled = false;
        leftArrow.enabled = false;
        rightArrow.enabled = false;
        state.enabled = false;
        usageButton.enabled = false;
    }

    /// <summary>
    /// Update inventory gui.
    /// </summary>
    void UpdateUI() {
        if (PlayerInventory.Instance.inventoryData.slots.Count == 0) {
            itemNameText.text = "No item in inventory.";
            itemDescriptionText.text = "";
            leftArrow.color = Color.grey;
            rightArrow.color = Color.grey;
            state.text = "";
            usageButton.text = "";
            DestroyInspectedItem();

        } else {
            if (currentItemIndex >= 0 && (currentItemIndex + 1) < PlayerInventory.Instance.inventoryData.slots.Count) {
                rightArrow.color = Color.white;
            } else {
                rightArrow.color = Color.grey;
            }
            if (currentItemIndex != 0) {
                leftArrow.color = Color.white;
            } else {
                leftArrow.color = Color.grey;
            }

            InventorySlot slot = PlayerInventory.Instance.inventoryData.slots[currentItemIndex];
            currentItemData = slot.itemData;
            ViewCurrentItem(currentItemData, slot.amount);
            ShowItemUsage(currentItemData);

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
    /// Inventory gui view current item.
    /// </summary>
    private void ViewCurrentItem(ItemData currentItemData, uint amount) {
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

    /// <summary>
    /// Inventory gui display current item usage information.
    /// </summary>
    private void ShowItemUsage(ItemData currentItemData) {
        if (PlayerInventory.Instance.inventoryData.equippedItem == currentItemData) {
            state.text = "*equipped";
            usageButton.text = "[E]quip";
            usageButton.color = Color.green;
        } else if (currentItemData.category == ItemCategory.Weapon) {
            state.text = "";
            usageButton.text = "[E]quip";
            usageButton.color = Color.white;
        } else if (currentItemData.category == ItemCategory.Resource) {
            state.text = "";
            usageButton.text = "[U]se";
            if (currentItemData.itemName == "Battery" && PlayerInventory.Instance.inventoryData.equippedItem == null) {
                usageButton.color = Color.grey;
            } else {
                usageButton.color = Color.white;
            }


        } else if (currentItemData.category == ItemCategory.Normal) {
            state.text = "";
            usageButton.text = "";
            usageButton.color = Color.white;
        } else if (currentItemData.category == ItemCategory.CarPart) {
            state.text = "";
            usageButton.text = "";
            usageButton.color = Color.white;
        }
    }

    /// <summary>
    /// Destroy currently inspected item object.
    /// </summary>
    private void DestroyInspectedItem() {
        if (inspectedItem != null) {
            Destroy(inspectedItem.gameObject);
        }
        inspectedItem = null;
    }

    /// <summary>
    /// Returns true if the inventory gui is visible.
    /// </summary>
    public bool IsVisible() {
        return false;
    }
}