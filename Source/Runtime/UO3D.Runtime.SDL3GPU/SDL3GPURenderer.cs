using SDL3;

using UO3D.Runtime.Platform;
using UO3D.Runtime.Renderer;

namespace UO3D.Runtime.SDL3GPU;

public class SDL3GPURenderer : IRenderer
{
    public event Action<IRenderContext>? OnFrameBegin;
    public event Action<IRenderContext>? OnFrameEnd;

    private readonly SDL3GPURenderContext _context = new();
    private readonly IWindow _window;

    private IntPtr _gpuDevice;

    private SDL3GPUSwapChain _swapChain;

    public SDL3GPURenderer(IWindow window)
    {
        _window = window;
    }

    public void Startup()
    {
        SDL.SDL_GPUShaderFormat flags = SDL.SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXIL;

        _gpuDevice = SDL.SDL_CreateGPUDevice(flags, true, null);

        if(_gpuDevice == IntPtr.Zero)
        {
            throw new Exception("Failed to initialise GPU device.");
        }

       if(SDL.SDL_ClaimWindowForGPUDevice(_gpuDevice, _window.Handle) == false)
       {
            throw new Exception("Failed to claim window for GPU device.");
       }

       _swapChain = new SDL3GPUSwapChain(_gpuDevice, _window.Handle);
    }

    public void Shutdown()
    {
        SDL.SDL_DestroyGPUDevice(_gpuDevice);
    }

    public void FrameBegin()
    {
        _context.BeginRecording(_gpuDevice);

        _swapChain.Acquire(_context);

        _context.BeginRenderPass(new RenderPassInfo());

        OnFrameBegin?.Invoke(_context);
    }

    public void FrameEnd()
    {
        OnFrameEnd?.Invoke(_context);

        _context.EndRenderPass();
        _context.EndRecording();
    }
}
