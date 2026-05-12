using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OutlineEffectRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Color outlineColor = Color.black;

        [Range(0f, 5f)]
        public float thickness = 1f;

        [Range(0f, 1f)]
        public float depthThreshold = 0.01f;

        [Range(0f, 1f)]
        public float normalThreshold = 0.4f;

        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public Settings settings = new Settings();
    OutlineEffectPass _pass;

    public override void Create()
    {
        _pass = new OutlineEffectPass(settings)
        {
            renderPassEvent = settings.renderPassEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);
    }
}
