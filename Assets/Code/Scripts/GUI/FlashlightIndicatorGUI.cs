using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The flashlight indicator gui is used to diplay the current battery life of an equipped flashlight.
/// The flashlight indicator gui is a singleton.
/// </summary>
public class FlashlightIndicatorGUI : MonoBehaviour {
    /// <summary>
    /// Sprites used to indicate battery life.
    /// </summary>
    public Sprite[] indicatorSprites;

    /// <summary>
    /// Gui image used to display current flashlight battery life.
    /// </summary>
    private Image indicatorImage;

    /// <summary>
    /// Reference to the player flashlight manager.
    /// </summary>
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