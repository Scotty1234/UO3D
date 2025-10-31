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

    private readonly IRenderTexture[] _textures;

    public ShaderInstance(RhiShaderResource shaderResource)
    {
        _shaderResource = shaderResource;

        BindingData[(int)ShaderProgramType.Vertex].Bindings = new ShaderBindingDataEntry[_shaderResource.VertexParameters.Length];

        for(int i = 0; i < _shaderResource.VertexParameters.Length; i++)
        {
            BindingData[(int)ShaderProgramType.Vertex].Bindings[i] = new ShaderBindingDataEntry(_shaderResource.VertexParameters[i].SlotIndex, _shaderResource.VertexParameters[i].Size);
        }

        int numTextures = 0;

        for (int i = 0; i < _shaderResource.PixelParameters.Length; i++)
        {
            if(_shaderResource.VertexParameters[i].InputType == RhiShaderInputType.Texture)
            {
                numTextures++;
            }

            BindingData[(int)ShaderProgramType.Pixel].Bindings[i] = new ShaderBindingDataEntry(_shaderResource.VertexParameters[i].SlotIndex, _shaderResource.VertexParameters[i].Size);
        }

        _textures = new IRenderTexture[numTextures];
    }

    public ShaderBindingHandle GetBindingHandleTexturePixel(string name)
    {
        return GetBindingHandle(ShaderProgramType.Pixel, RhiShaderInputType.Texture, name);
    }

    public ShaderBindingHandle GetBindingHandle(ShaderProgramType programType, RhiShaderInputType inputType, string name)
    {
        return _shaderResource.GetBindingHandle(programType, inputType, name);
    }

    public void SetParameter(ShaderBindingHandle bindingHandle, in Matrix4x4 matrix)
    {
        var memory = BindingData[(int)bindingHandle.ProgramType].Bindings[bindingHandle.Handle].Data;

        Debug.Assert(memory != null);

        MemoryMarshal.Write(memory.AsSpan(), matrix);
    }

    public void SetTexture(ShaderBindingHandle bindingHandle, IRenderTexture texture)
    {
        _textures[bindingHandle.Handle] = texture;
    }
}
