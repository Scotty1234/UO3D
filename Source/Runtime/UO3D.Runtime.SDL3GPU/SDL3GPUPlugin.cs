using Microsoft.Extensions.DependencyInjection;
using UO3D.Runtime.Plugin;
using UO3D.Runtime.RHI;

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
        services.AddSingleton<IRenderResourceFactory, SDL3GPUResourceFactory>();
        services.AddSingleton<Sdl3GpuDevice>();
    }

    public void Startup()
    {
        _renderer.Startup();
    }

    public void Shutdown()
    {
    }
}
