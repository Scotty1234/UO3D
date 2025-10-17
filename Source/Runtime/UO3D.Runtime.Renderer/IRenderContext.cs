using UO3D.Runtime.Renderer.Resources;

namespace UO3D.Runtime.Renderer;

public interface IRenderContext
{
    public IRenderTexture RenderTarget { get; set; }
    public IShaderInstance ShaderInstance { get; set; }

    public void BeginRenderPass(in RenderPassInfo renderPassInfo);
    public void EndRenderPass();


    public void Draw();

    public void BindShader(IShaderInstance shaderInstance);
}
