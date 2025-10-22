using SDL3;

using UO3D.Runtime.Platform;
using UO3D.Runtime.RHI;

namespace UO3D.Runtime.SDL3GPU;

public class SDL3GPURenderer : IRenderer
{
    public IntPtr Device { get; private set; }

    private readonly IWindow _window;

    private SDL3GPUSwapChain _swapChain;

    public SDL3GPURenderer(IWindow window)
    {
        _window = window;
    }

    public void Startup()
    {
        SDL.SDL_GPUShaderFormat flags = SDL.SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXIL;

        Device = SDL.SDL_CreateGPUDevice(flags, true, null);

        if(Device == IntPtr.Zero)
        {
            throw new Exception("Failed to initialise GPU device.");
        }

       if(SDL.SDL_ClaimWindowForGPUDevice(Device, _window.Handle) == false)
       {
            throw new Exception("Failed to claim window for GPU device.");
       }

       _swapChain = new SDL3GPUSwapChain(Device, _window.Handle);
    }

    public void Shutdown()
    {
        SDL.SDL_DestroyGPUDevice(Device);
    }

    public IRenderContext CreateRenderContext()
    {
        return new SDL3GPURenderContext(Device);
    }
}
