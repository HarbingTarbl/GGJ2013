
sampler sceneImage : register(s0);


float2 origin;
float power;
float angle;

struct VertexToPixel
{
	float4 Position : POSITION;
	float4 Color : COLOR0;
};

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{



    // TODO: add your pixel shader code here.

    return float4(1, 1, 1, 0);
}


technique StaticLighting
{
    pass Pass1
    {
        // TODO: set renderstates here.
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
