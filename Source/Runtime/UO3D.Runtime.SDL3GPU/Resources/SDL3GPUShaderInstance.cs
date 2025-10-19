using static SDL3.SDL;

using UO3D.Runtime.Renderer.Resources;

namespace UO3D.Runtime.SDL3GPU.Resources;

struct ShaderParameters
{
    public Dictionary<string, ShaderParameter> Inputs = [];

    public ShaderParameters()
    {

    }
}

internal class SDL3GPUShaderInstance: IShaderInstance
{
    public readonly ShaderParameters VertexParameters = new();
    public readonly ShaderParameters PixelParameters = new();

    public SDL3GPUShaderInstance(SDL3GPUShaderProgram vertexProgram, SDL3GPUShaderProgram pixelProgram)
    {
        VertexParameters.Inputs = vertexProgram.Parameters;
        PixelParameters.Inputs = pixelProgram.Parameters;
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
