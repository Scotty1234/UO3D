using Microsoft.Extensions.DependencyInjection;
using UO3D.Runtime.Plugin;
using UO3D.Runtime.Renderer;

namespace UO3D.Runtime.SDL3GPU;

public class SDL3GPUPlugin: IPlugin
{
    private readonly SDL3GPURenderer _renderer;

    public SDL3GPUPlugin(IRenderer renderer)
    {
        _renderer = renderer as SDL3GPURenderer;
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IRenderer, SDL3GPURenderer>();
    }

    public void Startup()
    {
        _renderer.Startup();
    }

    public void Shutdown()
    {
    }
}
