﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using static SDL3.SDL;

using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.SDL3GPU.Resources;

internal class Sdl3GpuBuffer<T>: Sdl3GpuResource
{
    public readonly RenderBufferType Type;
    public readonly T[] Data;

    private readonly SDL_GPUBufferCreateInfo _description;
    private SDL_GPUBufferBinding _bufferBinding = new();

    private readonly Sdl3GpuDevice _device;

    public Sdl3GpuBuffer(Sdl3GpuDevice device, RenderBufferType type, uint length, string name = "")
        : base(device, SDL_SetGPUBufferName, name)
    {
        // SDL_SetGPUBufferName seems to not be set in the .c.
        Type = type;
        _device = device;

        switch (type)
        {
            case RenderBufferType.Index:
                {
                    _description.usage = SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_INDEX;
                    break;
                }

            default:
                {
                    Debug.Assert(false);
                    break;
                }
        }

        _description.size = (uint)(length * Marshal.SizeOf<T>());

        Handle = SDL_CreateGPUBuffer(device.Handle, ref _description);

        _bufferBinding.buffer = Handle;

        Data = new T[length];
    }

    public void Upload()
    {
        // Eventually we will want a ring buffer to submit to in batches, but quick and easy for now...
        var createInfo = new SDL_GPUTransferBufferCreateInfo
        {
            size = _description.size,
            usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD
        };

        IntPtr transferBuffer = SDL_CreateGPUTransferBuffer(_device.Handle, ref createInfo);

        IntPtr mappedMemory = SDL_MapGPUTransferBuffer(Device.Handle, transferBuffer, false);

        unsafe
        {
            fixed (void* src = Data)
            {
                Buffer.MemoryCopy(src, (void*)mappedMemory, _description.size, _description.size);
            }
        }

        SDL_UnmapGPUTransferBuffer(Device.Handle, mappedMemory);

        IntPtr commandBuffer = SDL_AcquireGPUCommandBuffer(Device.Handle);
        IntPtr copyPass = SDL_BeginGPUCopyPass(commandBuffer);

        SDL_GPUTransferBufferLocation location = new SDL_GPUTransferBufferLocation
        {
            transfer_buffer = transferBuffer,
            
            offset = 0
        };

        var region = new SDL_GPUBufferRegion
        {
            buffer = Handle,
            offset = 0,
            size = _description.size
        };

        SDL_UploadToGPUBuffer(copyPass, ref location, ref region, false);

        SDL_EndGPUCopyPass(copyPass);
        SDL_SubmitGPUCommandBuffer(commandBuffer);

        SDL_ReleaseGPUTransferBuffer(Device.Handle, transferBuffer);
    }

    public void Bind(IntPtr renderPassHandle)
    {
        switch (Type)
        {
            case RenderBufferType.Index:
                {
                    SDL_BindGPUIndexBuffer(renderPassHandle, ref _bufferBinding, SDL_GPUIndexElementSize.SDL_GPU_INDEXELEMENTSIZE_16BIT);
                    break;
                }
            default:
                {
                    Debug.Assert(false);
                    break;
                }
        }
    }

    protected override void FreeResource()
    {
        SDL_ReleaseGPUBuffer(Device.Handle, _bufferBinding.buffer);

        _bufferBinding.buffer = IntPtr.Zero;
    }
}
