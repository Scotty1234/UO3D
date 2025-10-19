using Microsoft.Xna.Framework.Graphics;
using UO3D.Runtime.Core;
using UO3D.Runtime.Renderer;
using UO3D.Runtime.Renderer.Resources;

namespace UO3D;

internal class UO3DApplication: Application
{
    private IShaderInstance _shaderInstance = null!;

    protected override void Initialise()
    {
        var renderFactory = GetService<IRenderResourceFactory>();

        string vertexShader = @"D:\UODev\Work\UO3D\Source\Shaders\TexturedQuadVS.hlsl";
        string fragmentShader = @"D:\UODev\Work\UO3D\Source\Shaders\TexturedQuadPS.hlsl";

        _shaderInstance = renderFactory.CreateShaderInstance(vertexShader, fragmentShader);
    }

}
