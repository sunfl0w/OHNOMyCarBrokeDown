using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// The ppfx manager adjusts the current volume's profile based on opened guis.
/// The manager also allows for global brightness adjustements and a brightness slider in the settings.
/// The ppfx manager is a singleton.
/// </summary>
public class PPFXManager : MonoBehaviour {
    /// <summary>
    /// Reference to the currently used volume.
    /// </summary>
    public Volume volume;

    /// <summary>
    /// Base exposure value.
    /// </summary>
    public float baseExposure = 0.0f;

    private static PPFXManager instance;
    public static PPFXManager Instance { get { return instance; } }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }

        UnifiedGUI.GUIEnterEvent += OnGUIEnter;
        UnifiedGUI.GUILeaveEvent += OnGUILeave;
    }

    void Start() {
        DepthOfField df;
        volume.profile.TryGet<DepthOfField>(out df);
        df.mode.Override(DepthOfFieldMode.Off);
        UpdateBrightness();
    }

    /// <summary>
    /// Enables volume blur based on whether the opened gui demands it.
    /// </summary>
    void OnGUIEnter(bool enableGUIBlur, bool disableMovemen) {
        if (enableGUIBlur) {
            DepthOfField df;
            volume.profile.TryGet<DepthOfField>(out df);
            df.mode.Override(DepthOfFieldMode.Gaussian);

            ColorAdjustments ca;
            volume.profile.TryGet<ColorAdjustments>(out ca);
            ca.postExposure.Override(baseExposure + (PlayerPrefs.GetFloat("Brightness") - 0.5f) * 4.0f - 1.5f);
        }
    }

    /// <summary>
    /// Disables volume blur based on whether the opened gui demands it.
    /// </summary>
    void OnGUILeave() {
        DepthOfField df;
        volume.profile.TryGet<DepthOfField>(out df);
        df.mode.Override(DepthOfFieldMode.Off);
        ColorAdjustments ca;
        volume.profile.TryGet<ColorAdjustments>(out ca);
        ca.postExposure.Override(baseExposure + (PlayerPrefs.GetFloat("Brightness") - 0.5f) * 4.0f);
    }

    /// <summary>
    /// Updates volume brightness based on player settings.
    /// </summary>
    public void UpdateBrightness() {
        ColorAdjustments ca;
        volume.profile.TryGet<ColorAdjustments>(out ca);
        ca.postExposure.Override(baseExposure + (PlayerPrefs.GetFloat("Brightness") - 0.5f) * 4.0f);
    }

    /// <summary>
    /// Directly updates volume brightness based on provided brightness value.
    /// </summary>
    public void UpdateBrightnessDirect(float brightness) {
        ColorAdjustments ca;
        volume.profile.TryGet<ColorAdjustments>(out ca);
        ca.postExposure.Override(baseExposure + (brightness - 0.5f) * 4.0f);
    }
}