using UO3D.Runtime.Core;
using UO3D.Runtime.RHI;
using UO3D.Runtime.RHI.Resources;

namespace UO3D;

internal class UO3DApplication: Application
{
    private IShaderInstance _shaderInstance = null!;
    private IGraphicsPipeline _pipeline = null!;

    protected override void Initialise()
    {
        var renderFactory = GetService<IRenderResourceFactory>();

        string vertexShader = @"D:\UODev\Work\UO3D\Source\Shaders\TexturedQuadVS.hlsl";
        string pixelShader = @"D:\UODev\Work\UO3D\Source\Shaders\TexturedQuadPS.hlsl";

        _shaderInstance = renderFactory.CreateShaderInstance(vertexShader, pixelShader);

        _pipeline = renderFactory.CreateGraphicsPipeline(_shaderInstance);

    }

    protected override void BeginDraw(IRenderContext context)
    {
        context.ShaderInstance = _shaderInstance;
        context.GraphicsPipline = _pipeline;

        context.DrawIndexedPrimitives(1);
    }

}
