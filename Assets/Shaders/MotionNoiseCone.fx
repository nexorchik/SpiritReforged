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
float progress;
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

const float yFadeDist = 0.9f;
const float xFadeDist = 0.2f;
const float colorLerpThreshold = 0.5f;
float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float yCoord = input.TextureCoordinates.y;
    float4 finalColor = uColor;
    float taperMod = max(lerp(1, input.TextureCoordinates.x, Tapering), 0.01f);
    
    //Center the y coordinate around 0.5, then divide it based on the x coordinate to "compress" it
    yCoord -= 0.5f;
    yCoord /= taperMod;
    yCoord += 0.5f;
    
    float2 textureCoords = float2((input.TextureCoordinates.x * xMod) + progress, yCoord * yMod);
    
    float strength = tex2D(textureSampler, textureCoords).r; //sample texture for base value
    float absYDist = 1 - (abs(yCoord - 0.5f) * 2);
    float absXDist = 1 - (abs(input.TextureCoordinates.x - 0.5f) * 2);
    
    float sampleTexExponent = 20 * pow(max(input.TextureCoordinates.x, 0.01f), 2); //start at a high number, reduce exponent based on x coordinate to make the dots appear bigger
    float baseStrength = lerp(0.9f, 0, pow(input.TextureCoordinates.x, 0.25f)); //base strength added to the noise sample, reduced based on x coordinate
    strength = min((pow(strength, sampleTexExponent) * 3) + baseStrength, 1);
    
    if (absYDist < 0) //Return 0 no matter what if absolute y distance is too far- prevents the colors from breaking if tapering is active
        return float4(0, 0, 0, 0);
        
    if (absYDist < yFadeDist) //slow fadeout based on absolute distance vertically
        strength *= pow(absYDist / yFadeDist, 0.5f);
    
    strength *= pow(1 - input.TextureCoordinates.x, 0.33f); //fadeout based on x coordinate
    if (strength > colorLerpThreshold) //interpolate from the dark color to the light color based on strength
        finalColor = lerp(uColor, uColor2, pow((strength - colorLerpThreshold) / (1 - colorLerpThreshold), 2));
    
    if (absXDist < xFadeDist) //fadeout based on absolute distance horizontally, if a threshold is met
        strength *= pow(absXDist / xFadeDist, 2);
    
    return strength * finalColor * input.Color * 1.5f; //Multiplied by 1.5 as a bandaid to make the colors more vibrant
}

technique BasicColorDrawing
{
    pass PrimitiveTextureMap
	{
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
};