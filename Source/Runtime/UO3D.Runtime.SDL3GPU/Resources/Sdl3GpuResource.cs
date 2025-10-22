using static SDL3.SDL;

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

            _setNameFunc(Handle, Handle, _name);
        }
    }

    protected IntPtr Device { get; private set; }

    private bool _disposed;
    private readonly Action<IntPtr> _destroyFunc;
    private readonly Action<IntPtr, IntPtr, string> _setNameFunc;

    private string _name = "";


    protected Sdl3GpuResource(IntPtr device, Action<IntPtr> destroyFunc, Action<IntPtr, IntPtr, string> setNameFunc, string? debugName = null)
    {
        if (device == IntPtr.Zero)
            throw new ArgumentException("Device object handle cannot be null.");

        Device = device;

        _destroyFunc = destroyFunc ?? throw new ArgumentNullException(nameof(destroyFunc));
        _setNameFunc = setNameFunc ?? throw new ArgumentNullException(nameof(setNameFunc));

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

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (Handle != IntPtr.Zero)
        {
            _destroyFunc(Handle);

            Handle = IntPtr.Zero;
        }
    }

    ~Sdl3GpuResource()
    {
        Dispose(false);
    }
}
