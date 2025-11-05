using System.Diagnostics;
using UO3D.Runtime.RHI.Resources;
using UO3D.Runtime.SDL3GPU.Resources;

namespace UO3D.Runtime.SDL3GPU;

internal class Sdl3GpuGlobalSamplers
{
    private readonly Sdl3GpuDevice _device;
    private readonly Dictionary<int, Sdl3GpuSampler> _globalSamplers = [];

    public Sdl3GpuGlobalSamplers(Sdl3GpuDevice device)
    {
        _device = device;

        RegisterGlobalSampler(new RhiSampler { Filter = SamplerFilter.Point });
    }

    public Sdl3GpuSampler GetSampler(RhiSampler rhiSampler)
    {
        Debug.Assert(rhiSampler.Filter != SamplerFilter.Invalid);

        return _globalSamplers[rhiSampler.GetHashCode()];
    }

    private void RegisterGlobalSampler(RhiSampler rhiSampler)
    {
        var globalSampler = new Sdl3GpuSampler(_device, rhiSampler);

        _globalSamplers.Add(globalSampler.Description.GetHashCode(), globalSampler);
    }
}
