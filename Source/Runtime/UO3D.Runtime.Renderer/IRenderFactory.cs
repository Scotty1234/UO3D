using UO3D.Runtime.Renderer.Resources;

namespace UO3D.Runtime.Renderer;

public interface IRenderResourceFactory
{
    public IRenderTexture CreateTexture(int width, int height);

    public IShaderInstance CreateShaderInstance(string vertexShader, string fragmentShader);
}
