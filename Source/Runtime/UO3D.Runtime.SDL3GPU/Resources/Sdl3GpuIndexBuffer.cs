using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.SDL3GPU.Resources;

internal class Sdl3GpuIndexBuffer: Sdl3GpuBuffer<ushort>, IRenderIndexBuffer
{
    public ushort[] Indices => Data;

    public Sdl3GpuIndexBuffer(Sdl3GpuDevice device, uint length, string? name = "")
        : base(device, RenderBufferType.Index, length, name)
    {

    }

}
