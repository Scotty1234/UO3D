namespace UO3D.Runtime.SDL3GPU.Resources;

internal abstract class Sdl3GpuResource : IDisposable
{
    public IntPtr Handle { get; protected set; }
   

    public string Name
    {
        get => _name;
        set
        {
            _name = value;

            (_setNameFunc ?? throw new Exception("_setNameFunc not set"))(Handle, Handle, _name);
        }
    }

    public readonly Sdl3GpuDevice Device;

    private readonly Action<IntPtr, IntPtr, string>? _setNameFunc;

    private bool _disposed;
    private string _name = "";

    protected Sdl3GpuResource(Sdl3GpuDevice device, Action<IntPtr, IntPtr, string>? setNameFunc = null, string? debugName = null)
    {
        Device = device;

        _setNameFunc = setNameFunc;

        if (debugName != null)
        {
            Name = debugName;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected abstract void FreeResource();

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (Handle != IntPtr.Zero)
        {
            FreeResource();

            Handle = IntPtr.Zero;
        }
    }

    ~Sdl3GpuResource()
    {
        Dispose(false);
    }
}
