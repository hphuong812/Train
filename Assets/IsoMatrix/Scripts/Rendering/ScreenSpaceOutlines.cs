using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenSpaceOutlines : ScriptableRendererFeature
{
    [System.Serializable] private class ViewSpaceNormalsTextureSettings
    {
        public RenderTextureFormat colorFormat;
        public int depthBufferBits;
        public FilterMode filterMode;
        public Color backgroundColor;
    }
    private class ViewSpaceNormalsTexturePass: ScriptableRenderPass
    {
        private readonly RenderTargetHandle normals;
        private readonly List<ShaderTagId> _shaderTagIdList;
        private readonly Material normalsMaterial;
        private ViewSpaceNormalsTextureSettings _normalsTextureSettings;
        private FilteringSettings _filteringSettings;
        public ViewSpaceNormalsTexturePass(RenderPassEvent renderPassEvent, LayerMask outLineLayerMask, ViewSpaceNormalsTextureSettings settings)
        {
            _shaderTagIdList = new List<ShaderTagId>
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefaultUnlit")
            };
            this.renderPassEvent = renderPassEvent;
            normals.Init("_SceneViewSpaceNormals");
            var shader = Shader.Find("Shader Graphs/ViewSpaceNormalsShader");
            if (shader)
            {
                normalsMaterial = new Material(
                shader
                );
            }
            _filteringSettings = new FilteringSettings(RenderQueueRange.opaque, outLineLayerMask);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor normalsTextureDescriptor = cameraTextureDescriptor;
            normalsTextureDescriptor.colorFormat = _normalsTextureSettings.colorFormat;
            normalsTextureDescriptor.depthBufferBits = _normalsTextureSettings.depthBufferBits;
            cmd.GetTemporaryRT(normals.id, normalsTextureDescriptor, _normalsTextureSettings.filterMode);
            ConfigureTarget(normals.Identifier());
            ConfigureClear(ClearFlag.All, _normalsTextureSettings.backgroundColor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            if (!normalsMaterial)
            {
                return;
            }
            using (new ProfilingScope(cmd, new ProfilingSampler(
                    "SceneViewSpaceNormalsTextureCreation")))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                DrawingSettings drawingSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData,
                    renderingData.cameraData.defaultOpaqueSortFlags);
                drawingSettings.overrideMaterial = normalsMaterial;
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(normals.id);
        }
    }

    private class ScreenSpawceOutlinePass : ScriptableRenderPass
    {
        private readonly Material screenSpaceOutlineMaterial;
        private RenderTargetIdentifier cameraColorTarget;
        private RenderTargetIdentifier temporaryBuffer;
        private int temporaryBufferID = Shader.PropertyToID("_TemporaryBuffer");
        public ScreenSpawceOutlinePass(RenderPassEvent renderPassEvent)
        {
            this.renderPassEvent = renderPassEvent;
            screenSpaceOutlineMaterial = new Material(
                Shader.Find("Shader Graphs/OutlineShader")
            );
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!screenSpaceOutlineMaterial)
            {
                return;
            }
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("ScreenSpaceOutlines")))
            {
                Blit(cmd, cameraColorTarget, temporaryBuffer);
                Blit(cmd, temporaryBuffer, cameraColorTarget,screenSpaceOutlineMaterial);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [SerializeField] private RenderPassEvent _renderPassEvent;
    [SerializeField] private ViewSpaceNormalsTextureSettings _viewSpaceNormalsTextureSettings;
    private ViewSpaceNormalsTexturePass _viewSpaceNormalsTexturePass;
    private ScreenSpawceOutlinePass _screenSpawceOutlinePass;
    [SerializeField] private LayerMask outLineLayerMask;
    public override void Create()
    {
        _viewSpaceNormalsTexturePass = new ViewSpaceNormalsTexturePass(_renderPassEvent, outLineLayerMask, _viewSpaceNormalsTextureSettings);
        _screenSpawceOutlinePass = new ScreenSpawceOutlinePass(_renderPassEvent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_viewSpaceNormalsTexturePass);
        renderer.EnqueuePass(_screenSpawceOutlinePass);
    }
}
