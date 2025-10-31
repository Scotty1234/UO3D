using System.Diagnostics;
using System.Text;

using static SDL3.SDL;

using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.SDL3GPU.Resources;

internal class SDL3GPUShaderProgram: Sdl3GpuResource
{
    public readonly ShaderProgramType Type;

    public readonly ShaderParameter[] UniformBindings = [];
    public readonly ShaderStreamBinding[] StreamBindings = [];

    public SDL3GPUShaderProgram(Sdl3GpuDevice device, ShaderProgramType type, in ShaderProgramCompileResult compileResult)
        : base(device)
    {
        Type = type;

        SDL_GPUShaderStage stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX;

        switch(type)
        {
            case ShaderProgramType.Vertex:  stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX; break;
            case ShaderProgramType.Pixel: stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_FRAGMENT; break;
            default: Debug.Assert(false); break;    
        }

        StreamBindings = compileResult.StreamBindings;
        UniformBindings = compileResult.UniformBindings;

        string entryName = "main";
        Span<byte> span = Encoding.ASCII.GetBytes(entryName);

        unsafe
        {
            fixed(byte* code = &compileResult.ByteCode[0])
            fixed (byte* p = span)
            {
                var createInfo = new SDL_GPUShaderCreateInfo()
                {
                    code = code,
                    code_size = (UIntPtr)compileResult.ByteCode.Length,
                    entrypoint = p,
                    stage = stage,
                    format = SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXIL,
                    num_uniform_buffers = (uint)UniformBindings.Length,
                    num_samplers = compileResult.NumSamplers,
                };

                Handle = SDL_CreateGPUShader(Device.Handle, createInfo);
            }
        }

        Debug.Assert(Handle !=  IntPtr.Zero);
    }

    protected override void FreeResource()
    {
        SDL_ReleaseGPUShader(Device.Handle, Handle);
    }
}
