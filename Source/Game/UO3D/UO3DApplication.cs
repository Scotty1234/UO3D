using UO3D.Runtime.Core;
using UO3D.Runtime.Renderer;
using UO3D.Runtime.Renderer.Resources;

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
        var renderPassInfo = new RenderPassInfo
        {
            RenderTarget = null
        };

        context.BeginRenderPass(renderPassInfo);

        context.ShaderInstance = _shaderInstance;
        context.GraphicsPipline = _pipeline;

        context.DrawIndexedPrimitives(1);

        context.EndRenderPass();
    }

}
