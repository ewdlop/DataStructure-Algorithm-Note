#include <windows.h>
#include <d3d11.h>
#include <d3dcompiler.h>
#include <directxmath.h>
#include <vector>

using namespace DirectX;

// Window dimensions
const int WINDOW_WIDTH = 800;
const int WINDOW_HEIGHT = 600;

// DirectX pointers
ID3D11Device* g_pd3dDevice = nullptr;
ID3D11DeviceContext* g_pImmediateContext = nullptr;
IDXGISwapChain* g_pSwapChain = nullptr;
ID3D11RenderTargetView* g_pRenderTargetView = nullptr;
ID3D11VertexShader* g_pVertexShader = nullptr;
ID3D11PixelShader* g_pPixelShader = nullptr;
ID3D11InputLayout* g_pVertexLayout = nullptr;
ID3D11Buffer* g_pVertexBuffer = nullptr;
ID3D11Buffer* g_pIndexBuffer = nullptr;
ID3D11Buffer* g_pConstantBuffer = nullptr;

// Vertex structure
struct Vertex {
    XMFLOAT3 Pos;
    XMFLOAT4 Color;
};

// Constant buffer structure
struct ConstantBuffer {
    XMMATRIX mWorld;
    XMMATRIX mView;
    XMMATRIX mProjection;
};

// Game grid dimensions
const int GRID_WIDTH = 6;
const int GRID_HEIGHT = 12;
const int GRID_DEPTH = 6;

// Tetris piece structure
struct TetrisPiece {
    std::vector<XMFLOAT3> blocks;
    XMFLOAT3 position;
    XMFLOAT4 color;
};

// Game state
bool g_gameGrid[GRID_WIDTH][GRID_HEIGHT][GRID_DEPTH] = {false};
TetrisPiece g_currentPiece;
float g_dropTimer = 0.0f;
const float DROP_INTERVAL = 1.0f;

// Vertex shader
const char* vertexShaderSource = R"(
cbuffer ConstantBuffer : register(b0) {
    matrix World;
    matrix View;
    matrix Projection;
}

struct VS_INPUT {
    float3 Pos : POSITION;
    float4 Color : COLOR;
};

struct PS_INPUT {
    float4 Pos : SV_POSITION;
    float4 Color : COLOR;
};

PS_INPUT VS(VS_INPUT input) {
    PS_INPUT output;
    float4 pos = float4(input.Pos, 1.0f);
    pos = mul(pos, World);
    pos = mul(pos, View);
    pos = mul(pos, Projection);
    output.Pos = pos;
    output.Color = input.Color;
    return output;
}
)";

// Pixel shader
const char* pixelShaderSource = R"(
struct PS_INPUT {
    float4 Pos : SV_POSITION;
    float4 Color : COLOR;
};

float4 PS(PS_INPUT input) : SV_Target {
    return input.Color;
}
)";

// Window procedure
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
    switch (message) {
        case WM_KEYDOWN:
            switch (wParam) {
                case VK_LEFT:
                    // Move piece left
                    break;
                case VK_RIGHT:
                    // Move piece right
                    break;
                case VK_UP:
                    // Rotate piece
                    break;
                case VK_DOWN:
                    // Move piece down
                    break;
                case VK_SPACE:
                    // Drop piece
                    break;
            }
            break;
        case WM_DESTROY:
            PostQuitMessage(0);
            return 0;
    }
    return DefWindowProc(hWnd, message, wParam, lParam);
}

