

sampler LightmaskSampler : register(s0);
sampler SceneSampler : register(s1);

float minIntensity;
float maxIntensity;
float lerpValue;

float minFlicker;
float maxFlicker;



float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // TODO: add your pixel shader code here.

    return float4(1, 0, 0, 1);
}




technique StaticLighting
{
    pass Pass1
    {
        // TODO: set renderstates here.
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
