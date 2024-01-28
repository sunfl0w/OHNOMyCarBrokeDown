using UnityEngine;
using System.Collections;

/// <summary>
/// The player flashlight manager manages flashlight battery life and depletion.
/// </summary>
public class PlayerFlashlightManager : MonoBehaviour {
    public float batteryLife = 100f;
    public float depletionRate = 1f;
    public Light flashlight;
    public Light uiLight;

    private bool flashlightActive = false;
    private bool isFlickering = false;
    private float flashlightOriginalIntensity = 0.0f;
    private float uiLightoriginalIntensity = 0.0f;
    private PlayerInventory playerInventory = null;

    void Start() {
        flashlightOriginalIntensity = flashlight.intensity;
        uiLightoriginalIntensity = uiLight.intensity;
        playerInventory = playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();

        if (SavestateManager.Instance.GetCurrentSaveState().playerSaveState.initialized) {
            float loadedBatteryLife = SavestateManager.Instance.GetCurrentSaveState().playerSaveState.flashlightBatteryLife;
            Debug.Log("Loaded flashlight battery life of " + loadedBatteryLife + " from current save state");
            batteryLife = loadedBatteryLife;
        }
    }

    void Update() {
        if (flashlightActive && playerInventory.GetInventory().GetEquippedItem()?.GetItemCategory() == ItemCategory.FLASHLIGHT) {
            batteryLife -= Mathf.Max(depletionRate * Time.deltaTime, 0.0f); // Deplete battery

            if (Input.GetButtonDown("Use")) {
                flashlightActive = false;
            }
        } else if (!flashlightActive && playerInventory.GetInventory().GetEquippedItem()?.GetItemCategory() == ItemCategory.FLASHLIGHT) {
            if (Input.GetButtonDown("Use")) {
                flashlightActive = true;
            }
        }

        if (flashlightActive && playerInventory.GetInventory().GetEquippedItem()?.GetItemCategory() == ItemCategory.FLASHLIGHT && batteryLife > 20.0f) { // Normal flashlight behaviour
            isFlickering = false;
            StopAllCoroutines();
            flashlight.intensity = flashlightOriginalIntensity;
            uiLight.intensity = uiLightoriginalIntensity;
        } else if (flashlightActive && playerInventory.GetInventory().GetEquippedItem()?.GetItemCategory() == ItemCategory.FLASHLIGHT && batteryLife <= 0.0f) { // Flashlight off behaviour
            flashlightActive = false;
        } else if (flashlightActive && playerInventory.GetInventory().GetEquippedItem()?.GetItemCategory() == ItemCategory.FLASHLIGHT && batteryLife <= 20.0f) { // Flashlight flickering behaviour
            if (!isFlickering) {
                isFlickering = true;
                StartCoroutine(Flicker());
            }
        }

        flashlight.enabled = flashlightActive;
        uiLight.enabled = flashlightActive;
    }

    /// <summary>
    /// Coroutine used to make the flashlight flicker randomly when it is close to being empty.
    /// </summary>
    private IEnumerator Flicker() {
        while (true) {
            float rand = Random.Range(0.0f, 0.8f);
            flashlight.intensity = rand * flashlightOriginalIntensity;
            uiLight.intensity = rand * uiLightoriginalIntensity; // Also flicker ui light in inventory and inspection GUIs as a nice effect
            yield return new WaitForSeconds(Random.Range(1.0f, 3.5f)); // Adjust the duration between flickers
        }
    }

    /// <summary>
    /// Returns the current battery life percentage.
    /// </summary>
    public float GetBatteryLife() {
        return batteryLife;
    }

    /// <summary>
    /// Returns true if the flashlight is currently active.
    /// </summary>
    public bool IsFlashlightActive() {
        return flashlightActive;
    }
}
