#include "ParticleSystem.h"
#include <random>

ParticleSystem::ParticleSystem() : m_vertexBuffer(nullptr), m_instanceBuffer(nullptr),
    m_vertexShader(nullptr), m_pixelShader(nullptr), m_geometryShader(nullptr),
    m_inputLayout(nullptr), m_blendState(nullptr), m_depthState(nullptr),
    m_random(std::random_device{}()) {}

ParticleSystem::~ParticleSystem() {
    if (m_vertexBuffer) m_vertexBuffer->Release();
    if (m_instanceBuffer) m_instanceBuffer->Release();
    if (m_vertexShader) m_vertexShader->Release();
    if (m_pixelShader) m_pixelShader->Release();
    if (m_geometryShader) m_geometryShader->Release();
    if (m_inputLayout) m_inputLayout->Release();
    if (m_blendState) m_blendState->Release();
    if (m_depthState) m_depthState->Release();
}

bool ParticleSystem::Initialize(ID3D11Device* device) {
    // Create blend state for alpha blending
    D3D11_BLEND_DESC blendDesc = {};
    blendDesc.AlphaToCoverageEnable = false;
    blendDesc.IndependentBlendEnable = false;
    blendDesc.RenderTarget[0].BlendEnable = true;
    blendDesc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
    blendDesc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
    blendDesc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
    blendDesc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ONE;
    blendDesc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ZERO;
    blendDesc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
    blendDesc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;

    HRESULT hr = device->CreateBlendState(&blendDesc, &m_blendState);
    if (FAILED(hr)) return false;

    // Create depth stencil state
    D3D11_DEPTH_STENCIL_DESC depthDesc = {};
    depthDesc.DepthEnable = true;
    depthDesc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ZERO; // Don't write to depth buffer
    depthDesc.DepthFunc = D3D11_COMPARISON_LESS;

    hr = device->CreateDepthStencilState(&depthDesc, &m_depthState);
    if (FAILED(hr)) return false;

    // Initialize particle buffer
    m_particles.reserve(MAX_PARTICLES);
    
    return true;
}

void ParticleSystem::Update(float deltaTime) {
    // Update existing particles
    for (auto it = m_particles.begin(); it != m_particles.end();) {
        it->life -= deltaTime;
        if (it->life <= 0.0f) {
            it = m_particles.erase(it);
        } else {
            // Update position based on velocity
            it->position.x += it->velocity.x * deltaTime;
            it->position.y += it->velocity.y * deltaTime;
            it->position.z += it->velocity.z * deltaTime;
            
            // Add gravity effect
            it->velocity.y -= 9.8f * deltaTime;
            
            // Update color alpha based on life
            it->color.w = it->life / PARTICLE_LIFETIME;
            
            ++it;
        }
    }
}

XMFLOAT3 ParticleSystem::RandomVelocity(float speed) {
    std::uniform_real_distribution<float> angleDist(0.0f, XM_2PI);
    std::uniform_real_distribution<float> elevationDist(-XM_PIDIV2, XM_PIDIV2);
    
    float angle = angleDist(m_random);
    float elevation = elevationDist(m_random);
    
    return XMFLOAT3(
        speed * cos(elevation) * cos(angle),
        speed * sin(elevation),
        speed * cos(elevation) * sin(angle)
    );
}

void ParticleSystem::EmitLineClear(int y) {
    std::uniform_real_distribution<float> colorDist(0.5f, 1.0f);
    std::uniform_real_distribution<float> speedDist(2.0f, 5.0f);
    
    for (int x = 0; x < GameState::GRID_WIDTH; x++) {
        for (int z = 0; z < GameState::GRID_DEPTH; z++) {
            if (m_particles.size() >= MAX_PARTICLES) return;
            
            Particle p;
            p.position = XMFLOAT3(
                x - GameState::GRID_WIDTH/2.0f,
                y,
                z - GameState::GRID_DEPTH/2.0f
            );
            p.velocity = RandomVelocity(speedDist(m_random));
            p.color = XMFLOAT4(
                colorDist(m_random),
                colorDist(m_random),
                colorDist(m_random),
                1.0f
            );
            p.life = PARTICLE_LIFETIME;
            p.size = 0.1f;
            
            m_particles.push_back(p);
        }
    }
}

void ParticleSystem::EmitPieceLock(const XMFLOAT3& position) {
    std::uniform_real_distribution<float> speedDist(1.0f, 3.0f);
    std::uniform_real_distribution<float> colorDist(0.7f, 1.0f);
    
    for (int i = 0; i < 20; i++) {
        if (m_particles.size() >= MAX_PARTICLES) return;
        
        Particle p;
        p.position = position;
        p.velocity = RandomVelocity(speedDist(m_random));
        p.color = XMFLOAT4(
            colorDist(m_random),
            colorDist(m_random),
            1.0f,  // More blue for lock effect
            1.0f
        );
        p.life = PARTICLE_LIFETIME * 0.5f;
        p.size = 0.05f;
        
        m_particles.push_back(p);
    }
}

void ParticleSystem::EmitGameOver() {
    std::uniform_real_distribution<float> speedDist(3.0f, 8.0f);
    std::uniform_real_distribution<float> colorDist(0.8f, 1.0f);
    
    for (int i = 0; i < 200; i++) {
        if (m_particles.size() >= MAX_PARTICLES) return;
        
        Particle p;
        p.position = XMFLOAT3(0.0f, GameState::GRID_HEIGHT/2.0f, 0.0f);
        p.velocity = RandomVelocity(speedDist(m_random));
        p.color = XMFLOAT4(
            1.0f,  // Red for game over
            colorDist(m_random) * 0.3f,
            colorDist(m_random) * 0.3f,
            1.0f
        );
        p.life = PARTICLE_LIFETIME * 2.0f;
        p.size = 0.15f;
        
        m_particles.push_back(p);
    }
}