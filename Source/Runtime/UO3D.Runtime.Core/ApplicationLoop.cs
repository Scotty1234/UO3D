﻿namespace UO3D.Runtime.Core;

public class ApplicationLoop
{
    public event Action<float>? OnUpdate;

    internal void Update(float time)
    {
        OnUpdate?.Invoke(time);
    }
}
