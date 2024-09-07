sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
matrix WorldViewProjection;
float4 RingColor;
float4 BloomColor;
float RingWidth;

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

const float ringBase = 0.5f;

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float4 finalColor;
    
    float distance = (2 * sqrt(pow(input.TextureCoordinates.x - 0.5, 2) + pow(input.TextureCoordinates.y - 0.5, 2)));
    float DistFromRingbase = abs(distance - ringBase) * 2;
    if (distance >= 1) //transparent if too much distance from center, as the shader is being applied to a square
        return float4(0, 0, 0, 0);
    
    else if (DistFromRingbase <= RingWidth * 0.5f) //always return peak opacity within the specified range
        finalColor = RingColor;
    
    else if (DistFromRingbase <= RingWidth * 0.75f) //interpolate to the bloom color between the given range
    {
        float lerpFactor = 1 - cos(3.14f * min((abs(DistFromRingbase - (RingWidth / 2)) / (RingWidth / 4)), 1)) / 2;
        finalColor = lerp(RingColor, BloomColor, lerpFactor);
    }
    
    else //interpolate to transparent if too far from the ring's edges
    {
        float lerpFactor = abs(DistFromRingbase - (RingWidth * 0.75f)) / (RingWidth);
        finalColor = lerp(BloomColor, float4(0, 0, 0, 0), lerpFactor);
    }
    
    finalColor *= input.Color;
    return finalColor * 2;
}

technique BasicColorDrawing
{
    pass PrimitiveTextureMap
	{
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
};