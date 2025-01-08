#pragma once
#include <DirectXMath.h>
#include <array>
#include <optional>

using namespace DirectX;

class CameraSystem {
public:
    struct CameraConfig {
        static constexpr float DEFAULT_FOV = XM_PIDIV4;
        static constexpr float DEFAULT_NEAR_PLANE = 0.1f;
        static constexpr float DEFAULT_FAR_PLANE = 100.0f;
        static constexpr float MIN_PITCH = -XM_PIDIV2 * 0.9f;
        static constexpr float MAX_PITCH = XM_PIDIV2 * 0.9f;
        static constexpr float MIN_DISTANCE = 5.0f;
        static constexpr float MAX_DISTANCE = 30.0f;
        static constexpr float DEFAULT_ROTATION_SPEED = 0.005f;
        static constexpr float DEFAULT_ZOOM_SPEED = 0.1f;
    };

    struct CameraState {
        XMFLOAT3 position{0.0f, 5.0f, -15.0f};
        XMFLOAT3 target{0.0f, 0.0f, 0.0f};
        XMFLOAT3 up{0.0f, 1.0f, 0.0f};
        float yaw = 0.0f;
        float pitch = 0.0f;
        float distance = 15.0f;
    };

private:
    CameraState m_state;
    float m_rotationSpeed = CameraConfig::DEFAULT_ROTATION_SPEED;
    float m_zoomSpeed = CameraConfig::DEFAULT_ZOOM_SPEED;

    // Cached matrices
    std::optional<XMMATRIX> m_cachedView;
    std::optional<XMMATRIX> m_cachedProjection;
    float m_lastAspectRatio = 0.0f;

public:
    void Rotate(float dx, float dy) {
        m_state.yaw += dx * m_rotationSpeed;
        m_state.pitch += dy * m_rotationSpeed;
        
        m_state.pitch = std::clamp(m_state.pitch, 
                                  CameraConfig::MIN_PITCH, 
                                  CameraConfig::MAX_PITCH);
        
        UpdatePosition();
        InvalidateCache();
    }

    void Zoom(float delta) {
        m_state.distance = std::clamp(
            m_state.distance - delta * m_zoomSpeed,
            CameraConfig::MIN_DISTANCE,
            CameraConfig::MAX_DISTANCE
        );
        
        UpdatePosition();
        InvalidateCache();
    }

    void SetTarget(const XMFLOAT3& target) {
        m_state.target = target;
        UpdatePosition();
        InvalidateCache();
    }

    void ApplyScreenShake(const XMFLOAT3& shake) {
        m_state.position.x += shake.x;
        m_state.position.y += shake.y;
        m_state.position.z += shake.z;
        InvalidateCache();
    }

    [[nodiscard]]
    const XMMATRIX& GetViewMatrix() {
        if (!m_cachedView) {
            m_cachedView = XMMatrixLookAtLH(
                XMLoadFloat3(&m_state.position),
                XMLoadFloat3(&m_state.target),
                XMLoadFloat3(&m_state.up)
            );
        }
        return *m_cachedView;
    }

    [[nodiscard]]
    const XMMATRIX& GetProjectionMatrix(float aspectRatio) {
        if (!m_cachedProjection || m_lastAspectRatio != aspectRatio) {
            m_cachedProjection = XMMatrixPerspectiveFovLH(
                CameraConfig::DEFAULT_FOV,
                aspectRatio,
                CameraConfig::DEFAULT_NEAR_PLANE,
                CameraConfig::DEFAULT_FAR_PLANE
            );
            m_lastAspectRatio = aspectRatio;
        }
        return *m_cachedProjection;
    }

    [[nodiscard]]
    const CameraState& GetState() const { return m_state; }

private:
    void UpdatePosition() {
        // Calculate new camera position based on spherical coordinates
        float x = m_state.distance * cosf(m_state.pitch) * cosf(m_state.yaw);
        float y = m_state.distance * sinf(m_state.pitch);
        float z = m_state.distance * cosf(m_state.pitch) * sinf(m_state.yaw);

        m_state.position = {
            m_state.target.x + x,
            m_state.target.y + y,
            m_state.target.z + z
        };
    }

    void InvalidateCache() {
        m_cachedView.reset();
    }
};