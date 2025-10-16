namespace UO3D.Runtime.Core;

public class ApplicationLoop
{
    public event Action<TimeSpan>? OnUpdate;

    internal void Update(TimeSpan time)
    {
        OnUpdate?.Invoke(time);
    }
}
