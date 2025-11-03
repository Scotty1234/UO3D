using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace UO3D.Runtime.RHI.Resources;

//[StructLayout(LayoutKind.Explicit)]
public struct ShaderBindingData
{
    //[FieldOffset(0)]
    public RhiSampler Sampler;

    //[FieldOffset(0)]
    public readonly byte[] Buffer;

    //[FieldOffset(0)]
    public IRenderTexture Texture;

    public ShaderBindingData(uint bufferLength)
    {
        Buffer = new byte[bufferLength];
    }
}

[DebuggerDisplay("{BindingIndex}, {InputType}")]
public struct ShaderBindingDataEntry
{
    public readonly uint BindingIndex;
    public readonly RhiShaderInputType InputType;
    public ShaderBindingData Data;

    public ShaderBindingDataEntry(RhiShaderInputType inputType, uint index, uint dataSize)
    {
        InputType = inputType;
        BindingIndex = index;

        if(InputType == RhiShaderInputType.Buffer)
        {
            Debug.Assert(dataSize > 0);

            Data = new ShaderBindingData(dataSize);
        }
    }

    public void SetSampler(RhiSampler sampler)
    {
        Debug.Assert(InputType == RhiShaderInputType.Sampler);

        Data.Sampler = sampler;
    }

    public void SetTexture(IRenderTexture texture)
    {
        Debug.Assert(InputType == RhiShaderInputType.Texture);

        Data.Texture = texture;
    }

    public void SetBuffer<T>(T value) where T: struct
    {
        Debug.Assert(InputType == RhiShaderInputType.Buffer);

        MemoryMarshal.Write(Data.Buffer.AsSpan(), value);
    }
}

public struct ShaderProgramBindings1
{
    public ShaderBindingDataEntry[] Bindings;
}

public class ShaderInstance
{
    public readonly ShaderProgramBindings1[] BindingData = new ShaderProgramBindings1[(int)ShaderProgramType.Count];

    private readonly RhiShaderResource _shaderResource;

    public ShaderInstance(RhiShaderResource shaderResource)
    {
        _shaderResource = shaderResource;

        for (int i = 0; i < _shaderResource.ProgramBindings.Length; i++)
        {
            ref var bindingsForProgram = ref _shaderResource.ProgramBindings[i];

            if(bindingsForProgram.Parameters == null)
            {
                continue;
            }

            BindingData[i].Bindings = new ShaderBindingDataEntry[bindingsForProgram.Parameters.Length];

            ref var bindings = ref BindingData[i].Bindings;
           
            for(int bindEntryIndex = 0; bindEntryIndex < bindings.Length; bindEntryIndex++)
            {
               ref var bindInfo = ref bindingsForProgram.Parameters[bindEntryIndex];

                bindings[bindEntryIndex] = new ShaderBindingDataEntry(bindInfo.InputType, bindInfo.SlotIndex, bindInfo.Size);

            }
        }
    }

    public uint GetNumTextures(ShaderProgramType programType)
    {
        return _shaderResource.GetNumTextures(programType);
    }

    public uint GetNumSamplers(ShaderProgramType programType)
    {
        return _shaderResource.GetNumSamplers(programType);
    }

    public ShaderBindingHandle GetBindingHandleTexturePixel(string name)
    {
        return GetBindingHandle(ShaderProgramType.Pixel, RhiShaderInputType.Texture, name);
    }

    public ShaderBindingHandle GetBindingHandleSamplerPixel(string name)
    {
        return GetBindingHandle(ShaderProgramType.Pixel, RhiShaderInputType.Sampler, name);
    }

    public ShaderBindingHandle GetBindingHandle(ShaderProgramType programType, RhiShaderInputType inputType, string name)
    {
        return _shaderResource.GetBindingHandle(programType, inputType, name);
    }

    public void SetParameter(ShaderBindingHandle bindingHandle, in Matrix4x4 matrix)
    {
        GetBindingData(bindingHandle, out var entry);

        entry.SetBuffer(matrix);
    }

    public void SetTexture(ShaderBindingHandle bindingHandle, IRenderTexture texture)
    {
        GetBindingData(bindingHandle, out var entry);

        entry.SetTexture(texture);
    }

    public void SetSampler(ShaderBindingHandle bindingHandle, RhiSampler sampler)
    {
        GetBindingData(bindingHandle, out var entry);

        entry.SetSampler(sampler);
    }

    private void GetBindingData(ShaderBindingHandle bindingHandle, out ShaderBindingDataEntry entry)
    {
        entry = BindingData[(int)bindingHandle.ProgramType].Bindings[bindingHandle.Handle];
    }
}
