using static SDL3.SDL;

using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.SDL3GPU.Resources;

internal struct SDL3GPUTextureDescription
{
    public uint Width;
    public uint Height;
    public SDL_GPUTextureUsageFlags Usage;
    public SDL_GPUTextureFormat Format;
    public string Name;
}

internal class SDL3GPUTexture: Sdl3GpuResource, IRenderTexture
{
    public readonly SDL3GPUTextureDescription Description;

    public SDL3GPUTexture(Sdl3GpuDevice device, in SDL3GPUTextureDescription description)
        : base(device, SDL_SetGPUTextureName, description.Name)
    {
        Description = description;
    }

    public void Init()
    {
        var createInfo = new SDL_GPUTextureCreateInfo()
        {
            usage = Description.Usage,
            format = SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R8G8B8A8_UNORM,
            width = Description.Width,
            height = Description.Height,
            layer_count_or_depth = 1,
            num_levels = 1,
            sample_count = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1,
            props = 0,
        };

        Handle = SDL_CreateGPUTexture(Device.Handle, createInfo);
    }

    public void InitFromExistingResource(IntPtr _handle)
    {
        Handle = _handle;
    }

    protected override void FreeResource()
    {
        SDL_DestroyTexture(Handle);
    }
}
