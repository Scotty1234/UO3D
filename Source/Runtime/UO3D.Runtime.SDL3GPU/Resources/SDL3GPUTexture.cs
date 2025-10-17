using SDL3;
using UO3D.Runtime.Renderer.Resources;
using static SDL3.SDL;

namespace UO3D.Runtime.SDL3GPU.Resources;

internal struct SDL3GPUTextureDescription
{
    public uint Width;
    public uint Height;
    public SDL_GPUTextureUsageFlags Usage; 
}

internal class SDL3GPUTexture: IRenderTexture, IDisposable
{
    public readonly SDL3GPUTextureDescription Description;

    public IntPtr Handle { get; private set; }

    private readonly IntPtr _device;

    public SDL3GPUTexture(IntPtr device, in SDL3GPUTextureDescription description)
    {
        _device = device;
        Description = description;
    }

    public void Init(IntPtr device)
    {
        var createInfo = new SDL_GPUTextureCreateInfo()
        {
            usage = Description.Usage,
            width = Description.Width,
            height = Description.Height,
            layer_count_or_depth = 0,
            num_levels = 0,
            sample_count = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1,
            props = 0
        };

        Handle = SDL_CreateGPUTexture(device, ref createInfo);
    }

    public void InitFromExistingResource(IntPtr _handle)
    {
        Handle = _handle;
    }

    public void Dispose()
    {
        SDL_ReleaseGPUTexture(_device, Handle);

        Handle = IntPtr.Zero;
    }
}
