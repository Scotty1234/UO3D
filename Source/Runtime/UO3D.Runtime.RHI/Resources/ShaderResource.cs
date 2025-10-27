using System.Diagnostics;

namespace UO3D.Runtime.RHI.Resources;

public enum ShaderInputType
{
    Buffer,
    Texture,
    Count,
    Invalid
}

[DebuggerDisplay("{Name}")]
public struct ShaderParameter
{
    public string Name;
    public uint StartOffset;
    public uint Size;
    ShaderInputType InputType;
    public uint SlotIndex;
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

    public ShaderBindingHandle GetBindingHandle(ShaderProgramType programType, string name)
    {
        switch (programType)
        {
            case ShaderProgramType.Vertex: return GetBindingHandleVertex(name);
            case ShaderProgramType.Pixel: return GetBindingHandlePixel(name);
            default: break;
        }

        throw new UnreachableException("Could not find shader binding handle.");
    }

    private ShaderBindingHandle GetBindingHandleVertex(string name)
    {
        for(int i = 0; i < VertexParameters.Length; i++)
        {
            if (VertexParameters[i].Name == name)
            {
                return new ShaderBindingHandle((ushort)i, ShaderProgramType.Vertex);
            }
        }

        throw new UnreachableException("Could not find shader binding handle in vertex shader.");
    }

    private ShaderBindingHandle GetBindingHandlePixel(string name)
    {
        for (int i = 0; i < PixelParameters.Length; i++)
        {
            if (PixelParameters[i].Name == name)
            {
                return new ShaderBindingHandle((ushort)i, ShaderProgramType.Pixel);
            }
        }

        throw new UnreachableException("Could not find shader binding handle in pixel shader.");
    }
}
