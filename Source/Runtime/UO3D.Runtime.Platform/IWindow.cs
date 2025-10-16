namespace UO3D.Runtime.Platform;

public interface IWindow
{
    public IntPtr Handle { get; }
    public uint Width { get; }
    public uint Height { get; }

}
