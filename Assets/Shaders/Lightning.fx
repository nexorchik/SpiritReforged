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
float xMod;
float yMod;

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
    float dissipateProgress = max((Progress - 0.5f) * 2, 0);
    
    float2 texCoords = float2((input.TextureCoordinates.x * xMod) + uTime, (input.TextureCoordinates.y / yMod) + (0.5f - 0.5f / yMod));
    float2 noiseCoords = float2((input.TextureCoordinates.x + uTime / 3) * 30 * xMod, (input.TextureCoordinates.y + uTime) * yMod) / 25;
    
    float noiseStrength = (1 - tex2D(noiseSampler, noiseCoords).r) * 0.5f;
    float strength = pow(tex2D(textureSampler, texCoords).r, 2) * 3;
    
    //Fadeout if the absolute x value from the center is 0.95f or greater, 20 here is 1 / 0.05 (1 - the fadeout value)
    float xFadeOut = max(((GetAbsDistance(input.TextureCoordinates.x) - 0.8f) * 5), 0);
    strength *= pow(1 - xFadeOut, 1.5f);
    strength = pow(strength, 1 + xFadeOut);
    
    if (dissipateProgress > noiseStrength)
    {
        float dissipateStrength = min((dissipateProgress - noiseStrength) * 2, 1);
        return lerp(color * strength, float4(0, 0, 0, 0), dissipateStrength);
    }
    
    return color * strength;
}

technique BasicColorDrawing
{
    pass DefaultPS
	{
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
};