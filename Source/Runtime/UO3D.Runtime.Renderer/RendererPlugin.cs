using Microsoft.Extensions.DependencyInjection;

using UO3D.Runtime.Plugin;

namespace UO3D.Runtime.Renderer;

public class RendererPlugin : IPlugin
{
    private readonly RenderSystem _renderSystem;

    public RendererPlugin(RenderSystem renderSystem)
    {
        _renderSystem = renderSystem;
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<RenderSystem>();
    }

    public void PostStartup()
    {
        _renderSystem.Startup();
    }
}
