using System.Diagnostics;
using static SDL3.SDL;

using UO3D.Runtime.Renderer;
using UO3D.Runtime.Renderer.Resources;
using UO3D.Runtime.SDL3GPU.Resources;

namespace UO3D.Runtime.SDL3GPU;

internal class SDL3GPURenderContext: IRenderContext
{
    public IntPtr RecordedCommands { get; private set; }
    public IRenderTexture RenderTarget { get; set; }
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

    private IntPtr _renderPass;
    private IShaderInstance _shaderInstance;
    private bool _stateDirty = true;

    public void BeginRecording(IntPtr device)
    {
        RecordedCommands = SDL_AcquireGPUCommandBuffer(device);

        Debug.Assert(RecordedCommands != IntPtr.Zero);

    }

    public void EndRecording()
    {
        SDL_SubmitGPUCommandBuffer(RecordedCommands);
    }

    public void BeginRenderPass(in RenderPassInfo renderPassInfo)
    {
        Debug.Assert(_renderPass ==  IntPtr.Zero);

        SDL_GPUColorTargetInfo colourTargetInfo = new()
        {
            texture = (RenderTarget as SDL3GPUTexture)!.Handle,
            mip_level = 0,
            layer_or_depth_plane = 0,
            clear_color = new()
            {
                r = 1.0f,
                g = 0.0f, 
                b = 0.0f,
                a = 1.0f
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
    }

    public void Draw()
    {
        if(_stateDirty)
        {
            _stateDirty = false;
        }
    }

    public void BindShader(IShaderInstance shaderInstance)
    {
        throw new NotImplementedException();
    }
}
