using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Custom render feature to generate a low resolution output image
/// </summary>
public class LowResFeature : ScriptableRendererFeature {
    [System.Serializable]
    public class CustomPassSettings {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public int screenHeight = 240;
    }

    [SerializeField] private CustomPassSettings settings;
    private LowResPass customPass;

    public override void Create() {
        customPass = new LowResPass(settings);
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {

#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera) return;
#endif
        renderer.EnqueuePass(customPass);
    }
}


