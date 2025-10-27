using System.Diagnostics;
using UO3D.Runtime.RHI.Resources;
using Vortice.Direct3D;
using Vortice.Direct3D12.Shader;
using Vortice.Dxc;

namespace UO3D.Runtime.SDL3GPU;

struct ShaderProgramCompileResult
{
    public byte[] ByteCode;
    public ShaderStreamBinding[] StreamBindings;
    public ShaderParameter[] UniformBindings;
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

        List<ShaderParameter> shaderParameters = [];

        for(uint i = 0; i < reflection.BoundResources.Length; i++)
        {
            var resourceDescription = reflection.BoundResources[i];
            
            if(resourceDescription.Type == Vortice.Direct3D.ShaderInputType.ConstantBuffer)
            {
                ID3D12ShaderReflectionConstantBuffer constantBuffer = reflection.GetConstantBufferByName(resourceDescription.Name);
                
                for(uint j = 0; j < constantBuffer.Variables.Length; j++)
                {
                    ShaderVariableDescription varDesc = constantBuffer.GetVariableByIndex(j).Description;

                    shaderParameters.Add(new ShaderParameter
                    {
                        Name = varDesc.Name,
                        StartOffset = varDesc.StartOffset,
                        Size = varDesc.Size,
                    });
                }
            }
            else
            {
                Debug.Assert(false);
            }
        }

        outCompileResult.UniformBindings = [.. shaderParameters];

        outCompileResult.StreamBindings = new ShaderStreamBinding[reflection.InputParameters.Length];

        for (uint i = 0; i < reflection.InputParameters.Length; i++)
        {
            var param = reflection.InputParameters[i];

            outCompileResult.StreamBindings[i] = new ShaderStreamBinding
            {
                SemanticName = param.SemanticName,
                SemanticIndex = param.Register
            };
        }
    }
}
