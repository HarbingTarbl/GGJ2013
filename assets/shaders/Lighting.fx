

sampler LightmaskSampler : register(0);
sampler SceneSampler : register(1);

float minIntensity;
float maxIntensity;
float lerpValue;

float minFlicker;
float maxFlicker;
float lerpValue;



float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
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
