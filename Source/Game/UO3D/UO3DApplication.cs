using System.Numerics;
using System.Runtime.InteropServices;
using UO3D.Runtime.Core;
using UO3D.Runtime.RHI;
using UO3D.Runtime.RHI.Resources;

namespace UO3D;

[StructLayout(LayoutKind.Sequential, Pack = 16)]
struct PerViewData
{
    Matrix4x4 Projection;
}

internal class UO3DApplication: Application
{
    private RhiShaderResource _shaderResource = null!;
    private ShaderInstance _shaderInstance = null!;
    private ShaderBindingHandle _projectionBinding;
    private IGraphicsPipeline _pipeline = null!;

    protected override void Initialise()
    {
        var renderFactory = GetService<IRenderResourceFactory>();

        string vertexShader = @"D:\UODev\Work\UO3D\Source\Shaders\TexturedQuadVS.hlsl";
        string pixelShader = @"D:\UODev\Work\UO3D\Source\Shaders\TexturedQuadPS.hlsl";

        _shaderResource = renderFactory.NewShaderResource();
        _shaderResource.Load(vertexShader, pixelShader);

        _shaderInstance = renderFactory.NewShaderInstance(_shaderResource);

        _projectionBinding = _shaderInstance.GetBindingHandle(ShaderProgramType.Vertex, "Projection");

        _pipeline = renderFactory.CreateGraphicsPipeline(new GraphicsPipelineDescription
        {
            Name = "TestPipeline",
            ShaderResource = _shaderResource
        });
    }

    protected override void BeginDraw(IRenderContext context)
    {
        Matrix4x4 projection = Matrix4x4.Identity;

        _shaderInstance.SetParameter(_projectionBinding, projection);

        context.ShaderInstance = _shaderInstance;
        context.GraphicsPipline = _pipeline;

        context.DrawIndexedPrimitives(1);
    }

}
