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

    float4 light = float4(0.03, 0.33, 0.68, 1);
    float4 dark = float4(0.02, 0.06, 0.34, 1);

    // Make pixels darker as we go higher from the horizon
    output.Color = lerp(light, dark, input.ObjectPosition.y);


    return output;
}

PixelToFrame ColoredGlobePixelShader(VertexShaderOutput input)
{
    PixelToFrame output = (PixelToFrame)0;

    // Behind the camera
    if (input.ObjectPosition.z > 0) output.Color = float4(0, 1, 0, 1);
    // Right side of the camera view
    else if (input.ObjectPosition.x > 0) output.Color = float4(0, 0, 1, 1);
    // Above the horizon
    else if (input.ObjectPosition.y > 0) output.Color = float4(1, 0, 0, 1);
    // Below the horizon
    else  output.Color = float4(0.7, 0, 0.2, 1);

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

technique Debug
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 ColoredGlobePixelShader();
    }
}
