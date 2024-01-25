using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PPFXManager : MonoBehaviour {
    public Volume volume;
    public float baseExposure = 0.0f;

    private static PPFXManager instance;
    public static PPFXManager Instance { get { return instance; } }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }

        UnifiedGUI.GUIEnterEvent += SetBlurProfile;
        UnifiedGUI.GUILeaveEvent += SetBaseProfile;
    }

    void Start() {
        DepthOfField df;
        volume.profile.TryGet<DepthOfField>(out df);
        df.mode.Override(DepthOfFieldMode.Off);
        UpdateBrightness();
    }

    void SetBlurProfile(bool enableGUIBlur, bool disableMovemen) {
        if (enableGUIBlur) {
            DepthOfField df;
            volume.profile.TryGet<DepthOfField>(out df);
            df.mode.Override(DepthOfFieldMode.Gaussian);

            ColorAdjustments ca;
            volume.profile.TryGet<ColorAdjustments>(out ca);
            ca.postExposure.Override(baseExposure + (PlayerPrefs.GetFloat("Brightness") - 0.5f) * 4.0f - 1.5f);
        }
    }

    void SetBaseProfile() {
        DepthOfField df;
        volume.profile.TryGet<DepthOfField>(out df);
        df.mode.Override(DepthOfFieldMode.Off);
        ColorAdjustments ca;
        volume.profile.TryGet<ColorAdjustments>(out ca);
        ca.postExposure.Override(baseExposure + (PlayerPrefs.GetFloat("Brightness") - 0.5f) * 4.0f);
    }

    public void UpdateBrightness() {
        ColorAdjustments ca;
        volume.profile.TryGet<ColorAdjustments>(out ca);
        ca.postExposure.Override(baseExposure + (PlayerPrefs.GetFloat("Brightness") - 0.5f) * 4.0f);
    }

    public void UpdateBrightnessDirect(float brightness) {
        ColorAdjustments ca;
        volume.profile.TryGet<ColorAdjustments>(out ca);
        ca.postExposure.Override(baseExposure + (brightness - 0.5f) * 4.0f);
    }
}