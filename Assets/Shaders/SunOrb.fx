sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
matrix WorldViewProjection;
float4 lightColor;
float4 darkColor;

texture uTexture;
sampler textureSampler = sampler_state
{
    Texture = (uTexture);
    AddressU = wrap;
    AddressV = wrap;
};
texture noiseTexture;
sampler noiseSampler = sampler_state
{
    Texture = (noiseTexture);
    AddressU = wrap;
    AddressV = wrap;
};
texture rayTexture;
sampler raySampler = sampler_state
{
    Texture = (rayTexture);
    AddressU = wrap;
    AddressV = wrap;
};

float2 textureStretch;
float2 rayStretch;
float2 scroll;
float2 rayScroll;
float intensity;
float rayIntensity;

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

float GetDistance(float2 input)
{
    return (2 * sqrt(pow(input.x - 0.5f, 2) + pow(input.y - 0.5f, 2)));
}

float GetAngle(float2 input)
{
    return atan2(input.y - 0.5f, input.x - 0.5f) + 3.14f;
}

float2 GetNoiseCoords(float2 inputCoords, float distance, float2 stretch, float2 uScroll)
{
    float2 noiseCoords = float2((GetAngle(inputCoords) / 6.28f), distance) + uScroll;
    noiseCoords.y -= 0.5f;
    noiseCoords *= stretch;
    noiseCoords.y += 0.5f;
    
    return noiseCoords;
}

float4 ApplyNoise(float2 inputCoords, float distance, float texStrength)
{
    float2 noiseCoords = GetNoiseCoords(inputCoords, distance, textureStretch, scroll);
    float noiseStrength = pow(tex2D(noiseSampler, noiseCoords).r, 2) * (1 - texStrength) * pow(1 - distance, 3);
    return lerp(darkColor, lightColor, pow(noiseStrength, 0.2f)) * noiseStrength * 2;
}

float4 ApplyRay(float2 inputCoords, float distance, float texStrength)
{
    float2 noiseCoords = GetNoiseCoords(inputCoords, distance, rayStretch, rayScroll);
    float noiseStrength = pow(tex2D(raySampler, noiseCoords).r, lerp(0.5f, 10, pow(distance, 2))) * pow(1 - distance, 1.5f);
    return lightColor * noiseStrength * rayIntensity;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float distance = GetDistance(input.TextureCoordinates);
    if (distance >= 1) 
        return float4(0, 0, 0, 0);
    
    float texStrength = sqrt(tex2D(textureSampler, input.TextureCoordinates).r);
    float distMod = (1 - pow(distance, 2));
    texStrength = texStrength * distMod;
    float strength = texStrength * distMod;
    
    float4 finalColor = lerp(darkColor, lightColor, pow(strength, 0.8f)) * strength * distMod;
    finalColor += ApplyNoise(input.TextureCoordinates, distance, texStrength);
    finalColor = pow(finalColor, 1.5f);
    finalColor += ApplyRay(input.TextureCoordinates, distance, texStrength);

    return finalColor * intensity * input.Color;
}

technique BasicColorDrawing
{
    pass GeometricStyle
	{
        VertexShader = compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 MainPS();
    }
};