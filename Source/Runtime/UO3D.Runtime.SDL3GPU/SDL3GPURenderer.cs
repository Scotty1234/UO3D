﻿using UO3D.Runtime.Platform;
using UO3D.Runtime.RHI;
using UO3D.Runtime.RHI.Resources;
using static SDL3.SDL;

namespace UO3D.Runtime.SDL3GPU;

internal class SDL3GPURenderer : IRenderer
{
    public IRenderSwapChain SwapChain { get; private set; }

    private readonly IWindow _window;

    private readonly Sdl3GpuDevice _device;

    public SDL3GPURenderer(IWindow window, Sdl3GpuDevice device)
    {
        _window = window;
        _device = device;
    }

    public void Startup()
    {
        _device.Setup();

       if(SDL_ClaimWindowForGPUDevice(_device.Handle, _window.Handle) == false)
       {
            throw new Exception("Failed to claim window for GPU device.");
       }

        SwapChain = new SDL3GPUSwapChain(_device, _window.Handle);
    }

    public void Shutdown()
    {
        SDL_DestroyGPUDevice(_device.Handle);
    }

    public IRenderContext CreateRenderContext()
    {
        return new SDL3GPURenderContext(_device);
    }
}
