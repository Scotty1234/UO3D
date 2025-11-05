namespace UO3D.Runtime.RHI.Resources;

public interface IRenderTexture
{
    public string Name { get; set; }

    public IntPtr Handle { get; }

    public void SetData(uint[] texels);
}
