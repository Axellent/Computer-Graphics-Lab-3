float4x4 World;
float4x4 View;
float4x4 Projection;

float4 FogColor = float4(1, 1, 1, 1);
float FogStart = 400;
float FogEnd = 1000;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.2;

float4x4 WorldInverseTranspose;

float3 DiffuseLightDirection = float3(1, 1, 1);
float4 DiffuseColor = float4(0, 0, 1, 1);
float DiffuseIntensity = 1.0;

float3 DiffuseLightDirection2 = float3(-1, 0, 0);
float4 DiffuseColor2 = float4(0.5, 0, 0, 1);
float DiffuseIntensity2 = 1.0;

float Shininess = 200;
float4 SpecularColor = float4(1, 1, 1, 1);
float SpecularIntensity = 1;
float3 ViewVector = float3(1, 0, 0);

float viewDistance = 800;
float3 cameraPos;

texture ModelTexture;
sampler2D textureSampler = sampler_state {
	Texture = (ModelTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

float BumpConstant = 1;
texture NormalMap;
sampler2D bumpSampler = sampler_state {
	Texture = (NormalMap);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 Tangent : TEXCOORD2;
	float3 Binormal : TEXCOORD3;
	float4 Position3D: TEXCOORD4;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose));
	output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose));
	output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose));

	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float ComputeFogFactor(float d)
{
    return clamp((d - FogStart) / (FogEnd - FogStart), 0, 1);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// Calculate the normal, including the information in the bump map
	float3 bump = BumpConstant * (tex2D(bumpSampler, input.TextureCoordinate) - (0.5, 0.5, 0.5));
	float3 bumpNormal = input.Normal + (bump.x * input.Tangent + bump.y * input.Binormal);
	bumpNormal = normalize(bumpNormal);

	// Calculate the diffuse light component with the bump map normal
	float diffuseIntensity = dot(normalize(DiffuseLightDirection), bumpNormal);
	if (diffuseIntensity < 0)
		diffuseIntensity = 0;

	// Calculate the diffuse light2 component with the bump map normal
	float diffuseIntensity2 = dot(normalize(DiffuseLightDirection2), bumpNormal);
	if (diffuseIntensity2 < 0)
		diffuseIntensity2 = 0;

	// Calculate the specular light component with the bump map normal
	float3 light = normalize(DiffuseLightDirection);
	float3 r = normalize(2 * dot(light, bumpNormal) * bumpNormal - light);
	float3 v = normalize(mul(normalize(ViewVector), World));
	float dotProduct = dot(r, v);

	// Calculate the specular light component with the bump map normal
	float3 light2 = normalize(DiffuseLightDirection2);
	float3 r2 = normalize(2 * dot(light2, bumpNormal) * bumpNormal - light2);
	float3 v2 = normalize(mul(normalize(ViewVector), World));
	float dotProduct2 = dot(r2, v2);
	
	float4 specular = SpecularIntensity * SpecularColor * max(pow(abs(dotProduct), Shininess), 0) * diffuseIntensity;
	float4 specular2 = SpecularIntensity * SpecularColor * max(pow(abs(dotProduct2), Shininess), 0) * diffuseIntensity2;

	// Calculate the texture color
	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
	clip(textureColor.a - 0.2f);
	textureColor.a = 0.55;

	float dist = distance(cameraPos, input.Position3D);

	float fogFactor = ComputeFogFactor(dist);

	textureColor.rgb = lerp(textureColor.rgb, FogColor, fogFactor);

	//textureColor.rgb = lerp(textureColor.rgb, fogColor * textureColor.a, fogFactor);

	// Combine all of these values into one (including the ambient light)
	return saturate(textureColor * (diffuseIntensity +diffuseIntensity2)+AmbientColor * AmbientIntensity + specular + specular2);
}

technique BumpMapped
{
	pass Pass1
	{
		AlphaBlendEnable = TRUE;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}