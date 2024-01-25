using UnityEngine;
using UnityEngine.UI;

public class FlashlightIndicatorGUI : MonoBehaviour {
    public Sprite[] indicatorSprites;

    private Image indicatorImage;
    private PlayerFlashlightManager playerFlashlightManager;

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
    }

    void Update() {
        if (PlayerInventory.Instance.IsFlashlightEquipped()) {
            indicatorImage.enabled = true;
            indicatorImage.sprite = indicatorSprites[Mathf.RoundToInt((playerFlashlightManager.GetBatteryLife()) + 24) / 25];
        } else {
            indicatorImage.enabled = false;
        }
    }
}