// Initialize DirectX
bool InitDirectX(HWND hWnd) {
    DXGI_SWAP_CHAIN_DESC sd = {};
    sd.BufferCount = 1;
    sd.BufferDesc.Width = WINDOW_WIDTH;
    sd.BufferDesc.Height = WINDOW_HEIGHT;
    sd.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
    sd.BufferDesc.RefreshRate.Numerator = 60;
    sd.BufferDesc.RefreshRate.Denominator = 1;
    sd.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
    sd.OutputWindow = hWnd;
    sd.SampleDesc.Count = 1;
    sd.SampleDesc.Quality = 0;
    sd.Windowed = TRUE;

    D3D_FEATURE_LEVEL featureLevels[] = {D3D_FEATURE_LEVEL_11_0};
    D3D_FEATURE_LEVEL featureLevel;

    if (FAILED(D3D11CreateDeviceAndSwapChain(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, 0,
        featureLevels, 1, D3D11_SDK_VERSION, &sd, &g_pSwapChain, &g_pd3dDevice,
        &featureLevel, &g_pImmediateContext))) {
        return false;
    }

    // Create render target view
    ID3D11Texture2D* pBackBuffer = nullptr;
    if (FAILED(g_pSwapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), (LPVOID*)&pBackBuffer))) {
        return false;
    }

    if (FAILED(g_pd3dDevice->CreateRenderTargetView(pBackBuffer, nullptr, &g_pRenderTargetView))) {
        pBackBuffer->Release();
        return false;
    }
    pBackBuffer->Release();

    g_pImmediateContext->OMSetRenderTargets(1, &g_pRenderTargetView, nullptr);

    // Setup viewport
    D3D11_VIEWPORT vp;
    vp.Width = (FLOAT)WINDOW_WIDTH;
    vp.Height = (FLOAT)WINDOW_HEIGHT;
    vp.MinDepth = 0.0f;
    vp.MaxDepth = 1.0f;
    vp.TopLeftX = 0;
    vp.TopLeftY = 0;
    g_pImmediateContext->RSSetViewports(1, &vp);

    return true;
}

