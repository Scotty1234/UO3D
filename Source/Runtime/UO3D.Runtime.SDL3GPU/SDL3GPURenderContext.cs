using System.Diagnostics;
using static SDL3.SDL;

using UO3D.Runtime.RHI;
using UO3D.Runtime.RHI.Resources;
using UO3D.Runtime.SDL3GPU.Resources;

namespace UO3D.Runtime.SDL3GPU;

internal class SDL3GPURenderContext: IRenderContext
{
    public IntPtr RecordedCommands { get; private set; }

    public IShaderInstance ShaderInstance
    {
        get => _shaderInstance;
        set 
        { 
            if(_shaderInstance == value)
            {
                return;
            }

            _shaderInstance = value;
            _stateDirty = true;
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

            _graphicsPipeline = value as Sdl3GpuGraphicsPipeline?? throw new Exception("Wrong type of graphics pipeline");
            _pipelineDirty = true;
        }
    }

    private IntPtr _renderPass;
    private IShaderInstance _shaderInstance;
    private Sdl3GpuGraphicsPipeline? _graphicsPipeline;
    private readonly Sdl3GpuDevice _device;

    private bool _stateDirty = true;
    private bool _pipelineDirty = true;

    private RenderPassInfo? _activeRenderPass;

    private Sdl3GpuBuffer<ushort> _indexBuffer;

    public SDL3GPURenderContext(Sdl3GpuDevice device)
    {
        _device = device;

        ushort[] drawIndices = [ 0, 1, 2 ];

        _indexBuffer = new Sdl3GpuBuffer<ushort>(device, RenderBufferType.Index, drawIndices);
    }

    public void BeginRecording()
    {
        RecordedCommands = SDL_AcquireGPUCommandBuffer(_device.Handle);

        _graphicsPipeline = null;
        _activeRenderPass = null;
        _pipelineDirty = true;

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

        _indexBuffer.Bind(_renderPass);
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
        }
        
        SDL_DrawGPUIndexedPrimitives(_renderPass, _indexBuffer.Length, numInstances, 0, 0, 0);
    }
}
