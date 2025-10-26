using System.Diagnostics;
using UO3D.Runtime.RHI.Resources;
using Vortice.Dxc;
using Vortice.Direct3D12.Shader;

namespace UO3D.Runtime.SDL3GPU;

struct ShaderProgramCompileResult
{
    public byte[] ByteCode;
    public ShaderParameter[] InputParameters;
}

internal class UO3DDxcCompiler
{
    public static void Compile(string shaderFile, ShaderProgramType type, out ShaderProgramCompileResult outCompileResult)
    {
        if(File.Exists(shaderFile) == false)
        {
            throw new Exception($"Could not find file {shaderFile}");
        }

        var source = File.ReadAllText(shaderFile);

        string targetProfileType = "";
        string shaderModelVersion = "6_0";

        switch (type)
        {
            case ShaderProgramType.Vertex: targetProfileType = "vs"; break;
            case ShaderProgramType.Pixel: targetProfileType = "ps"; break;
            default: Debug.Assert(false); break;
        }

        string targetProfile = $"{targetProfileType}_{shaderModelVersion}";

        string[] arguments = new[]
{
            "-E",               "main",
            "-T",               targetProfile,
            "-Zi",                                  // Debug info
            "-Qembed_debug",                        // Embed debug info in the shader
            "-O0"                                   // Optimization level
        };

        using IDxcResult result = DxcCompiler.Compile(source, arguments);

        if (result.GetStatus().Failure)
        {
            throw new Exception("Compilation failed:\n" + result.GetErrors());
        }

        var blob = result.GetResult();

        outCompileResult = new ShaderProgramCompileResult();

        outCompileResult.ByteCode = blob.AsBytes();

        using ID3D12ShaderReflection reflection = DxcCompiler.Utils.CreateReflection<ID3D12ShaderReflection>(blob);

        for(uint i = 0; i < reflection.BoundResources.Length; i++)
        {

        }

        outCompileResult.InputParameters = new ShaderParameter[reflection.InputParameters.Length];

        for (uint i = 0; i < reflection.InputParameters.Length; i++)
        {
            var param = reflection.InputParameters[i];

            outCompileResult.InputParameters[i] = new ShaderParameter
            {
                Name = param.SemanticName,
                Register = param.Register
            };
        }
    }
}
