float4x4 World;
float4x4 View;
float4x4 Projection;
int Time;



struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 ObjectPosition : TEXCOORD1;
};

struct PixelToFrame
{
    float4 Color : COLOR0;
};


VertexShaderOutput VertexShaderFunction(float4 Position : POSITION0)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(Position, World);
    float4 viewPosition = mul(worldPosition, View);

    output.Position = mul(viewPosition, Projection);
    output.ObjectPosition = Position;
    
    return output;
}

PixelToFrame PixelShaderFunction(VertexShaderOutput input)
{
    PixelToFrame output = (PixelToFrame)0;

    float4 sun = float4(1, 1, 1, saturate(cos((float)Time/1000)));
    float4 ground = float4(cos((float)Time/6001), 0.6, 0.5, 1);
    float4 sky = lerp(float4(0.1, 0.55, 0.9, saturate(sin((float)Time/1000)+0.5)), sun, saturate(input.ObjectPosition.x));
    output.Color = lerp(ground, sky, saturate(input.ObjectPosition.y/0.5));

    return output;
}

technique Simple
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
