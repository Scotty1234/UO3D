﻿using static SDL3.SDL;
using UO3D.Runtime.RHI;

namespace UO3D.Runtime.SDL3GPU;

internal class Sdl3GpuDevice: IRenderDevice
{
    //public IntPtr Handle { get; private set; }

    public void Setup()
    {
        SDL_GPUShaderFormat flags = SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXIL;

        Handle = SDL_CreateGPUDevice(flags, true, null);

        if (Handle == IntPtr.Zero)
        {
            throw new Exception("Failed to initialise GPU device.");
        }
    }
}
