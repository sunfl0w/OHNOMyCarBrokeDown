using UnityEngine;
using System.Collections;

public class PlayerFlashlightManager : MonoBehaviour {
    public float batteryLife = 100f; // Initial battery life in percentage
    public float depletionRate = 1f; // Rate at which battery depletes per second
    public Light flashlight;
    public Light uiLight;

    private bool flashlightActive = false;
    private bool isFlickering = false;
    private float flashlightOriginalIntensity = 0.0f;
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
        if(flashlightActive && PlayerInventory.Instance.IsFlashlightEquipped()) {
            batteryLife -= Mathf.Max(depletionRate * Time.deltaTime, 0.0f);

            if(Input.GetButtonDown("Use")) {
                flashlightActive = false;
            }
        } else if(!flashlightActive && PlayerInventory.Instance.IsFlashlightEquipped()) {
            if(Input.GetButtonDown("Use")) {
                flashlightActive = true;
            }
        }

        if(flashlightActive && PlayerInventory.Instance.IsFlashlightEquipped() && batteryLife > 20.0f) {
            isFlickering = false;
            StopAllCoroutines();
            flashlight.intensity = flashlightOriginalIntensity;
            uiLight.intensity = uiLightoriginalIntensity;
        } else if(flashlightActive && PlayerInventory.Instance.IsFlashlightEquipped() && batteryLife <= 0.0f) {
            flashlightActive = false;
        } else if(flashlightActive && PlayerInventory.Instance.IsFlashlightEquipped() && batteryLife <= 20.0f) {
            if (!isFlickering) {
                isFlickering = true;
                StartCoroutine(Flicker());
            }
        }

        flashlight.enabled = flashlightActive;
        uiLight.enabled = flashlightActive;
    }

    private IEnumerator Flicker() {
        while(true) {
            float rand = Random.Range(0.0f, 0.8f);
            flashlight.intensity = rand * flashlightOriginalIntensity;
            uiLight.intensity = rand * uiLightoriginalIntensity; // Also flicker ui light in inventory and inspection GUIs as a nice effect
            yield return new WaitForSeconds(Random.Range(1.0f, 3.5f)); // Adjust the duration between flickers
        }
    }

    public float GetBatteryLife() {
        return batteryLife;
    }

    public bool IsFlashlightActive() {
        return flashlightActive;
    }
}