sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
matrix WorldViewProjection;
texture uTexture;
sampler textureSampler = sampler_state
{
    Texture = (uTexture);
    AddressU = wrap;
    AddressV = wrap;
};
float2 scroll;
float2 textureStretch;
float2 texExponentRange;
float finalIntensityMod;
float4 uColor;
float4 uColor2;
float finalExponent;
float taperRatio;


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

float GetAngle(float2 input, float2 centeredPos, float baseAngle)
{
    return atan2(input.y - centeredPos.y, input.x - centeredPos.x) + baseAngle;
}

const float piOver4 = 0.785f;
float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float xCoord = input.TextureCoordinates.x - 0.5f;
    xCoord /= lerp(max(1 - input.TextureCoordinates.y, 0.01f), 1, taperRatio);
    xCoord += 0.5f;
    if (xCoord > 1 || xCoord < 0)
        return float4(0, 0, 0, 0);
    
    float noiseXCoord = ((xCoord - 0.5f) * textureStretch.x) + 0.5f;
    float2 noiseCoords = float2((input.TextureCoordinates.y * textureStretch.y) + scroll.y, noiseXCoord);
    
    float noiseExponent = lerp(texExponentRange.x, texExponentRange.y, input.TextureCoordinates.y);
    float strength = pow(tex2D(textureSampler, noiseCoords).r, noiseExponent);
    float absXDist = 1 - (abs(xCoord - 0.5f) * 2);
    float circularXDist = sqrt(1 - pow(absXDist - 1, 2));
    strength = strength/2 + circularXDist * 0.9f;
    strength *= pow(circularXDist, 2);
    strength *= input.TextureCoordinates.y;
    
    float4 finalColor = lerp(uColor, uColor2, min(max(1 - strength, 0), 1));
    strength = pow(min(strength, 1), finalExponent);
    
    return input.Color * finalIntensityMod * strength * finalColor;
}

technique BasicColorDrawing
{
    pass PrimitiveTextureMap
	{
        VertexShader = compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 MainPS();
    }
};