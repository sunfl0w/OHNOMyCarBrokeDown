using UnityEngine;
using TMPro;
using System;

/// <summary>
/// The inventory displays the player character's inventory and the equipped item.
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
    private PlayerInventory playerInventory = null;

    void Start() {
        unifiedGUI = GameObject.FindGameObjectWithTag("UnifiedGUI").GetComponent<UnifiedGUI>();
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        Hide();
    }

    void Update() {
        if ((Input.GetButtonDown("Inventory") || Input.GetButtonDown("Cancel")) && isVisible) {
            InventoryGUILeaveEvent?.Invoke();
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
            if (currentItemIndex > 0) {
                currentItemIndex -= 1;
                UpdateUI();
            }
        } else if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.RightArrow)) && isVisible) {
            if (currentItemIndex < playerInventory.GetInventory().GetSlotCount() - 1) {
                currentItemIndex += 1;
                UpdateUI();
            }
        }

        currentItem = playerInventory.GetInventory().GetItemBySlotIndex(currentItemIndex);

        if (Input.GetKeyDown(KeyCode.E) && isVisible && currentItem.Equipable()) {
            if (playerInventory.GetInventory().GetEquippedItem().GetName() == currentItem.GetName()) { // Unequip if item is already equipped
                playerInventory.GetInventory().UnequipItem();
            } else {
                playerInventory.GetInventory().EquipItem(currentItem.GetName());
            }
            UpdateUI();

        } else if (Input.GetKeyDown(KeyCode.U) && isVisible && currentItem.Usable()) {
            currentItem.Use();
            if (!playerInventory.GetInventory().ItemExists(currentItem.GetName())) {
                currentItemIndex = Math.Max(currentItemIndex - 1, 0);
            }
            UpdateUI();
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
        //InteractGUI.Instance.Hide();
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
            //DestroyInspectedItem();

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

    private void ViewCurrentItem(Item currentItem, uint amount) {
        itemNameText.text = currentItem.GetName() + "(" + amount + ")";
        itemDescriptionText.text = currentItem.GetInteractText();

        // Show item inspect gui
    }

    private void ShowItemUsage(Item currentItem) {
        state.text = "";
        usageButton.text = "";
        usageButton.color = Color.white;

        Item equippedItem = playerInventory.GetInventory().GetEquippedItem();
        if (equippedItem != null && equippedItem.GetName() == currentItem.GetName()) {
            state.text = "Equipped";
            usageButton.text = "Unequip [E]";
            usageButton.color = Color.green;
        } else if (currentItem.Equipable()) {
            state.text = "";
            usageButton.text = "Equip [E]";
            usageButton.color = Color.white;
        } else if (currentItem.Usable()) {
            state.text = "";
            usageButton.text = "Use [U]";
            usageButton.color = Color.white;
            if (currentItem.GetItemCategory() == ItemCategory.BATTERY && !playerInventory.GetInventory().IsFlashlightEquipped()) {
                usageButton.color = Color.grey;
            }
        }
    }

    public bool IsVisible() {
        return false;
    }
}