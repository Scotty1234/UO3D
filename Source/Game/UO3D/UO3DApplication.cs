using System.Numerics;
using System.Runtime.InteropServices;
using UO3D.Runtime.Core;
using UO3D.Runtime.RHI;
using UO3D.Runtime.RHI.Resources;

namespace UO3D;

internal class UO3DApplication: Application
{
    private RhiShaderResource _shaderResource = null!;
    private ShaderInstance _shaderInstance = null!;
    private ShaderBindingHandle _projectionBinding;
    private IGraphicsPipeline _pipeline = null!;
    private IRenderTexture _whiteTexture = null!;
    ShaderBindingHandle _textureBindingHandle = null!;

    protected override void Initialise()
    {
        var renderFactory = GetService<IRenderResourceFactory>();

        string vertexShader = @"D:\UODev\Work\UO3D\Source\Shaders\TexturedQuadVS.hlsl";
        string pixelShader = @"D:\UODev\Work\UO3D\Source\Shaders\TexturedQuadPS.hlsl";

        _shaderResource = renderFactory.NewShaderResource();
        _shaderResource.Load(vertexShader, pixelShader);

        _shaderInstance = renderFactory.NewShaderInstance(_shaderResource);

        _textureBindingHandle = _shaderInstance.GetBindingHandleTexturePixel("Texture");

        _whiteTexture = renderFactory.CreateTexture(22, 22);

        uint[] white = new uint[22 * 22];

        _whiteTexture.SetData(white);

        //_projectionBinding = _shaderInstance.GetBindingHandle(ShaderProgramType.Vertex, "Projection");

        _pipeline = renderFactory.CreateGraphicsPipeline(new GraphicsPipelineDescription
        {
            Name = "TestPipeline",
            ShaderResource = _shaderResource
        });
    }

    protected override void BeginDraw(IRenderContext context)
    {
        Matrix4x4 projection = Matrix4x4.Identity;

        context.MVP = new ModelViewProjection
        {
            Projection = Matrix4x4.Identity,
            View = Matrix4x4.CreateTranslation(-0.5f, -0.5f, 0.0f)
        };

        _shaderInstance.SetParameter()

        context.GraphicsPipline = _pipeline;
        context.ShaderInstance = _shaderInstance;

        context.DrawIndexedPrimitives(1);
    }

}
