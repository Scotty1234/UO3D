using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.RHI;

public interface IRenderResourceFactory
{
    public IRenderTexture CreateTexture(int width, int height);

    public IShaderInstance CreateShaderInstance(string vertexShader, string fragmentShader);

    public IGraphicsPipeline CreateGraphicsPipeline(IShaderInstance shaderInstance);
}
