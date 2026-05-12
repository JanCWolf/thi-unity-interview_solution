using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class OutlineEffectPass : ScriptableRenderPass
{
    private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");
    private static readonly int ThicknessId = Shader.PropertyToID("_Thickness");
    private static readonly int DepthThresholdId = Shader.PropertyToID("_DepthThreshold");
    private static readonly int NormalThresholdId = Shader.PropertyToID("_NormalThreshold");

    const string m_PassName = "OutlineEffectPass";
    Material _material;

    readonly OutlineEffectRenderFeature.Settings _settings;

    public OutlineEffectPass(OutlineEffectRenderFeature.Settings settings)
    {
        _settings = settings;

        // loading the Outline shader via Shader.Find and creating the material.
        // shader is always include in builds
        var shader = Shader.Find("Interview/OutlineEffect");
        if (shader != null)
        {
            _material = CoreUtils.CreateEngineMaterial(shader);
        }
        else
        {
            Debug.LogError("Could not find Outline shader");
        }

        requiresIntermediateTexture = true;
    }


    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        _material.SetColor(OutlineColorId, _settings.outlineColor);
        _material.SetFloat(ThicknessId, _settings.thickness);
        _material.SetFloat(DepthThresholdId, _settings.depthThreshold);
        _material.SetFloat(NormalThresholdId, _settings.normalThreshold);

        var resourceData = frameData.Get<UniversalResourceData>();

        if (resourceData.isActiveTargetBackBuffer)
        {
            Debug.LogError($"Skipping render pass. OutlineEffectRendererFeature requires an intermediate ColorTexture, we can't use the BackBuffer as a texture input.");
            return;
        }

        var source = resourceData.activeColorTexture;
        var destinationDesc = renderGraph.GetTextureDesc(source);
        destinationDesc.name = $"CameraColor-{m_PassName}";
        destinationDesc.clearBuffer = false;

        TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

        RenderGraphUtils.BlitMaterialParameters para = new(source, destination, _material, 0);
        renderGraph.AddBlitPass(para, passName: m_PassName);


        resourceData.cameraColor = destination;
    }
}
