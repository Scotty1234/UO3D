using System.Diagnostics;
using System.Text;
using UO3D.Runtime.RHI.Resources;
using static SDL3.SDL;

namespace UO3D.Runtime.SDL3GPU.Resources;

internal class SDL3GPUShaderProgram: Sdl3GpuResource
{
    public readonly ShaderProgramType Type;

    public readonly List<ShaderParameter> Parameters = [];

    public SDL3GPUShaderProgram(Sdl3GpuDevice device, ShaderProgramType type, in ShaderProgramCompileResult compileResult)
        : base(device)
    {
        Type = type;

        SDL_GPUShaderStage stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX;

        switch(type)
        {
            case ShaderProgramType.Vertex:  stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX; break;
            case ShaderProgramType.Fragment: stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_FRAGMENT; break;
            default: Debug.Assert(false); break;    
        }

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
                };

                Handle = SDL_CreateGPUShader(Device.Handle, ref createInfo);
            }
        }

        Debug.Assert(Handle !=  IntPtr.Zero);

        foreach(var input in compileResult.InputParameters)
        {
            Parameters.Add(input);
        }
    }

    protected override void FreeResource()
    {
        SDL_ReleaseGPUShader(Device.Handle, Handle);
    }
}
