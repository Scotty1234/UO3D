using System.Diagnostics;

using static SDL3.SDL;

using UO3D.Runtime.SDL3GPU.Resources;
using UO3D.Runtime.RHI;

namespace UO3D.Runtime.SDL3GPU;

internal class Sdl3GpuGraphicsPipeline: IGraphicsPipeline
{
    public readonly IntPtr Handle;

    public readonly SDL3GPUShaderProgram VertexProgram;
    public readonly SDL3GPUShaderProgram PixelProgram;

    public Sdl3GpuGraphicsPipeline(IntPtr device, SDL3GPUShaderProgram vertexProgram, SDL3GPUShaderProgram pixelProgram)
    {
        VertexProgram = vertexProgram;
        PixelProgram = pixelProgram;

        SDL_GPUColorTargetDescription colourTargetDesc;

        unsafe
        {
            var createInfo = new SDL_GPUGraphicsPipelineCreateInfo
            {
                vertex_shader = vertexProgram.Handle,
                fragment_shader = pixelProgram.Handle,
                primitive_type = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_TRIANGLESTRIP,
                target_info = new SDL_GPUGraphicsPipelineTargetInfo
                {
                    color_target_descriptions = &colourTargetDesc,
                    num_color_targets = 1
                }
            };

            Handle = SDL_CreateGPUGraphicsPipeline(device, ref createInfo);
        }

        Debug.Assert(Handle != IntPtr.Zero);
    }
}
