using System.Diagnostics;

namespace UO3D.Runtime.RHI.Resources;

public enum RhiShaderInputType
{
    Buffer,
    Texture,
    Sampler,
    Count,
    Invalid
}

[DebuggerDisplay("{Name}")]
public struct ShaderVariable
{
    public string Name;
    public uint Size;
    public uint Offset;
}

[DebuggerDisplay("{Name}")]
public struct ShaderParameter
{
    public string Name;
    public uint StartOffset;
    public uint Size;
    public RhiShaderInputType InputType;
    public uint SlotIndex;
    public ShaderVariable[] Variables;
}

[DebuggerDisplay("{SemanticName}, {SemanticIndex}")]
public struct ShaderStreamBinding
{
    public string SemanticName;
    public uint SemanticIndex;
}

public readonly struct ShaderBindingHandle
{
    public readonly ushort Handle;
    public readonly ShaderProgramType ProgramType;
    public const ushort InvalidHandle = 0xFF;
    public static readonly ShaderBindingHandle Invalid = new(InvalidHandle, ShaderProgramType.Invalid);

    public bool IsValid => Handle != InvalidHandle;

    public ShaderBindingHandle(ushort handle, ShaderProgramType shaderProgramType)
    {
        ProgramType = shaderProgramType;
        Handle = handle;
    }
}

public abstract class RhiShaderResource
{
    public ShaderParameter[] VertexParameters { get; protected set; }
    public ShaderParameter[] PixelParameters { get; protected set; }

    public abstract void Load(string vertexShader, string fragmentShader);

    public ShaderBindingHandle GetBindingHandle(ShaderProgramType programType, RhiShaderInputType inputType, string name)
    {
        switch (programType)
        {
            case ShaderProgramType.Vertex: return GetBindingHandleVertex(inputType, name);
            case ShaderProgramType.Pixel: return GetBindingHandlePixel(inputType, name);
            default: break;
        }

        throw new UnreachableException("Could not find shader binding handle.");
    }

    private ShaderBindingHandle GetBindingHandleVertex(RhiShaderInputType inputType, string name)
    {
        for(int i = 0; i < VertexParameters.Length; i++)
        {
            if ((VertexParameters[i].Name == name) && (VertexParameters[i].InputType == inputType)
            {
                return new ShaderBindingHandle((ushort)i, ShaderProgramType.Vertex);
            }
        }

        throw new UnreachableException("Could not find shader binding handle in vertex shader.");
    }

    private ShaderBindingHandle GetBindingHandlePixel(RhiShaderInputType inputType, string name)
    {
        for (int i = 0; i < PixelParameters.Length; i++)
        {
            if ((PixelParameters[i].Name == name) && (PixelParameters[i].InputType == inputType))
            {
                return new ShaderBindingHandle((ushort)i, ShaderProgramType.Pixel);
            }
        }

        throw new UnreachableException("Could not find shader binding handle in pixel shader.");
    }
}
