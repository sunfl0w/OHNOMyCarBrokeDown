
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Custom render pass to generate a low resolution output image.
/// Ignore the warnings. Unity changes the backend code all the time and deprecates functions from time to time.
/// If the pass does produce a black image, try another graphics API.
/// It seems like OpenGL and custom passes don't like each other.
/// </summary>
public class LowResPass : ScriptableRenderPass {
    private LowResFeature.CustomPassSettings settings;

    private RenderTargetIdentifier colorBuffer, pixelBuffer;
    private int pixelBufferID = Shader.PropertyToID("_PixelBuffer");

    private Material material;
    private int pixelScreenHeight, pixelScreenWidth;

    public LowResPass(LowResFeature.CustomPassSettings settings) {
        this.settings = settings;
        this.renderPassEvent = settings.renderPassEvent;
        if (material == null) {
            material = CoreUtils.CreateEngineMaterial("Custom/Pixelize");
        }
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
        colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

        pixelScreenHeight = settings.screenHeight;
        pixelScreenWidth = (int)Mathf.Ceil(pixelScreenHeight * renderingData.cameraData.camera.aspect);
        descriptor.height = pixelScreenHeight;
        descriptor.width = pixelScreenWidth;

        cmd.GetTemporaryRT(pixelBufferID, descriptor, FilterMode.Point);
        pixelBuffer = new RenderTargetIdentifier(pixelBufferID);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
        CommandBuffer cmd = CommandBufferPool.Get();
        Blit(cmd, colorBuffer, pixelBuffer, material);
        Blit(cmd, pixelBuffer, colorBuffer);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd) {
        if (cmd == null) {
            throw new System.ArgumentNullException("cmd");
        }
        cmd.ReleaseTemporaryRT(pixelBufferID);
    }
}
