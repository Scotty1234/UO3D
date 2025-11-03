namespace UO3D.Runtime.RHI.Resources;

public enum ShaderProgramType
{
    Vertex,
    Pixel,
    Compute,
    Count,
    Invalid
}

public static class ShaderProgramTypeExtensions
{
    public static int ToInt(this ShaderProgramType shaderProgramType)
    {
        return (int)shaderProgramType;
    }
}