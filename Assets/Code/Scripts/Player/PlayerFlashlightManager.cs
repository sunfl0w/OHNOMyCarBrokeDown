using UnityEngine;
using System.Collections;

/// <summary>
/// The player flashlight manager manages flashlight battery life and depletion.
/// </summary>
public class PlayerFlashlightManager : MonoBehaviour {
    /// <summary>
    /// Initial battery life in percent.
    /// </summary>
    public float batteryLife = 100f;

    /// <summary>
    /// Rate at which battery depletes per second.
    /// </summary>
    public float depletionRate = 1f;

    /// <summary>
    /// Reference to flashlight light.
    /// </summary>
    public Light flashlight;

    /// <summary>
    /// Reference to light used by gui.
    /// </summary>
    public Light uiLight;

    /// <summary>
    /// Flag specifying, whether the flashlight is currently active.
    /// </summary>
    private bool flashlightActive = false;

    /// <summary>
    /// Specifies, whether the flashlight is flickering currently.
    /// </summary>
    private bool isFlickering = false;

    /// <summary>
    /// Original intensity of the flashlight light.
    /// </summary>
    private float flashlightOriginalIntensity = 0.0f;

    /// <summary>
    /// Original intensity of the gui light.
    /// </summary>
    private float uiLightoriginalIntensity = 0.0f;

    void Start() {
        flashlightOriginalIntensity = flashlight.intensity;
        uiLightoriginalIntensity = uiLight.intensity;

        if (SavestateManager.Instance.GetCurrentSaveState().playerSaveState.initialized) {
            float loadedBatteryLife = SavestateManager.Instance.GetCurrentSaveState().playerSaveState.flashlightBatteryLife;
            Debug.Log("Loaded flashlight battery life of " + loadedBatteryLife + " from current save state");
            batteryLife = loadedBatteryLife;
        }
    }

    void Update() {
        if (flashlightActive && PlayerInventory.Instance.IsFlashlightEquipped()) {
            batteryLife -= Mathf.Max(depletionRate * Time.deltaTime, 0.0f); // Deplete battery

            if (Input.GetButtonDown("Use")) {
                flashlightActive = false;
            }
        } else if (!flashlightActive && PlayerInventory.Instance.IsFlashlightEquipped()) {
            if (Input.GetButtonDown("Use")) {
                flashlightActive = true;
            }
        }

        if (flashlightActive && PlayerInventory.Instance.IsFlashlightEquipped() && batteryLife > 20.0f) { // Normal flashlight behaviour
            isFlickering = false;
            StopAllCoroutines();
            flashlight.intensity = flashlightOriginalIntensity;
            uiLight.intensity = uiLightoriginalIntensity;
        } else if (flashlightActive && PlayerInventory.Instance.IsFlashlightEquipped() && batteryLife <= 0.0f) { // Flashlight off behaviour
            flashlightActive = false;
        } else if (flashlightActive && PlayerInventory.Instance.IsFlashlightEquipped() && batteryLife <= 20.0f) { // Flashlight flickering behaviour
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
