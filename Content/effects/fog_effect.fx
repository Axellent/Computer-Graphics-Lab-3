float4x4 World;
float4x4 View;
float4x4 Projection;

float3 LightDirection = normalize(float3(-1, -1, -1));
float3 DiffuseLight = 1.25;
float3 AmbientLight = 0.25;
uniform const float3    DiffuseColor = 1;
uniform const float     Alpha = 1;
uniform const float3    EmissiveColor = 0;
uniform const float3    SpecularColor = 1;
uniform const float     SpecularPower = 16;
uniform const float3    EyePosition;
uniform const float     FogEnabled;
uniform const float     FogStart;
uniform const float     FogEnd;
uniform const float3    FogColor;
float3 cameraPos : CAMERAPOS;

texture Texture;

sampler Sampler = sampler_state{
    Texture = (Texture);
};

struct CommonVSOutput{
    float4  Pos_ws;
    float4  Pos_ps;
    float4  Diffuse;
    float3  Specular;
    float   FogFactor;
};

struct VertexLightingVSOutputTx{
    float4  PositionPS  : POSITION;
    float4  Diffuse     : COLOR0;
    float4  Specular    : COLOR1;
    float2  TexCoord    : TEXCOORD0;
};

struct VertexLightingPSInputTx{
    float4  Diffuse     : COLOR0;
    float4  Specular    : COLOR1;
    float2  TexCoord    : TEXCOORD0;
};

struct VSInputTx{
    float4  Position    : POSITION;
    float2  TexCoord    : TEXCOORD0;
};

float ComputeFogFactor(float d){
    return clamp((d - FogStart) / (FogEnd - FogStart), 0, 1) * FogEnabled;
}

CommonVSOutput ComputeCommonVSOutput(float4 position){
    CommonVSOutput out;

    float4 pos_ws = mul(position, World);
    float4 pos_vs = mul(pos_ws, View);
    float4 pos_ps = mul(pos_vs, Projection);

    out.Pos_ws = pos_ws;
    out.Pos_ps = pos_ps;

    out.Diffuse = float4(DiffuseColor.rgb + EmissiveColor, Alpha);
    out.Specular = 0;
    out.FogFactor = ComputeFogFactor(length(EyePosition - pos_ws));

    return out;
}

VertexLightingVSOutputTx VSBasicTx(VSInputTx in){
    VertexLightingVSOutputTx vlout;

    CommonVSOutput cout = ComputeCommonVSOutput(in.Position);

    vlout.PositionPS = cout.Pos_ps;
    vlout.Diffuse = cout.Diffuse;
    vlout.Specular = float4(cout.Specular, cout.FogFactor);
    vlout.TexCoord = in.TexCoord;

    return out;
}

float4 PSBasicTx(VertexLightingPSInputTx in) : COLOR{
    float4 color = tex2D(Sampler, in.TexCoord) * pin.Diffuse + float4(in.Specular.rgb, 0);
    color.rgb = lerp(color.rgb, FogColor, in.Specular.w);
	
    return color;
}

technique NoInstancing{
    pass Pass0
    {
        VertexShader = compile vs_4_0 VSBasicTx();
        PixelShader = compile ps_4_0 PSBasicTx();
    }
}