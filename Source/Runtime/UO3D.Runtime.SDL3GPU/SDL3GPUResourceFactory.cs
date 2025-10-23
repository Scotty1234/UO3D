using System.Diagnostics;
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

    public IShaderInstance CreateShaderInstance(string vertexShader, string fragmentShader)
    {
        UO3DDxcCompiler.Compile(vertexShader, ShaderProgramType.Vertex, out var vertexCompileResult);
        UO3DDxcCompiler.Compile(fragmentShader, ShaderProgramType.Fragment, out var fragmentCompileResult);

        var vertexProgram = new SDL3GPUShaderProgram(_device, ShaderProgramType.Vertex, vertexCompileResult);
        var fragmentProgram = new SDL3GPUShaderProgram(_device, ShaderProgramType.Fragment, fragmentCompileResult);

        var shaderInstance = new SDL3GPUShaderInstance(vertexProgram, fragmentProgram);

        return shaderInstance;
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

    public IGraphicsPipeline CreateGraphicsPipeline(IShaderInstance shaderInstance, string name)
    {
        SDL3GPUShaderInstance sdl3GpuShaderInstance = (shaderInstance as SDL3GPUShaderInstance)!;

        var pipeline = new Sdl3GpuGraphicsPipeline(_device, sdl3GpuShaderInstance.VertexProgram, sdl3GpuShaderInstance.PixelProgram, name);

        return pipeline;
    }

    public IRenderIndexBuffer CreateIndexBuffer(uint length, string name)
    {
        var indexBuffer = new Sdl3GpuIndexBuffer(_device, length, name);

        return indexBuffer;
    }
}
