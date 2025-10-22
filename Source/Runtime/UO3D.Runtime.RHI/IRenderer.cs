using UO3D.Runtime.RHI.Resources;

namespace UO3D.Runtime.RHI;

public interface IRenderer
{
    public IRenderSwapChain SwapChain { get; }

    public IRenderContext CreateRenderContext();

}
