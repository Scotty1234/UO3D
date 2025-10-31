namespace UO3D.Runtime.RHI.Resources;

public interface IRenderTexture
{
    public string Name { get; set; }

    public void SetData(uint[] texels);
}
