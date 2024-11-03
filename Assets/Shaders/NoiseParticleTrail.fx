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
float alphaMod;
float intensity;

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

float4 FadeOutColor(float4 inputColor, float fadeProgress)
{
    return float4(inputColor.rgb * lerp(1, pow(inputColor.a, 0.75f), 1 - fadeProgress), inputColor.a); //Raise base color to a power based on fade progress, to make the less opaque parts fade out first
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
    textureColor = baseColorLight * samplerStrength;
    
    //fade out based on position
    if (input.TextureCoordinates.x < progress) //horizontal
    {
        float fadeProgress = input.TextureCoordinates.x / progress;
        strength *= pow(fadeProgress, 1.5f);
        
        textureColor = FadeOutColor(textureColor, fadeProgress);
        textureColor = lerp(baseColorLight, baseColorDark, fadeProgress * progress) * samplerStrength;
    }
    
    //fade out at the other horizontal edge
    if (input.TextureCoordinates.x > (progress * FadeOutRangeX))
    {
        strength *= 1 - ((input.TextureCoordinates.x - (progress * FadeOutRangeX)) / (progress * (1 - FadeOutRangeX)));
    }
    
    //vertical
    float yAbsDist = 1 - (2 * abs(input.TextureCoordinates.y - 0.5f));
    float fadeProgressY = pow(yAbsDist / FadeOutRangeY, 0.66f);
    strength *= pow(fadeProgressY, 1.5f);

    textureColor = FadeOutColor(textureColor, fadeProgressY);
    
    //add overlay color
    textureColor = lerp(textureColor, overlayColor, pow(tex2D(overlaySampler, overlayTexCoords).r, lerp(overlayExponentRange.x, overlayExponentRange.y, strength))); //raised to absurdly high power to create smaller stars, without making them too close to each other

    float4 finalColor = color * textureColor * strength;
    finalColor.a *= alphaMod;
    return finalColor * intensity; //final band-aid fix to make colors more intense
}

technique BasicColorDrawing
{
    pass DefaultPass
	{
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
};