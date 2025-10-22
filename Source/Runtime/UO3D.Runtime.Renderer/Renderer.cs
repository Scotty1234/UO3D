using UO3D.Runtime.Renderer.Resources;
using UO3D.Runtime.RHI;
using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.Renderer;

public class RenderSystem
{
    public event Action<IRenderContext>? OnFrameBegin;
    public event Action<IRenderContext>? OnFrameEnd;

    public readonly RenderTarget GBufferDiffuse = new();

    private IRenderContext _context = null!;
    private readonly IRenderer _rhiRenderer = null!;
    private readonly IRenderResourceFactory _resourceFactory;

    public RenderSystem(IRenderer rhiRenderer, IRenderResourceFactory resourceFactory)
    {
        _rhiRenderer = rhiRenderer;
        _resourceFactory = resourceFactory;
    }

    public void Startup()
    {
        _context = _rhiRenderer.CreateRenderContext();
        var gBufferTexture = _resourceFactory.CreateTexture(1920, 1080);

        GBufferDiffuse.Setup(gBufferTexture);

    }

    public void FrameBegin()
    {
        _context.BeginRecording();

        //_swapChain.Acquire(_context);

        _context.BeginRenderPass(new RenderPassInfo
        {
            RenderTarget = GBufferDiffuse,
        });

        OnFrameBegin?.Invoke(_context);
    }

    public void FrameEnd()
    {
        OnFrameEnd?.Invoke(_context);

        _context.EndRenderPass();
        _context.EndRecording();
    }

    public void ResizeSwapchain(uint width,  uint height)
    {

    }
}
