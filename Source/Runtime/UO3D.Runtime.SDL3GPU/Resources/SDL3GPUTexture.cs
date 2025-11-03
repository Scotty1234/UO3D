using UO3D.Runtime.RHI.Resources;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static SDL3.SDL;

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

    public readonly uint[] Texels;

    public SDL3GPUTexture(Sdl3GpuDevice device, in SDL3GPUTextureDescription description)
        : base(device, SDL_SetGPUTextureName, description.Name)
    {
        Description = description;
        Texels = new uint[Description.Width * Description.Height];
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

    public void SetData(uint[] texels)
    {
        texels.AsSpan().CopyTo(Texels);

        IntPtr transferBuffer = SDL_CreateGPUTransferBuffer(Device.Handle, new SDL_GPUTransferBufferCreateInfo
        {
            size = (uint)(texels.Length * sizeof(uint)),
            usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD
        });

        IntPtr mappedMemory = SDL_MapGPUTransferBuffer(Device.Handle, transferBuffer, false);

        uint size = (uint)Texels.Length * sizeof(uint);

        unsafe
        {
            fixed (void* src = Texels)
            {
                Buffer.MemoryCopy(src, (void*)mappedMemory, size, size);
            }
        }

        SDL_UnmapGPUTransferBuffer(Device.Handle, mappedMemory);

        IntPtr commandBuffer = SDL_AcquireGPUCommandBuffer(Device.Handle);
        IntPtr copyPass = SDL_BeginGPUCopyPass(commandBuffer);

        var transferInfo = new SDL_GPUTextureTransferInfo
        {
            transfer_buffer = transferBuffer,
            offset = 0,
        };

        var textureRegion = new SDL_GPUTextureRegion
        {
            texture = Handle,
            w = Description.Width,
            h = Description.Height,
            d = 1
        };

        SDL_UploadToGPUTexture(copyPass, transferInfo, textureRegion, false);

        SDL_EndGPUCopyPass(copyPass);
        SDL_SubmitGPUCommandBuffer(commandBuffer);
        SDL_UnmapGPUTransferBuffer(Device.Handle, transferBuffer);
    }

    protected override void FreeResource()
    {
        SDL_DestroyTexture(Handle);
    }
}
