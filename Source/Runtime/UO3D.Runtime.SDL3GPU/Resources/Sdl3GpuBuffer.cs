using System.Diagnostics;
using System.Runtime.InteropServices;
using UO3D.Runtime.RHI.Resources;
using static SDL3.SDL;

namespace UO3D.Runtime.SDL3GPU.Resources
{
    internal class Sdl3GpuBuffer<T>
    {
        public readonly RenderBufferType Type;

        public uint Length { get; private set; }

        private readonly SDL_GPUBufferCreateInfo _description;
        private SDL_GPUBufferBinding _bufferBinding = new();

        private readonly Sdl3GpuDevice _device;

        public Sdl3GpuBuffer(Sdl3GpuDevice device, RenderBufferType type, T[] data) 
        {
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

            _description.size = (uint)(data.Length * Marshal.SizeOf<T>());

            _bufferBinding.buffer = SDL_CreateGPUBuffer(device.Handle, ref _description);
            
            Length = (uint)data.Length;
        }

        public void Upload()
        {
            var createInfo = new SDL_GPUTransferBufferCreateInfo
            {

            };

            IntPtr uploadBuffer = SDL_CreateGPUTransferBuffer(_device.Handle, ref createInfo);
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
    }
}
