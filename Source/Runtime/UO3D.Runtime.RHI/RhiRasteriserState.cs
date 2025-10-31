
namespace UO3D.Runtime.RHI;

public enum RhiCullMode
{
    Disable,
    Back,
    Front
}

public enum RhiFillMode
{
    Fill,
    Wireframe
}

public struct RhiRasteriserState
{
    public RhiCullMode CullMode { get; set; } = RhiCullMode.Disable;
    public RhiFillMode FillMode { get; set; } = RhiFillMode.Fill;

    public RhiRasteriserState()
    {

    }
}
