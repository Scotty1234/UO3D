using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.RHI;

public interface IRenderResourceFactory
{
    public IRenderTexture CreateTexture(uint width, uint height);

    public IShaderInstance CreateShaderInstance(string vertexShader, string fragmentShader);

    public IGraphicsPipeline CreateGraphicsPipeline(IShaderInstance shaderInstance, string name);

    public IRenderIndexBuffer CreateIndexBuffer(uint length, string name);
}
