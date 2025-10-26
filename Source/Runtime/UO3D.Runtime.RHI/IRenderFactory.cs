using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.RHI;

public interface IRenderResourceFactory
{
    public RhiShaderResource NewShaderResource();
    public ShaderInstance NewShaderInstance(RhiShaderResource shaderResource);

    public IRenderTexture CreateTexture(uint width, uint height);

    public IGraphicsPipeline CreateGraphicsPipeline(in GraphicsPipelineDescription graphicsPipelineDescription);

    public IRenderIndexBuffer CreateIndexBuffer(uint length, string name);
}
