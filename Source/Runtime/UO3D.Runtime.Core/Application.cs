using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Xna.Framework;
using UO3D.Runtime.Platform;
using UO3D.Runtime.Plugin;
using UO3D.Runtime.Renderer;

namespace UO3D.Runtime.Core;

public class Application: IDisposable
{
    public static string? ExePath => Assembly.GetEntryAssembly().Location;

    public static string? BaseDirectory => Path.GetDirectoryName(ExePath);
    public static string? PluginDirectory => Path.Combine(BaseDirectory, "Plugins");

    private readonly ServiceCollection _services = new ServiceCollection();
    private ServiceProvider? _serviceProvider;

    private ApplicationLoop _applicationLoop = null!;
    private IRenderer _renderer = null!; 

    private CameraEntity _camera = null!;

    private Window _window = new();

    private bool _runApplication = true;

    public void RegisterPlugin<T>() where T : IPlugin
    {
        _services.AddSingleton(typeof(IPlugin), typeof(T));
    }

    public void Start()
    {
        InitialiseInternal();
        Initialise();

        GameTime gameTime = new GameTime();

        while(_runApplication)
        {

            Update(gameTime);
            BeginDraw();
            EndDraw();
        }

        _window.Dispose();

        var plugins = _serviceProvider.GetServices<IPlugin>();

        foreach (var plugin in plugins)
        {
            plugin.Shutdown();
        }
    }

    public T GetService<T>()
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    virtual protected void Initialise()
    {

    }

    private void InitialiseInternal()
    {
        _window.Startup();

        _services.AddSingleton<EntityManager>();
        _services.AddSingleton<ApplicationLoop>();
        _services.AddSingleton<Input>();
        _services.AddSingleton<IWindow>(_window);

        LoadPlugins(BaseDirectory);
        LoadPlugins(PluginDirectory, true);

        _serviceProvider = _services.BuildServiceProvider();

        _applicationLoop = GetService<ApplicationLoop>();
        _renderer = GetService<IRenderer>();

        var plugins = _serviceProvider.GetServices<IPlugin>();

        foreach (var plugin in plugins)
        {
            plugin.Startup();
        }

        var entityManager = GetService<EntityManager>();

        _camera = entityManager.NewEntity<CameraEntity>();
    }

    private void Update(GameTime gameTime)
    {
        if(_window.PollEvents())
        {
            _runApplication = false;

            return;
        }

        _applicationLoop.Update(gameTime.ElapsedGameTime);
    }

    private bool BeginDraw()
    {
        //_camera.SetProjection(bounds.Width, bounds.Height, -1.0f, 1.0f);
        _camera.SetProjection(1, 1, -1.0f, 1.0f);

        //_renderer.RenderContext.View = _camera.Projection * _camera.View;

        _renderer.FrameBegin();

        return true;
    }

    private void EndDraw()
    {
        _renderer.FrameEnd();
    }

    private void LoadPlugins(string directory, bool recurse = false)
    {
        if(Directory.Exists(directory) == false)
        {
            return;
        }

        if (recurse)
        {
            foreach (var subdir in Directory.GetDirectories(directory))
            {
                LoadPlugins(subdir, true);
            }
        }

        foreach (var dll in Directory.GetFiles(directory, "*.dll"))
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(dll);
            }
            catch
            {
                continue; // skip invalid DLLs
            }

            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch
            {
                continue;
            }

            foreach (var type in types)
            {
                if (typeof(IPlugin).IsAssignableFrom(type) == false)
                {
                    continue; // skip non-plugins
                }

                if (type.IsAbstract || !type.IsClass)
                {
                    continue;
                }

                Console.WriteLine($"Loading plugin {dll}");

                var configureServicesMethod = type.GetMethod("ConfigureServices");

                configureServicesMethod?.Invoke(null, [_services]);

                _services.AddSingleton(typeof(IPlugin), type);
            }
        }
    }
    public void Dispose()
    {
    }
}
