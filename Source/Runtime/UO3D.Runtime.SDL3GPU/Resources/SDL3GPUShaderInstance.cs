using static SDL3.SDL;

using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.SDL3GPU.Resources;

internal class SDL3GPUShaderInstance: IShaderInstance
{
    public readonly ShaderParameter[] VertexParameters;
    public readonly ShaderParameter[] PixelParameters;

    public readonly SDL3GPUShaderProgram VertexProgram;
    public readonly SDL3GPUShaderProgram PixelProgram;

    public SDL3GPUShaderInstance(SDL3GPUShaderProgram vertexProgram, SDL3GPUShaderProgram pixelProgram)
    {
        VertexProgram = vertexProgram;
        PixelProgram = pixelProgram;

        VertexParameters = [.. vertexProgram.Parameters];
        PixelParameters = [.. pixelProgram.Parameters];
    }

    public void GetVertexParameter(string name)
    {
    }

    public void SetVertexParameter(string name)
    {

    }

    public void GetPixelParameter(string name)
    {

    }
}
