
struct VsToPs
{
    float4 position : SV_Position;
    float2 uv: TEXCOORD0;
    uint instanceIndex: SV_InstanceID;
};

static const float width = 1.0f;//44.0;
static const float height = 1.0f;//44.0;

static const float4 cQuadVertsNDC[4] =
{
    {   0.0,   0.0,  0.0, 1.0},
    {   0.0, width,  0.0, 1.0},
    { width, width,  0.0, 1.0},
    { width,   0.0,  0.0, 1.0},
};

static const float2 cQuadUVs[4] =
{
    { 1.0,  0.0 },
    { 0.0,  0.0 },
    { 1.0,  1.0 },
    { 0.0,  1.0 },
};

VsToPs main(uint vid : SV_VertexID, uint instance_id : SV_InstanceID)
{
    VsToPs output;

    float4 vert = cQuadVertsNDC[vid];

    output.position = vert;
    output.position.w = 1.0f;
    output.uv = cQuadUVs[vid];
    output.instanceIndex = instance_id;

    return output;
}
