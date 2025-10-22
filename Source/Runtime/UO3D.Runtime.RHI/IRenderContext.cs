using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.RHI;

public interface IRenderContext
{
    //public IRenderTexture RenderTarget { get; set; }
    public IShaderInstance ShaderInstance { get; set; }

    public IGraphicsPipeline GraphicsPipline { get; set; }

    public void BeginRenderPass(in RenderPassInfo renderPassInfo);
    public void EndRenderPass();

    public void BeginRecording();
    public void EndRecording();

    public void DrawIndexedPrimitives(uint numInstances);

}
