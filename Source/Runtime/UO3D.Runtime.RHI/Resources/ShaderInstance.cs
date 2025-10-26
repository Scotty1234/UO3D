using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace UO3D.Runtime.RHI.Resources;

[DebuggerDisplay("{BindingIndex}, Data {Data.Length}")]
public readonly struct ShaderBindingDataEntry
{
    public readonly uint BindingIndex;
    public readonly byte[] Data;

    public ShaderBindingDataEntry(uint index, uint dataSize)
    {
        BindingIndex = index;

        Data = new byte[dataSize];
    }
}

public struct ShaderProgramBindings
{
    public ShaderBindingDataEntry[] Bindings;
}

public class ShaderInstance
{
    public readonly ShaderProgramBindings[] BindingData = new ShaderProgramBindings[(int)ShaderProgramType.Count];

    private readonly RhiShaderResource _shaderResource;

    public ShaderInstance(RhiShaderResource shaderResource)
    {
        _shaderResource = shaderResource;

        BindingData[(int)ShaderProgramType.Vertex].Bindings = new ShaderBindingDataEntry[_shaderResource.VertexParameters.Length];

        for(int i = 0; i < _shaderResource.VertexParameters.Length; i++)
        {
            BindingData[(int)ShaderProgramType.Vertex].Bindings[i] = new ShaderBindingDataEntry(_shaderResource.VertexParameters[i].Register, _shaderResource.VertexParameters[i].Size);
        }
    }

    public ShaderBindingHandle GetBindingHandle(ShaderProgramType programType, string name)
    {
        return _shaderResource.GetBindingHandle(programType, name);
    }

    public void SetParameter(ShaderBindingHandle bindingHandle, in Matrix4x4 matrix)
    {
        var memory = BindingData[(int)bindingHandle.ProgramType].Bindings[bindingHandle.Handle].Data;

        MemoryMarshal.Write(memory.AsSpan(), matrix);
    }
}
