#pragma once
#include <d3d11.h>
#include <directxmath.h>
#include <vector>
#include "Shaders.h"

using namespace DirectX;

class Graphics {
public:
    Graphics();
    ~Graphics();

    bool Initialize(HWND hWnd, int width, int height);
    void Render(const GameState& gameState);
    void Cleanup();

    // Camera control
    void SetCamera(XMVECTOR pos, XMVECTOR target, XMVECTOR up);
    void UpdateProjection(float fov, float aspect, float nearZ, float farZ);

private:
    // DirectX objects
    ID3D11Device* m_device;
    ID3D11DeviceContext* m_context;
    IDXGISwapChain* m_swapChain;
    ID3D11RenderTargetView* m_renderTargetView;
    ID3D11DepthStencilView* m_depthStencilView;
    ID3D11Buffer* m_vertexBuffer;
    ID3D11Buffer* m_indexBuffer;
    ID3D11Buffer* m_constantBuffer;

    // Shaders and related objects
    Shaders m_shaders;
    
    // Matrices
    XMMATRIX m_view;
    XMMATRIX m_projection;

    bool CreateDeviceAndSwapChain(HWND hWnd, int width, int height);
    bool CreateRenderTargetView();
    bool CreateDepthStencilView(int width, int height);
    bool CreateGeometryBuffers();
    void SetupViewport(int width, int height);
};