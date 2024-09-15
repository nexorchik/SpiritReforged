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
float4 uColor;
float4 uColor2;
float Tapering;
float TaperExponent;
float scroll;
float dissipation;
float xMod;
float yMod;
float texExponentLerp;
float colorLerpExponent;
float finalIntensityMod;
int numColors;


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

const float xFadeDist = 0.2f;
const float colorLerpThreshold = 0.5f;
float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float yCoord = input.TextureCoordinates.y;
    float4 finalColor = uColor;
    float taperMod = max(lerp(1, pow(input.TextureCoordinates.x, TaperExponent), Tapering), 0.01f);
    
    //Center the y coordinate around 0.5, then divide it based on the x coordinate to "compress" it
    yCoord -= 0.5f;
    yCoord /= taperMod;
    yCoord += 0.5f;
    
    float2 textureCoords = float2((input.TextureCoordinates.x + scroll) * xMod, yCoord * yMod);
    
    float strength = tex2D(textureSampler, textureCoords).r; //sample texture for base value
    float absYDist = 1 - (abs(yCoord - 0.5f) * 2);
    
    float sampleTexExponent = lerp(0.01f, 30, pow(input.TextureCoordinates.x, texExponentLerp)); //start at a low number, increase exponent based on x coordinate to make the dots appear smaller
    strength = pow(strength, sampleTexExponent);
    
    if (absYDist < 0) //Return 0 no matter what if absolute y distance is too far- prevents the colors from breaking if tapering is active
        return float4(0, 0, 0, 0);
    
    strength *= pow(1 - cos(1.57f * absYDist), 0.33f); //slow fadeout based on absolute distance vertically
    strength *= pow(absYDist, 0.33f);
    
    strength *= pow(cos(1.57f * (input.TextureCoordinates.x - 0.15f)), 2); //fadeout based on x coordinate
    if (input.TextureCoordinates.x < dissipation)
    {
        float dissipationFactor = pow((dissipation - input.TextureCoordinates.x) / dissipation, 1.5f);
        strength = pow(max(strength - dissipationFactor, 0), 1 + dissipationFactor);
    }
    
    strength *= pow((1 - dissipation), 0.5f);
    
    strength = round(strength * numColors) / numColors;
    
    finalColor = lerp(uColor, uColor2, pow(strength, colorLerpExponent)); //interpolate from the dark color to the light color based on strength
    
    finalColor *= strength;
    return finalColor * input.Color * finalIntensityMod;
}

technique BasicColorDrawing
{
    pass PrimitiveTextureMap
	{
        VertexShader = compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 MainPS();
    }
};