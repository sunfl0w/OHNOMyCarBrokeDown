using UnityEngine;
using TMPro;
using System;

/// <summary>
/// The inventory displays the player character's inventory and the equipped item.
/// The inventory gui is a singleton.
/// </summary>
public class InventoryGUI : MonoBehaviour {
    public Camera guiCamera;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI leftArrow;
    public TextMeshProUGUI rightArrow;
    public TextMeshProUGUI state;
    public TextMeshProUGUI usageButton;

    public static event Action<bool, bool> InventoryGUIEnterEvent;
    public static event Action InventoryGUILeaveEvent;

    private bool isVisible = false;
    private UnifiedGUI unifiedGUI;
    private int currentItemIndex = 0;
    private Item currentItem;
    private Vector3 rotationSpeed = Vector3.zero;
    private GameObject inspectedItem = null;
    private PlayerInventory playerInventory = null;

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
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
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
            currentItem = playerInventory.GetInventory().GetItemBySlotIndex(currentItemIndex);
            if (usageButton.color == Color.white) {
                playerInventory.GetInventory().EquipItem(currentItem.GetName());
            } else if (usageButton.color == Color.green) {
                playerInventory.GetInventory().UnequipItem();
            }
            UpdateUI();

        } else if (Input.GetKeyDown(KeyCode.U) && isVisible && usageButton.text == "[U]se") {
            currentItem = playerInventory.GetInventory().GetItemBySlotIndex(currentItemIndex);
            if (usageButton.color == Color.white) {
                currentItem.Use();
                if (!playerInventory.GetInventory().ItemExists(currentItem.GetName())) {
                    currentItemIndex = 0;
                }
                UpdateUI();
            }
        }
    }

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

    void Hide() {
        itemNameText.enabled = false;
        itemDescriptionText.enabled = false;
        leftArrow.enabled = false;
        rightArrow.enabled = false;
        state.enabled = false;
        usageButton.enabled = false;
    }

    void UpdateUI() {
        if (playerInventory.GetInventory().IsEmpty()) {
            itemNameText.text = "No item in inventory.";
            itemDescriptionText.text = "";
            leftArrow.color = Color.grey;
            rightArrow.color = Color.grey;
            state.text = "";
            usageButton.text = "";
            DestroyInspectedItem();

        } else {
            if (currentItemIndex >= 0 && (currentItemIndex + 1) < playerInventory.GetInventory().GetSlotCount()) {
                rightArrow.color = Color.white;
            } else {
                rightArrow.color = Color.grey;
            }
            if (currentItemIndex != 0) {
                leftArrow.color = Color.white;
            } else {
                leftArrow.color = Color.grey;
            }

            Slot slot = playerInventory.GetInventory().GetSlotByIndex(currentItemIndex);
            currentItem = slot.GetItem();
            ViewCurrentItem(currentItem, slot.GetAmount());
            ShowItemUsage(currentItem);

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

    private void ViewCurrentItem(Item currentItem, uint amount) {
        itemNameText.text = currentItem.GetName() + "(" + amount + ")";
        itemDescriptionText.text = currentItem.GetInteractText();

        DestroyInspectedItem();

        inspectedItem = Instantiate(currentItem.GetPrefab());
        // TODO
        /*MeshRenderer meshRenderer = inspectedItem.GetComponent<MeshRenderer>();
        meshRenderer.material = currentItemData.inspectMaterial;
        for (int i = 0; i < meshRenderer.materials.Length; i++) {
            meshRenderer.materials[i] = currentItemData.inspectMaterial;
        }*/
        inspectedItem.layer = LayerMask.NameToLayer("UI");
        inspectedItem.transform.position = guiCamera.transform.position + guiCamera.transform.forward * 1.0f;
        inspectedItem.transform.rotation = Quaternion.LookRotation(inspectedItem.transform.position - guiCamera.transform.position, Vector3.up) * Quaternion.Euler(90, 0, 0);
    }

    private void ShowItemUsage(Item currentItem) {
        Item equippedItem = playerInventory.GetInventory().GetEquippedItem();
        if (equippedItem != null && equippedItem.GetName() == currentItem.GetName()) {
            state.text = "*equipped";
            usageButton.text = "[E]quip";
            usageButton.color = Color.green;
        } else if (currentItem.GetItemCategory() == ItemCategory.FLASHLIGHT) {
            state.text = "";
            usageButton.text = "[E]quip";
            usageButton.color = Color.white;
        } else if (currentItem.GetItemCategory() == ItemCategory.FOOD || currentItem.GetItemCategory() == ItemCategory.BATTERY) {
            state.text = "";
            usageButton.text = "[U]se";
            if (currentItem.GetName() == "Battery" && playerInventory.GetInventory().GetEquippedItem() == null) {
                usageButton.color = Color.grey;
            } else {
                usageButton.color = Color.white;
            }


        } else {
            state.text = "";
            usageButton.text = "";
            usageButton.color = Color.white;
        }
    }

    private void DestroyInspectedItem() {
        if (inspectedItem != null) {
            Destroy(inspectedItem.gameObject);
        }
        inspectedItem = null;
    }

    public bool IsVisible() {
        return false;
    }
}