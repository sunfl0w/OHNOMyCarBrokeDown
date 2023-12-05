using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RenderTextureBlitter : MonoBehaviour {
    public RenderTexture tex;
    void Start() {
        RenderPipelineManager.endContextRendering += OnEndContextRendering;
    }

    void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras) {
        // Put the code that you want to execute at the end of RenderPipeline.Render here
        Graphics.Blit(tex, (RenderTexture)null);
    }

    void OnDestroy() {
        RenderPipelineManager.endContextRendering -= OnEndContextRendering;
    }
}
