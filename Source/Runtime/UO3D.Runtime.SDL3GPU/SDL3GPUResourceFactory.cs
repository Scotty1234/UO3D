using static SDL3.SDL;

using UO3D.Runtime.RHI;
using UO3D.Runtime.RHI.Resources;

using UO3D.Runtime.SDL3GPU.Resources;

namespace UO3D.Runtime.SDL3GPU;

internal class SDL3GPUResourceFactory : IRenderResourceFactory
{
    //private readonly IntPtr _device;
    private readonly Sdl3GpuDevice _device;

    public SDL3GPUResourceFactory(Sdl3GpuDevice device)
    {
        _device = device;
    }

    public RhiShaderResource NewShaderResource()
    {
        return new Sdl3GpuShaderResource(_device);
    }

    public ShaderInstance NewShaderInstance(RhiShaderResource shaderResource)
    {
        return new ShaderInstance(shaderResource);
    }

    public IRenderTexture CreateTexture(uint width, uint height)
    {
        var texture = new SDL3GPUTexture(_device, new SDL3GPUTextureDescription
        {
            Width = width,
            Height = height,
            Usage = SDL3.SDL.SDL_GPUTextureUsageFlags.SDL_GPU_TEXTUREUSAGE_COLOR_TARGET
        });

        texture.Init();

        return texture;
    }

    public IGraphicsPipeline CreateGraphicsPipeline(in GraphicsPipelineDescription graphicsPipelineDescription)
    {
        var pipeline = new Sdl3GpuGraphicsPipeline(_device, graphicsPipelineDescription);

        return pipeline;
    }

    public IRenderIndexBuffer CreateIndexBuffer(uint length, string name)
    {
        var indexBuffer = new Sdl3GpuIndexBuffer(_device, length, name);

        return indexBuffer;
    }
}
