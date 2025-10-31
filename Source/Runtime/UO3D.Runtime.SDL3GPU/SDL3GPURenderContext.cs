using System.Diagnostics;

using static SDL3.SDL;

using UO3D.Runtime.RHI;
using UO3D.Runtime.RHI.Resources;
using UO3D.Runtime.SDL3GPU.Resources;

namespace UO3D.Runtime.SDL3GPU;

internal class SDL3GPURenderContext: IRenderContext
{
    public IntPtr RecordedCommands { get; private set; }

    public ShaderInstance ShaderInstance
    {
        get => _shaderInstance;
        set 
        { 
            if(_shaderInstance == value)
            {
                return;
            }

            _shaderInstance = value;
            _shaderInstanceDirty = true;
        }
    }

    public IGraphicsPipeline GraphicsPipline 
    {
        get => _graphicsPipeline;
        set
        {
            if (_graphicsPipeline == value)
            {
                return;
            }

            _graphicsPipeline = (Sdl3GpuGraphicsPipeline)value;
            _pipelineDirty = true;
        }
    }

    public IRenderIndexBuffer IndexBuffer
    {
        get => _indexBuffer;
        set
        {
            if (_indexBuffer == value)
            {
                return;
            }

            _indexBuffer = (Sdl3GpuIndexBuffer)value;
            _indexBufferDirty = true;
        }
    }

    public ModelViewProjection MVP 
    { 
        get => _sceneView; 
        set
        {
            _sceneView = value;

            unsafe
            {
                fixed(ModelViewProjection* data = &_sceneView)
                {
                    SDL_PushGPUVertexUniformData(RecordedCommands, 0, (IntPtr)data, (uint)sizeof(ModelViewProjection));
                }
            }
        }
    }

    private IntPtr _renderPass;
    private ShaderInstance _shaderInstance;
    private Sdl3GpuGraphicsPipeline? _graphicsPipeline;
    private readonly Sdl3GpuDevice _device;

    private bool _pipelineDirty = true;
    private bool _indexBufferDirty = true;
    private bool _shaderInstanceDirty = true;

    private RenderPassInfo? _activeRenderPass;

    private Sdl3GpuIndexBuffer _indexBuffer;
    private ModelViewProjection _sceneView;

    private delegate void ShaderUploadFunc(IntPtr recordedCommands, uint bindingIndex, IntPtr dataPtr, uint dataSize);

    private static readonly ShaderUploadFunc[] UploadFuncs =
    {
        SDL_PushGPUVertexUniformData,
        SDL_PushGPUFragmentUniformData
    };

    public SDL3GPURenderContext(Sdl3GpuDevice device)
    {
        _device = device;
    }

    public void BeginRecording()
    {
        RecordedCommands = SDL_AcquireGPUCommandBuffer(_device.Handle);

        _graphicsPipeline = null;
        _activeRenderPass = null;
        _pipelineDirty = true;
        _indexBuffer = null;

        Debug.Assert(RecordedCommands != IntPtr.Zero);
    }

    public void EndRecording()
    {
        SDL_SubmitGPUCommandBuffer(RecordedCommands);
    }

    public void BeginRenderPass(in RenderPassInfo renderPassInfo)
    {
        Debug.Assert(_renderPass ==  IntPtr.Zero);

        _activeRenderPass = renderPassInfo;

        SDL_GPUColorTargetInfo colourTargetInfo = new()
        {
            texture = (renderPassInfo.RenderTarget.Texture as SDL3GPUTexture)!.Handle,
            mip_level = 0,
            layer_or_depth_plane = 0,
            // Note must always clear to 0 otherwise SDK layer complains.
            // Okay for now as usually do not need a clear colour other than "empty".
            clear_color = new()
            {
                r = 0.0f,
                g = 0.0f, 
                b = 0.0f,
                a = 0.0f
            },
            load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR,
            store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE,
            resolve_texture = IntPtr.Zero,
            resolve_layer = 0,
            cycle = false,
            cycle_resolve_texture = false,
        };
        
        SDL_GPUDepthStencilTargetInfo depthTargetInfo = default;

        // Note I changed the SDL binding here to ignore depth for now.
        _renderPass = SDL_BeginGPURenderPass(RecordedCommands, [colourTargetInfo], 1, IntPtr.Zero);

        Debug.Assert(_renderPass != IntPtr.Zero);
    }

    public void EndRenderPass()
    {
        Debug.Assert(_renderPass != IntPtr.Zero);

        SDL_EndGPURenderPass(_renderPass);

        _renderPass = IntPtr.Zero;
        _activeRenderPass = null;
    }

    public void DrawIndexedPrimitives(uint numInstances)
    {
        if(_pipelineDirty)
        {
            SDL_BindGPUGraphicsPipeline(_renderPass, _graphicsPipeline.Handle);

            _pipelineDirty = false;
            _shaderInstanceDirty = true;
        }

        if(_indexBufferDirty)
        {
            _indexBuffer.Bind(_renderPass);

            _indexBufferDirty = false;
        }

        if(_shaderInstanceDirty)
        {
            //BindShaderParameters();

            _shaderInstanceDirty = false;
        }
        
        SDL_DrawGPUIndexedPrimitives(_renderPass, (uint)_indexBuffer.Data.Length, numInstances, 0, 0, 0);
    }

    private void BindShaderParameters()
    {
        for(int i = 0; i < (int)ShaderProgramType.Count; i++)
        {
            var bindings = _shaderInstance.BindingData[i].Bindings;

            if(bindings is null)
            {
                continue;
            }

            var upload = UploadFuncs[i];

            foreach (var entry in bindings)
            {
               unsafe
                {
                    fixed(byte* data = entry.Data)
                    {
                        upload(RecordedCommands, entry.BindingIndex, (IntPtr)data, (uint)entry.Data.Length);
                    }
                }
            }
        }
    }
}
