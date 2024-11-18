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

texture overlayTexture;
sampler overlaySampler = sampler_state
{
    Texture = (overlayTexture);
    AddressU = wrap;
    AddressV = wrap;
};
float4 overlayColor;

float Progress;

float2 coordMods;
float2 overlayCoordMods;
float2 overlayScrollMod;
float2 overlayExponentRange;
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
    float2 overlayTexCoords = float2((input.TextureCoordinates.x - (timer * overlayScrollMod.x)) * overlayCoordMods.x, (input.TextureCoordinates.y - (timer * overlayScrollMod.y)) * overlayCoordMods.y);
    
    //add base texture color
    float samplerStrength = tex2D(baseSampler, baseTexCoords).r;
    textureColor = lerp(baseColorLight, baseColorDark, 1 - samplerStrength) * pow(samplerStrength, 0.75f);
    
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

    textureColor = FadeOutColor(textureColor, samplerStrength.r, pow(1 - fadeProgressY, 0.75f));
    textureColor *= pow(opacity, lerp(2, 0.33f, strength));
    
    //add overlay color
    float overlayStrength = pow(tex2D(overlaySampler, overlayTexCoords).r, lerp(overlayExponentRange.x, overlayExponentRange.y, strength));
    overlayStrength *= pow(opacity, 0.33f);
    textureColor = lerp(textureColor, overlayColor, overlayStrength);

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