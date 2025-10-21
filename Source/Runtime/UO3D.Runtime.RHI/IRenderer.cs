namespace UO3D.Runtime.RHI;

public interface IRenderer
{
    public event Action<IRenderContext>? OnFrameBegin;
    public event Action<IRenderContext>? OnFrameEnd;

    public void FrameBegin();

    public void FrameEnd();
}
