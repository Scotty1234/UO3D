
struct VsToPs
{
    float4 position : SV_Position;
    float2 uv: TEXCOORD0;
};

float4 main(in VsToPs input) : SV_TARGET
{
    return float4(1.0f, 0.0f, 0.0f, 1.0f);
}    