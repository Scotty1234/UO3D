using SDL3;
using UO3D.Runtime.SDL3GPU.Resources;

namespace UO3D.Runtime.SDL3GPU;

internal class SDL3GPUSwapChain
{
    private IntPtr _backbufferToRenderInto;
    private uint _backbufferWidth;
    private uint _backbufferHeight;

    private readonly IntPtr _windowHandle;
    private readonly IntPtr _device;

    public SDL3GPUSwapChain(IntPtr device, IntPtr windowHandle)
    {
        _device = device;
        _windowHandle = windowHandle;
    }

    public void Acquire(SDL3GPURenderContext context)
    {
        SDL.SDL_WaitAndAcquireGPUSwapchainTexture(context.RecordedCommands, _windowHandle,
            out _backbufferToRenderInto, out _backbufferWidth, out _backbufferHeight);

       var renderTarget = new SDL3GPUTexture(_device, new SDL3GPUTextureDescription
        {
            Width = _backbufferWidth,
            Height = _backbufferHeight
        });

        renderTarget.InitFromExistingResource(_backbufferToRenderInto);
    }
}
