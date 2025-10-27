﻿using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.SDL3GPU.Resources;

internal class Sdl3GpuShaderResource: RhiShaderResource
{
    public SDL3GPUShaderProgram VertexProgram { get; private set; } = null!;
    public SDL3GPUShaderProgram PixelProgram { get; private set; } = null!;

    private readonly Sdl3GpuDevice _device;

    public Sdl3GpuShaderResource(Sdl3GpuDevice device)
    {
        _device = device;
    }

    public override void Load(string vertexShader, string fragmentShader)
    {
        UO3DDxcCompiler.Compile(vertexShader, ShaderProgramType.Vertex, out var vertexCompileResult);
        UO3DDxcCompiler.Compile(fragmentShader, ShaderProgramType.Pixel, out var fragmentCompileResult);

        VertexProgram = new SDL3GPUShaderProgram(_device, ShaderProgramType.Vertex, vertexCompileResult);
        PixelProgram = new SDL3GPUShaderProgram(_device, ShaderProgramType.Pixel, fragmentCompileResult);

        VertexParameters = [.. VertexProgram.UniformBindings];
        PixelParameters = [.. PixelProgram.UniformBindings];
    }
}
