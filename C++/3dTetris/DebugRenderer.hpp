#pragma once
#include <DirectXMath.h>
#include <vector>
#include <memory>
#include <array>
#include <string>

class DebugRenderer {
public:
    struct DebugVertex {
        XMFLOAT3 position;
        XMFLOAT4 color;
    };

    struct DebugLine {
        XMFLOAT3 start;
        XMFLOAT3 end;
        XMFLOAT4 color;
        float duration;
        bool depthTested;
    };

    struct DebugText {
        std::string text;
        XMFLOAT2 position;
        XMFLOAT4 color;
        float scale;
        float duration;
    };

    struct DebugConfig {
        static constexpr size_t MAX_LINES = 10000;
        static constexpr size_t MAX_TEXT = 1000;
        static constexpr float DEFAULT_DURATION = 0.0f;  // 0 = single frame
        static constexpr float DEFAULT_TEXT_SCALE = 1.0f;
    };

    DebugRenderer(ID3D11Device* device) : m_device(device) {
        CreateResources();
    }

    // Line drawing functions
    void DrawLine(const XMFLOAT3& start, const XMFLOAT3& end, 
                 const XMFLOAT4& color = {1,1,1,1},
                 float duration = DebugConfig::DEFAULT_DURATION,
                 bool depthTested = true) {
        m_lines.push_back({start, end, color, duration, depthTested});
    }

    void DrawBox(const XMFLOAT3& min, const XMFLOAT3& max,
                const XMFLOAT4& color = {1,1,1,1},
                float duration = DebugConfig::DEFAULT_DURATION) {
        // Draw 12 lines forming a box
        std::array<XMFLOAT3, 8> corners = {{
            {min.x, min.y, min.z}, {max.x, min.y, min.z},
            {min.x, max.y, min.z}, {max.x, max.y, min.z},
            {min.x, min.y, max.z}, {max.x, min.y, max.z},
            {min.x, max.y, max.z}, {max.x, max.y, max.z}
        }};

        static const std::array<std::pair<int, int>, 12> edges = {{
            {0,1}, {1,3}, {3,2}, {2,0},
            {4,5}, {5,7}, {7,6}, {6,4},
            {0,4}, {1,5}, {2,6}, {3,7}
        }};

        for (const auto& [start, end] : edges) {
            DrawLine(corners[start], corners[end], color, duration);
        }
    }

    void DrawSphere(const XMFLOAT3& center, float radius,
                   const XMFLOAT4& color = {1,1,1,1},
                   float duration = DebugConfig::DEFAULT_DURATION) {
        static constexpr int SEGMENTS = 16;
        
        for (int i = 0; i < SEGMENTS; ++i) {
            float angle1 = (float)i / SEGMENTS * XM_2PI;
            float angle2 = (float)(i + 1) / SEGMENTS * XM_2PI;

            // XY circle
            DrawLine(
                {center.x + radius * cosf(angle1), center.y + radius * sinf(angle1), center.z},
                {center.x + radius * cosf(angle2), center.y + radius * sinf(angle2), center.z},
                color, duration
            );

            // XZ circle
            DrawLine(
                {center.x + radius * cosf(angle1), center.y, center.z + radius * sinf(angle1)},
                {center.x + radius * cosf(angle2), center.y, center.z + radius * sinf(angle2)},
                color, duration
            );

            // YZ circle
            DrawLine(
                {center.x, center.y + radius * cosf(angle1), center.z + radius * sinf(angle1)},
                {center.x, center.y + radius * cosf(angle2), center.z + radius * sinf(angle2)},
                color, duration
            );
        }
    }

    // Text drawing functions
    void DrawText(const std::string& text, const XMFLOAT2& position,
                 const XMFLOAT4& color = {1,1,1,1},
                 float scale = DebugConfig::DEFAULT_TEXT_SCALE,
                 float duration = DebugConfig::DEFAULT_DURATION) {
        m_texts.push_back({text, position, color, scale, duration});
    }

    void DrawText3D(const std::string& text, const XMFLOAT3& position,
                   const XMFLOAT4& color = {1,1,1,1},
                   float scale = DebugConfig::DEFAULT_TEXT_SCALE,
                   float duration = DebugConfig::DEFAULT_DURATION) {
        // Project 3D position to screen space
        XMVECTOR pos = XMLoadFloat3(&position);
        pos = XMVector3Transform(pos, m_viewProjection);
        
        XMFLOAT4 projected;
        XMStoreFloat4(&projected, pos);

        if (projected.w > 0) {
            XMFLOAT2 screenPos = {
                (projected.x / projected.w + 1.0f) * m_viewport.Width * 0.5f,
                (-projected.y / projected.w + 1.0f) * m_viewport.Height * 0.5f
            };
            DrawText(text, screenPos, color, scale, duration);
        }
    }

    // Update and render
    void Update(float deltaTime) {
        // Update lifetimes and remove expired items
        for (auto it = m_lines.begin(); it != m_lines.end();) {
            if (it->duration > 0) {
                it->duration -= deltaTime;
                if (it->duration <= 0) {
                    it = m_lines.erase(it);
                    continue;
                }
            }
            ++it;
        }

        for (auto it = m_texts.begin(); it != m_texts.end();) {
            if (it->duration > 0) {
                it->duration -= deltaTime;
                if (it->duration <= 0) {
                    it = m_texts.erase(it);
                    continue;
                }
            }
            ++it;
        }
    }

    void Render(ID3D11DeviceContext* context,
               const XMMATRIX& view,
               const XMMATRIX& projection,
               const D3D11_VIEWPORT& viewport) {
        m_viewProjection = XMMatrixMultiply(view, projection);
        m_viewport = viewport;

        // Render lines
        if (!m_lines.empty()) {
            RenderLines(context, true);  // Depth tested lines
            RenderLines(context, false); // Overlay lines
        }

        // Render text
        if (!m_texts.empty()) {
            RenderText(context);
        }
    }

private:
    ID3D11Device* m_device;
    ComPtr<ID3D11Buffer> m_lineVB;
    ComPtr<ID3D11VertexShader> m_lineVS;
    ComPtr<ID3D11PixelShader> m_linePS;
    ComPtr<ID3D11InputLayout> m_lineLayout;

    ComPtr<ID2D1RenderTarget> m_textRT;
    ComPtr<IDWriteTextFormat> m_textFormat;
    ComPtr<ID2D1SolidColorBrush> m_textBrush;

    std::vector<DebugLine> m_lines;
    std::vector<DebugText> m_texts;

    XMMATRIX m_viewProjection;
    D3D11_VIEWPORT m_viewport;

    void CreateResources() {
        // Create line rendering resources
        CreateLineResources();
        
        // Create text rendering resources
        CreateTextResources();
    }

    void CreateLineResources() {
        // Implementation for creating line shaders and buffers
    }

    void CreateTextResources() {
        // Implementation for creating Direct2D/DirectWrite resources
    }

    void RenderLines(ID3D11DeviceContext* context, bool depthTested) {
        // Implementation for line rendering
    }

    void RenderText(ID3D11DeviceContext* context) {
        // Implementation for text rendering
    }
};