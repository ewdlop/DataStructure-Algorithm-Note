// Constant buffers
cbuffer PerFrame : register(b0) {
    matrix View;
    matrix Projection;
    float3 CameraPosition;
    float Time;
}

cbuffer PerObject : register(b1) {
    matrix World;
    float4 Color;
    float4 Parameters; // x: shininess, y: specular power, z: rim power, w: unused
}

// Input/Output structures
struct VS_INPUT {
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float2 TexCoord : TEXCOORD0;
};

struct VS_OUTPUT {
    float4 Position : SV_POSITION;
    float3 WorldPos : POSITION;
    float3 Normal : NORMAL;
    float2 TexCoord : TEXCOORD0;
    float4 Color : COLOR0;
};

// Vertex Shader
VS_OUTPUT VS_Main(VS_INPUT input) {
    VS_OUTPUT output;
    
    // Transform position
    float4 worldPos = mul(float4(input.Position, 1.0f), World);
    output.WorldPos = worldPos.xyz;
    output.Position = mul(worldPos, mul(View, Projection));
    
    // Transform normal
    output.Normal = normalize(mul(input.Normal, (float3x3)World));
    
    // Pass through texcoord and calculate color
    output.TexCoord = input.TexCoord;
    output.Color = Color;
    
    return output;
}

// Pixel Shader
float4 PS_Main(VS_OUTPUT input) : SV_Target {
    float3 normal = normalize(input.Normal);
    float3 viewDir = normalize(CameraPosition - input.WorldPos);
    
    // Ambient light
    float3 ambient = float3(0.2f, 0.2f, 0.2f);
    
    // Diffuse light
    float3 lightDir = normalize(float3(1.0f, 1.0f, -1.0f));
    float diffuseStrength = max(dot(normal, lightDir), 0.0f);
    float3 diffuse = diffuseStrength * float3(0.8f, 0.8f, 0.8f);
    
    // Specular light
    float3 reflectDir = reflect(-lightDir, normal);
    float specularStrength = pow(max(dot(viewDir, reflectDir), 0.0f), Parameters.y);
    float3 specular = Parameters.x * specularStrength * float3(1.0f, 1.0f, 1.0f);
    
    // Rim light
    float rimStrength = 1.0f - max(dot(viewDir, normal), 0.0f);
    rimStrength = pow(rimStrength, Parameters.z);
    float3 rim = rimStrength * float3(0.3f, 0.3f, 0.3f);
    
    // Final color
    float3 finalColor = (ambient + diffuse + specular + rim) * input.Color.rgb;
    return float4(finalColor, input.Color.a);
}

// Particle vertex shader
struct ParticleVS_Input {
    float3 Position : POSITION;
    float3 Velocity : NORMAL;
    float4 Color : COLOR0;
    float2 Life : TEXCOORD0; // x: current life, y: max life
};

struct ParticleVS_Output {
    float3 Position : POSITION;
    float4 Color : COLOR0;
    float Size : TEXCOORD0;
};

ParticleVS_Output VS_Particle(ParticleVS_Input input) {
    ParticleVS_Output output;
    output.Position = input.Position;
    output.Color = input.Color;
    
    // Calculate particle size based on life
    float lifeRatio = input.Life.x / input.Life.y;
    output.Size = lerp(0.0f, 0.2f, lifeRatio);
    
    return output;
}

// Particle geometry shader
[maxvertexcount(4)]
void GS_Particle(point ParticleVS_Output input[1], inout TriangleStream<VS_OUTPUT>