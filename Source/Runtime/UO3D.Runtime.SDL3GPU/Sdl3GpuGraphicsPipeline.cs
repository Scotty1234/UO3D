using System.Diagnostics;

using static SDL3.SDL;

using UO3D.Runtime.SDL3GPU.Resources;
using UO3D.Runtime.RHI;

namespace UO3D.Runtime.SDL3GPU;

internal class Sdl3GpuGraphicsPipeline: Sdl3GpuResource, IGraphicsPipeline
{
    public readonly SDL3GPUShaderProgram VertexProgram;
    public readonly SDL3GPUShaderProgram PixelProgram;

    public Sdl3GpuGraphicsPipeline(Sdl3GpuDevice device, SDL3GPUShaderProgram vertexProgram, SDL3GPUShaderProgram pixelProgram, string name = "")
        : base(device)
    {
        VertexProgram = vertexProgram;
        PixelProgram = pixelProgram;

        SDL_GPUColorTargetDescription colourTargetDesc = new SDL_GPUColorTargetDescription
        {
            format = SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_B8G8R8A8_UNORM
        };

        unsafe
        {
            var createInfo = new SDL_GPUGraphicsPipelineCreateInfo
            {
                vertex_shader = vertexProgram.Handle,
                fragment_shader = pixelProgram.Handle,
                primitive_type = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_TRIANGLELIST,
                target_info = new SDL_GPUGraphicsPipelineTargetInfo
                {
                    color_target_descriptions = &colourTargetDesc,
                    num_color_targets = 1,
                    
                },
                rasterizer_state = new SDL_GPURasterizerState
                {
                    cull_mode = SDL_GPUCullMode.SDL_GPU_CULLMODE_BACK,
                    fill_mode = SDL_GPUFillMode.SDL_GPU_FILLMODE_FILL,
                    front_face = SDL_GPUFrontFace.SDL_GPU_FRONTFACE_CLOCKWISE
                },
                props = CreateProperty(SDL_PROP_GPU_GRAPHICSPIPELINE_CREATE_NAME_STRING, name)
                
            };

            Handle = SDL_CreateGPUGraphicsPipeline(device.Handle, ref createInfo);
        }

        Debug.Assert(Handle != IntPtr.Zero);
    }

    protected override void FreeResource()
    {
        SDL_ReleaseGPUGraphicsPipeline(Device.Handle, Handle);
    }
}
