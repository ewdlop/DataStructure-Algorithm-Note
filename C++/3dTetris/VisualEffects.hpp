#pragma once
#include <DirectXMath.h>
#include <vector>
#include <random>

using namespace DirectX;

struct Particle {
    XMFLOAT3 position;
    XMFLOAT3 velocity;
    XMFLOAT4 color;
    float life;
    float size;
    float rotation;
    float rotationSpeed;
};

class VisualEffects {
public:
    VisualEffects() : m_rng(std::random_device{}()) {}

    void Update(float deltaTime) {
        // Update screen shake
        if (m_screenShake > 0.0f) {
            m_screenShake -= deltaTime * 5.0f;
            if (m_screenShake < 0.0f) m_screenShake = 0.0f;

            std::uniform_real_distribution<float> shakeDist(-m_screenShake, m_screenShake);
            m_shakeOffset = XMFLOAT3(shakeDist(m_rng), shakeDist(m_rng), shakeDist(m_rng));
        }

        // Update particles
        for (auto it = m_particles.begin(); it != m_particles.end();) {
            it->life -= deltaTime;
            if (it->life <= 0.0f) {
                it = m_particles.erase(it);
                continue;
            }

            // Update position
            it->position.x += it->velocity.x * deltaTime;
            it->position.y += it->velocity.y * deltaTime;
            it->position.z += it->velocity.z * deltaTime;

            // Apply gravity
            it->velocity.y -= 9.8f * deltaTime;

            // Update rotation
            it->rotation += it->rotationSpeed * deltaTime;

            ++it;
        }
    }

    void EmitLineClear(int y) {
        m_screenShake = 0.3f;
        
        std::uniform_real_distribution<float> velDist(-5.0f, 5.0f);
        std::uniform_real_distribution<float> rotDist(-10.0f, 10.0f);
        
        for (int x = 0; x < GameState::GRID_WIDTH; x++) {
            for (int z = 0; z < GameState::GRID_DEPTH; z++) {
                EmitParticle(
                    XMFLOAT3(x - GameState::GRID_WIDTH/2.0f, y, z - GameState::GRID_DEPTH/2.0f),
                    XMFLOAT3(velDist(m_rng), 5.0f, velDist(m_rng)),
                    XMFLOAT4(1.0f, 1.0f, 0.3f, 1.0f),  // Yellow sparkle
                    1.5f,  // Life
                    0.1f,  // Size
                    rotDist(m_rng)  // Rotation speed
                );
            }
        }
    }

    void EmitPieceLock(const XMFLOAT3& position) {
        m_screenShake = 0.1f;
        
        std::uniform_real_distribution<float> velDist(-3.0f, 3.0f);
        std::uniform_real_distribution<float> rotDist(-5.0f, 5.0f);
        
        for (int i = 0; i < 20; i++) {
            EmitParticle(
                position,
                XMFLOAT3(velDist(m_rng), velDist(m_rng), velDist(m_rng)),
                XMFLOAT4(0.3f, 0.3f, 1.0f, 1.0f),  // Blue spark
                0.5f,  // Life
                0.05f,  // Size
                rotDist(m_rng)  // Rotation speed
            );
        }
    }

    void EmitGameOver() {
        m_screenShake = 0.5f;
        
        std::uniform_real_distribution<float> velDist(-8.0f, 8.0f);
        std::uniform_real_distribution<float> rotDist(-15.0f, 15.0f);
        
        for (int i = 0; i < 100; i++) {
            EmitParticle(
                XMFLOAT3(0.0f, GameState::GRID_HEIGHT/2.0f, 0.0f),
                XMFLOAT3(velDist(m_rng), velDist(m_rng), velDist(m_rng)),
                XMFLOAT4(1.0f, 0.2f, 0.2f, 1.0f),  // Red explosion
                2.0f,  // Life
                0.15f,  // Size
                rotDist(m_rng)  // Rotation speed
            );
        }
    }

    const std::vector<Particle>& GetParticles() const { return m_particles; }
    const XMFLOAT3& GetShakeOffset() const { return m_shakeOffset; }

private:
    std::vector<Particle> m_particles;
    std::mt19937 m_rng;
    float m_screenShake = 0.0f;
    XMFLOAT3 m_shakeOffset = {0, 0, 0};

    void EmitParticle(const XMFLOAT3& position, const XMFLOAT3& velocity, 
                     const XMFLOAT4& color, float life, float size, float rotSpeed) {
        if (m_particles.size() >= 1000) return;  // Particle limit
        
        Particle p;
        p.position = position;
        p.velocity = velocity;
        p.color = color;
        p.life = life;
        p.size = size;
        p.rotation = 0.0f;  // Start at 0 rotation
        p.rotationSpeed = rotSpeed;
        
        // Add fade out effect based on life
        std::uniform_real_distribution<float> fadeDist(0.8f, 1.0f);
        p.color.w = fadeDist(m_rng);
        
        m_particles.push_back(p);
    }

    // Helper function to blend colors
    XMFLOAT4 BlendColors(const XMFLOAT4& color1, const XMFLOAT4& color2, float blend) {
        return XMFLOAT4(
            color1.x * (1.0f - blend) + color2.x * blend,
            color1.y * (1.0f - blend) + color2.y * blend,
            color1.z * (1.0f - blend) + color2.z * blend,
            color1.w * (1.0f - blend) + color2.w * blend
        );
    }
};