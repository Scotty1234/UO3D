using static SDL3.SDL;

using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.SDL3GPU.Resources;

internal struct SDL3GPUTextureDescription
{
    public uint Width;
    public uint Height;
    public SDL_GPUTextureUsageFlags Usage;
    public string Name;
}

internal class SDL3GPUTexture: Sdl3GpuResource, IRenderTexture
{
    public readonly SDL3GPUTextureDescription Description;

    public SDL3GPUTexture(IntPtr device, in SDL3GPUTextureDescription description)
        : base(device, SDL_DestroyTexture, SDL_SetGPUTextureName, description.Name)
    {
        Description = description;
    }

    public void Init()
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

        Handle = SDL_CreateGPUTexture(Device, ref createInfo);
    }

    public void InitFromExistingResource(IntPtr _handle)
    {
        Handle = _handle;
    }
}
