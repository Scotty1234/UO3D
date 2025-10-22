using Microsoft.Extensions.DependencyInjection;

namespace UO3D.Runtime.Plugin;

public interface IPlugin
{
    static void ConfigureServices(IServiceCollection services){}

    void Startup(){}
    void PostStartup() { }
    void Shutdown() {}
}
