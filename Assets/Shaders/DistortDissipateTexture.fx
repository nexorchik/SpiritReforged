matrix WorldViewProjection;
texture uTexture;
sampler textureSampler = sampler_state
{
    Texture = (uTexture);
    AddressU = wrap;
    AddressV = clamp;
};
texture perlinNoise;
sampler noiseSampler = sampler_state
{
    Texture = (perlinNoise);
    AddressU = wrap;
    AddressV = wrap;
};

float Progress;
float uTime;
float4 uColor;
float xMod;
float yMod;
float distortion;
float texExponent;

struct VertexShaderInput
{
	float2 TextureCoordinates : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float2 TextureCoordinates : TEXCOORD0;
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    float4 pos = mul(input.Position, WorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;

	output.TextureCoordinates = input.TextureCoordinates;

    return output;
};

float GetAbsDistance(float inputCoordinate)
{
    return abs(inputCoordinate - 0.5f) * 2;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    
    float2 noiseCoords = float2(input.TextureCoordinates.x * xMod, input.TextureCoordinates.y * yMod);
    float noiseFactor = 2 * (tex2D(noiseSampler, noiseCoords) - 0.5f);
    float2 texCoords = float2(input.TextureCoordinates.x + (distortion * noiseFactor), input.TextureCoordinates.y + (distortion * noiseFactor));
    
    float strength = pow(tex2D(textureSampler, texCoords).r, texExponent);
    
    return color * strength * uColor;
}

technique BasicColorDrawing
{
    pass DefaultPS
	{
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
};