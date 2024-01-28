using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The flashlight indicator gui is used to diplay the current battery life of an equipped flashlight.
/// The flashlight indicator gui is a singleton.
/// </summary>
public class FlashlightIndicatorGUI : MonoBehaviour {
    public Sprite[] indicatorSprites;
    private Image indicatorImage;
    private PlayerFlashlightManager playerFlashlightManager;

    private PlayerInventory playerInventory = null;

    private static FlashlightIndicatorGUI instance;
    public static FlashlightIndicatorGUI Instance { get { return instance; } }

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    void Start() {
        indicatorImage = GetComponent<Image>();
        playerFlashlightManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerFlashlightManager>();
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
    }

    void Update() {
        if (playerInventory.GetInventory().GetEquippedItem()?.GetItemCategory() == ItemCategory.FLASHLIGHT) {
            indicatorImage.enabled = true;
            indicatorImage.sprite = indicatorSprites[Mathf.RoundToInt((playerFlashlightManager.GetBatteryLife()) + 24) / 25];
        } else {
            indicatorImage.enabled = false;
        }
    }
}