sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
matrix WorldViewProjection;
texture baseTexture;
sampler baseSampler = sampler_state
{
    Texture = (baseTexture);
    AddressU = wrap;
    AddressV = wrap;
};
float4 baseColorDark;
float4 baseColorLight;

float2 textureExponent;

float2 coordMods;
float timer;
float progress;
float intensity;
float opacity;

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
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, WorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;

    output.TextureCoordinates = input.TextureCoordinates;

    return output;
};

const float FadeOutRangeX = 0.9f;
const float FadeOutRangeY = 1;

float4 FadeOutColor(float4 inputColor, float inputStrength, float fadeAmount)
{
    return inputColor * lerp(1, inputStrength, fadeAmount);
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    float4 textureColor;
    float strength = 1;
    
    float2 baseTexCoords = float2((input.TextureCoordinates.x - timer) * coordMods.x, input.TextureCoordinates.y * coordMods.y);
    
    //add base texture color
    float samplerStrength = tex2D(baseSampler, baseTexCoords).r;
    float texExponent = lerp(textureExponent.x, textureExponent.y, progress);
    textureColor = lerp(baseColorLight, baseColorDark, 1 - samplerStrength) * pow(samplerStrength, texExponent);
    
    //fade out based on position
    if (input.TextureCoordinates.x < progress) //horizontal
    {
        float fadeProgress = input.TextureCoordinates.x / progress;
        strength *= pow(fadeProgress, 1.5f);
        textureColor = FadeOutColor(textureColor, samplerStrength.r, pow(1 - fadeProgress, 0.5f));
    }
    
    //fade out at the other horizontal edge
    if (input.TextureCoordinates.x > (progress * FadeOutRangeX))
    {
        float fadeFactor = 1 - ((input.TextureCoordinates.x - (progress * FadeOutRangeX)) / (progress * (1 - FadeOutRangeX)));
        strength *= pow(fadeFactor, 0.75f);
        textureColor = FadeOutColor(textureColor, samplerStrength, 1 - fadeFactor);
    }
    
    //vertical
    float yAbsDist = 1 - (2 * abs(input.TextureCoordinates.y - 0.5f));
    float fadeProgressY = 1 - pow(yAbsDist - 1, 2);
    strength *= pow(fadeProgressY, 1.75f);
    strength *= pow(input.TextureCoordinates.y, 2);

    textureColor = FadeOutColor(textureColor, samplerStrength.r, pow(1 - fadeProgressY, 0.75f));
    textureColor *= pow(opacity, lerp(2, 0.33f, strength));

    float4 finalColor = color * textureColor * strength;
    return finalColor * intensity;
}

technique BasicColorDrawing
{
    pass DefaultPass
	{
        VertexShader = compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 MainPS();
    }
};