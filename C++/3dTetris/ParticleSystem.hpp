#pragma once
#include <d3d11.h>
#include <directxmath.h>
#include <vector>
#include <random>

using namespace DirectX;

struct Particle {
    XMFLOAT3 position;
    XMFLOAT3 velocity;
    XMFLOAT4 color;
    float life;
    float size;
};

class ParticleSystem {
public:
    ParticleSystem();
    ~ParticleSystem();

    bool Initialize(ID3D11Device* device);
    void Update(float deltaTime);
    void Render(ID3D11DeviceContext* context, const XMMATRIX& view, const XMMATRIX& projection);
    
    // Particle emission
    void EmitLineClear(int y);
    void EmitPieceLock(const XMFLOAT3& position);
    void EmitGameOver();

private:
    // DirectX resources
    ID3D11Buffer* m_vertexBuffer;
    ID3D11Buffer* m_instanceBuffer;
    ID3D11VertexShader* m_vertexShader;
    ID3D11PixelShader* m_pixelShader;
    ID3D11GeometryShader* m_geometryShader;
    ID3D11InputLayout* m_inputLayout;
    ID3D11BlendState* m_blendState;
    ID3D11DepthStencilState* m_depthState;
    
    // Particle storage
    std::vector<Particle> m_particles;
    std::mt19937 m_random;
    
    // Particle system parameters
    const int MAX_PARTICLES = 10000;
    const float PARTICLE_LIFETIME = 2.0f;
    
    void UpdateParticleBuffer(ID3D11DeviceContext* context);
    XMFLOAT3 RandomVelocity(float speed);
};