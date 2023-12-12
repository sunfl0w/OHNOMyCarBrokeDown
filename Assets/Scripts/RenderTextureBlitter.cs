using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RenderTextureBlitter : MonoBehaviour {
    public RenderTexture tex;
    void Start() {
        RenderPipelineManager.endContextRendering += OnEndContextRendering;
    }

    void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras) {
        Graphics.Blit(tex, (RenderTexture)null);
    }

    void OnDestroy() {
        RenderPipelineManager.endContextRendering -= OnEndContextRendering;
    }
}