// Create shader resources
bool CreateShaders() {
    ID3DBlob* pVSBlob = nullptr;
    ID3DBlob* pErrorBlob = nullptr;

    // Compile vertex shader
    if (FAILED(D3DCompile(vertexShaderSource, strlen(vertexShaderSource), nullptr, nullptr, nullptr,
        "VS", "vs_4_0", 0, 0, &pVSBlob, &pErrorBlob))) {
        if (pErrorBlob) {
            OutputDebugStringA((char*)pErrorBlob->GetBufferPointer());
            pErrorBlob->Release();
        }
        return false;
    }

    // Create vertex shader
    if (FAILED(g_pd3dDevice->CreateVertexShader(pVSBlob->GetBufferPointer(),
        pVSBlob->GetBufferSize(), nullptr, &g_pVertexShader))) {
        pVSBlob->Release();
        return false;
    }

    // Create input layout
    D3D11_INPUT_ELEMENT_DESC layout[] = {
        { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        { "COLOR", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 }
    };

    if (FAILED(g_pd3dDevice->CreateInputLayout(layout, 2, pVSBlob->GetBufferPointer(),
        pVSBlob->GetBufferSize(), &g_pVertexLayout))) {
        pVSBlob->Release();
        return false;
    }
    pVSBlob->Release();

    // Compile and create pixel shader
    ID3DBlob* pPSBlob = nullptr;
    if (FAILED(D3DCompile(pixelShaderSource, strlen(pixelShaderSource), nullptr, nullptr, nullptr,
        "PS", "ps_4_0", 0, 0, &pPSBlob, &pErrorBlob))) {
        if (pErrorBlob) {
            OutputDebugStringA((char*)pErrorBlob->GetBufferPointer());
            pErrorBlob->Release();
        }
        return false;
    }

    if (FAILED(g_pd3dDevice->CreatePixelShader(pPSBlob->GetBufferPointer(),
        pPSBlob->GetBufferSize(), nullptr, &g_pPixelShader))) {
        pPSBlob->Release();
        return false;
    }
    pPSBlob->Release();

    return true;
}

// Create vertex and index buffers for a cube
bool CreateGeometryBuffers() {
    // Vertex data for a cube
    Vertex vertices[] = {
        { XMFLOAT3(-0.5f, -0.5f, -0.5f), XMFLOAT4(0.0f, 0.0f, 0.0f, 1.0f) },
        { XMFLOAT3(-0.5f, 0.5f, -0.5f), XMFLOAT4(0.0f, 1.0f, 0.0f, 1.0f) },
        { XMFLOAT3(0.5f, 0.5f, -0.5f), XMFLOAT4(1.0f, 1.0f, 0.0f, 1.0f) },
        { XMFLOAT3(0.5f, -0.5f, -0.5f), XMFLOAT4(1.0f, 0.0f, 0.0f, 1.0f) },
        { XMFLOAT3(-0.5f, -0.5f, 0.5f), XMFLOAT4(0.0f, 0.0f, 1.0f, 1.0f) },
        { XMFLOAT3(-0.5f, 0.5f, 0.5f), XMFLOAT4(0.0f, 1.0f, 1.0f, 1.0f) },
        { XMFLOAT3(0.5f, 0.5f, 0.5f), XMFLOAT4(1.0f, 1.0f, 1.0f, 1.0f) },
        { XMFLOAT3(0.5f, -0.5f, 0.5f), XMFLOAT4(1.0f, 0.0f, 1.0f, 1.0f) }
    };

    D3D11_BUFFER_DESC bd = {};
    bd.Usage = D3D11_USAGE_DEFAULT;
    bd.ByteWidth = sizeof(Vertex) * 8;
    bd.BindFlags = D3D11_BIND_VERTEX_BUFFER;

    D3D11_SUBRESOURCE_DATA InitData = {};
    InitData.pSysMem = vertices;

    if (FAILED(g_pd3dDevice->CreateBuffer(&bd, &InitData, &g_pVertexBuffer))) {
        return false;
    }

    // Index data
    WORD indices[] = {
        0,1,2, 0,2,3,  // Front
        4,6,5, 4,7,6,  // Back
        4,5,1, 4,1,0,  // Left
        3,2,6, 3,6,7,  // Right
        1,5,6, 1,6,2,  // Top
        4,0,3, 4,3,7   // Bottom
    };

    bd.Usage = D3D11_USAGE_DEFAULT;
    bd.ByteWidth = sizeof(WORD) * 36;
    bd.BindFlags = D3D11_BIND_INDEX_BUFFER;

    InitData.pSysMem = indices;

    if (FAILED(g_pd3dDevice->CreateBuffer(&bd, &InitData, &g_pIndexBuffer))) {
        return false;
    }

    // Create constant buffer
    bd.Usage = D3D11_USAGE_DEFAULT;
    bd.ByteWidth = sizeof(ConstantBuffer);
    bd.BindFlags = D3D11_BIND_CONSTANT_BUFFER;

    if (FAILED(g_pd3dDevice->CreateBuffer(&bd, nullptr, &g_pConstantBuffer))) {
        return false;
    }

    return true;
}

// Initialize game state
void InitGame() {
    // Create initial piece
    g_currentPiece.blocks = {
        XMFLOAT3(0, 0, 0),
        XMFLOAT3(1, 0, 0),
        XMFLOAT3(0, 1, 0),
        XMFLOAT3(1, 1, 0)
    };
    g_currentPiece.position = XMFLOAT3(GRID_WIDTH/2, GRID_HEIGHT-1, GRID_DEPTH/2);
    g_currentPiece.color = XMFLOAT4(1.0f, 0.0f, 0.0f, 1.0f);
}

// Update game state
void Update(float deltaTime) {
    g_dropTimer += deltaTime;
    if (g_dropTimer >= DROP_INTERVAL) {
        g_dropTimer = 0.0f;
        // Move piece down
        g_currentPiece.position.y -= 1.0f;
        // Check for collision
        // Lock piece if needed
        // Generate new piece if needed
    }
}

// // Render frame
// void Render() {
//     // Clear the back buffer
//     float ClearColor[4] = { 0.0f, 0.2f, 0.4f, 1.0f };
//     g_pImmediateContext->ClearRenderTargetView(g_pRenderTargetView, ClearColor);

//     // Setup matrices
//     XMMATRIX world = XMMatrixIdentity();
//     XMMATRIX view = XMMatrixLookAtLH(
//         XMVectorSet(0.0f, 5.0f, -15.0f, 0.0f),
//         XMVectorSet(0.0f, 0.0f, 0.0f, 0.0f),
//         XMVectorSet(0.0f, 1.0f, 0.0f, 0.0f)
//     );
//     XMMATRIX projection = XMMat