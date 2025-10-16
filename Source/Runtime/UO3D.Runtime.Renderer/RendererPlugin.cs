using Microsoft.Extensions.DependencyInjection;

using UO3D.Runtime.Plugin;

namespace UO3D.Runtime.Renderer;

public class RendererPlugin : IPlugin
{
    public static void ConfigureServices(IServiceCollection services)
    {
    }

    public void Startup(IServiceProvider serviceProvider)
    {
    }
}
