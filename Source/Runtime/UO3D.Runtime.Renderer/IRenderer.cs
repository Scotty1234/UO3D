namespace UO3D.Runtime.Renderer;

public interface IRenderer
{
    public event Action<IRenderContext>? OnFrameBegin;
    public event Action<IRenderContext>? OnFrameEnd;

    public void RaiseFrameBegin();

    public void RaiseFrameEnd();
}
