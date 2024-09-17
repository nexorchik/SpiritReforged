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
float2 textureStretch;
float2 noiseStretch;
float2 exponentRange;
float2 distortRange;

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
    float dissipateProgress = pow(Progress, 3); //quartic easing    
    
    float2 noiseCoords = float2((input.TextureCoordinates.x + (Progress / 8)) * noiseStretch.x, (input.TextureCoordinates.y) * noiseStretch.y);
    float noiseStrength = pow(tex2D(noiseSampler, noiseCoords).r, 3);
    float yDistortion = lerp(distortRange.x, distortRange.y, Progress) * (noiseStrength - 0.5f);
    yDistortion *= pow(1 - GetAbsDistance(input.TextureCoordinates.x), 2);
    
    float yCoord = input.TextureCoordinates.y + yDistortion - 0.5f;
    yCoord *= textureStretch.y;
    yCoord += 0.5f;
    float2 texCoords = float2((input.TextureCoordinates.x * textureStretch.x) + uTime, yCoord); //y coordinate math here is to center it on 0.5f
    
    float texExponent = lerp(exponentRange.x, exponentRange.y, dissipateProgress);
    float strength = pow(tex2D(textureSampler, texCoords).r, texExponent);
    
    //Quick flash of bloom based on vertical absolute distance and how much the animation has progressed, using cubic in easing
    float bloomStrength = pow((1 - GetAbsDistance(input.TextureCoordinates.y)), 3) * pow(1 - Progress, 3);
    strength += bloomStrength;
    
    //Fadeout if the absolute x value from the center is 0.8f or greater, 5 here is 1 / 0.2 (1 - the fadeout value)
    float xFadeOut = max(((GetAbsDistance(input.TextureCoordinates.x) - 0.8f) * 5), 0);
    strength *= pow(1 - xFadeOut, 1.5f);
    strength = pow(strength, 1 + xFadeOut);
    
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