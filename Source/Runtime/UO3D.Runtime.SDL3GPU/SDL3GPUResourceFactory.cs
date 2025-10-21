using UO3D.Runtime.RHI;
using UO3D.Runtime.RHI.Resources;

using UO3D.Runtime.SDL3GPU.Resources;

namespace UO3D.Runtime.SDL3GPU;

internal class SDL3GPUResourceFactory : IRenderResourceFactory
{
    private readonly IntPtr _device;

    public SDL3GPUResourceFactory(IRenderer renderer)
    {
        _device = (renderer as SDL3GPURenderer)!.Device;
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

    public IRenderTexture CreateTexture(int width, int height)
    {
        throw new NotImplementedException();
    }

    public IGraphicsPipeline CreateGraphicsPipeline(IShaderInstance shaderInstance)
    {
        SDL3GPUShaderInstance sdl3GpuShaderInstance = (shaderInstance as SDL3GPUShaderInstance)!;

        var pipeline = new Sdl3GpuGraphicsPipeline(_device, sdl3GpuShaderInstance.VertexProgram, sdl3GpuShaderInstance.PixelProgram);

        return pipeline;
    }
}
