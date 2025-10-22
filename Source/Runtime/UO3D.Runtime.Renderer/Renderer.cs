using UO3D.Runtime.Renderer.Resources;
using UO3D.Runtime.RHI;
using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.Renderer;

public class RenderSystem
{
    public event Action<IRenderContext>? OnFrameBegin;
    public event Action<IRenderContext>? OnFrameEnd;

    public RenderTarget GBufferDiffuse;

    private IRenderContext _context = null!;
    private readonly IRenderer _rhiRenderer;
    private readonly IRenderResourceFactory _resourceFactory;

    private RenderPassInfo _mainPass;

    private uint _frameNumber = 0;

    public RenderSystem(IRenderer rhiRenderer, IRenderResourceFactory resourceFactory)
    {
        _rhiRenderer = rhiRenderer;
        _resourceFactory = resourceFactory;

        _mainPass = new RenderPassInfo
        {
            RenderTarget = GBufferDiffuse,
            Name = "MainPass"
        };
    }

    public void Startup()
    {
        _context = _rhiRenderer.CreateRenderContext();
    }

    public void FrameBegin()
    {
        _context.BeginRecording();

        GBufferDiffuse = _rhiRenderer.SwapChain.Acquire(_context);

        _mainPass.RenderTarget = GBufferDiffuse;

        _context.BeginRenderPass(_mainPass);

        OnFrameBegin?.Invoke(_context);
    }

    public void FrameEnd()
    {
        OnFrameEnd?.Invoke(_context);

        _context.EndRenderPass();
        _context.EndRecording();

        _frameNumber++;
    }

    public void ResizeSwapchain(uint width,  uint height)
    {

    }
}
