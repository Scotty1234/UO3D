using Hexa.NET.ImGui;

using UO3D.Runtime.Core;
using UO3D.Runtime.Plugin;
using UO3D.Runtime.Renderer;
using UO3D.Runtime.RHI;
using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.UI;

public class UIPlugin : IPlugin
{
    private readonly RenderSystem _rendererSystem;
    private readonly IRenderResourceFactory _renderFactory;

    private readonly List<IRenderTexture> _textures = [];

    public UIPlugin(RenderSystem renderer, IRenderResourceFactory renderFactory, ApplicationLoop applicationLoop)
    {
        _rendererSystem = renderer;
        _renderFactory = renderFactory;

        _rendererSystem.OnFrameBegin += OnFrameBegin;

        _rendererSystem.OnFrameEnd += OnFrameEnd;

        applicationLoop.OnUpdate += Update;
    }

    public void Startup()
    {
        var context = ImGui.CreateContext();

        ImGui.SetCurrentContext(context);

        RebuildFontAtlas();
    }

    private void Update(float time)
    {
        var io = ImGui.GetIO();

        io.DisplaySize = new System.Numerics.Vector2(1, 1);
        io.DisplayFramebufferScale = new System.Numerics.Vector2(1f, 1f);
    }

    private void OnFrameBegin(IRenderContext renderContext)
    {
        ImGui.NewFrame();
    }

    private void OnFrameEnd(IRenderContext renderContext)
    {
        ImGui.EndFrame();
        ImGui.Render();

        ImDrawDataPtr drawData = ImGui.GetDrawData();

        renderContext.BeginRenderPass(new RenderPassInfo
        {
            Name = "UI",
            RenderTarget = _rendererSystem.UIOverlay
        });

        renderContext.EndRenderPass();
    }

    public unsafe void RebuildFontAtlas()
    {
        var io = ImGui.GetIO();

        var texData = io.Fonts.TexData;

        //var tex2d = _renderFactory.CreateTexture((uint)texData.Width, (uint)texData.Height);

        //tex2d.SetDataPointer((UIntPtr)pixelData, width * height * bytesPerPixel);

        //_textures.Add(tex2d);

        // Let ImGui know where to find the texture
        io.Fonts.ClearTexData(); // Clears CPU side texture data
    }
}
