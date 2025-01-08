#pragma once
#include "GameTimer.h"
#include "AudioSystem.h"
#include "InputSystem.h"
#include "VisualEffects.h"
#include "CameraSystem.h"
#include "HoldPieceSystem.h"
#include "PieceMechanics.h"
#include <memory>

#pragma once
#include "Graphics.h"
#include "Audio.h"
#include "UI.h"
#include "ParticleSystem.h"
#include <memory>

// class Game {
// public:
//     Game();
//     ~Game();

//     bool Initialize(HINSTANCE hInstance, int nCmdShow);
//     void Update();
//     void Render();
//     void ProcessInput(UINT message, WPARAM wParam, LPARAM lParam);

// private:
//     std::unique_ptr<Graphics> m_graphics;
//     std::unique_ptr<Audio> m_audio;
//     std::unique_ptr<UI> m_ui;
//     std::unique_ptr<ParticleSystem> m_particles;
    
//     GameState m_gameState;
//     Timer m_timer;
    
//     void SpawnNewPiece();
//     void LockPiece();
//     void CheckLines();
//     void ResetGame();
//     bool CheckCollision(float x, float y, float z);
// };

class Game {
public:
    Game() : m_isPaused(false) {}
    ~Game() = default;

    bool Initialize(HINSTANCE hInstance, int nCmdShow) {
        // Create window
        if (!InitializeWindow(hInstance, nCmdShow))
            return false;

        // Initialize systems
        m_timer = std::make_unique<GameTimer>();
        m_audio = std::make_unique<AudioSystem>();
        m_input = std::make_unique<InputSystem>();
        m_visualEffects = std::make_unique<VisualEffects>();
        m_camera = std::make_unique<CameraSystem>();
        m_holdPiece = std::make_unique<HoldPieceSystem>();

        // Initialize graphics
        if (!InitializeDirectX())
            return false;

        // Initialize audio
        if (!m_audio->Initialize())
            return false;

        // Start background music
        m_audio->PlaySound(AudioSystem::BACKGROUND_MUSIC, true);

        // Initialize game state
        ResetGame();

        return true;
    }

    void Update() {
        m_timer->Tick();
        float deltaTime = m_timer->DeltaTime();

        if (!m_isPaused) {
            // Process input
            ProcessInput();

            // Update game state
            UpdateGame(deltaTime);

            // Update visual effects
            m_visualEffects->Update(deltaTime);

            // Update camera with screen shake
            m_camera->ApplyScreenShake(m_visualEffects->GetShakeOffset());
        }
    }

    void Render() {
        // Clear back buffer
        float clearColor[4] = { 0.0f, 0.2f, 0.4f, 1.0f };
        m_context->ClearRenderTargetView(m_renderTargetView.Get(), clearColor);
        m_context->ClearDepthStencilView(m_depthStencilView.Get(), D3D11_CLEAR_DEPTH | D3D11_CLEAR_STENCIL, 1.0f, 0);

        // Get view and projection matrices
        XMMATRIX view = m_camera->GetViewMatrix();
        XMMATRIX projection = m_camera->GetProjectionMatrix(GetAspectRatio());

        // Render game grid
        RenderGrid(view, projection);

        // Render current piece and ghost piece
        RenderPiece(m_gameState.currentPiece, view, projection);
        RenderGhostPiece(view, projection);

        // Render held piece if exists
        if (auto heldPiece = m_holdPiece->GetHeldPiece()) {
            RenderPiece(*heldPiece, view, projection);
        }

        // Render particles
        RenderParticles(view, projection);

        // Render UI
        RenderUI();

        // Present
        m_swapChain->Present(1, 0);
    }

    void ProcessInput() {
        m_input->Update(m_timer->DeltaTime());

        while (auto action = m_input->GetNextAction()) {
            switch (*action) {
                case InputSystem::Action::MOVE_LEFT:
                    MovePiece(-1, 0);
                    break;
                case InputSystem::Action::MOVE_RIGHT:
                    MovePiece(1, 0);
                    break;
                case InputSystem::Action::MOVE_FORWARD:
                    MovePiece(0, -1);
                    break;
                case InputSystem::Action::MOVE_BACKWARD:
                    MovePiece(0, 1);
                    break;
                case InputSystem::Action::ROTATE_X:
                    RotatePiece('x');
                    break;
                case InputSystem::Action::ROTATE_Y:
                    RotatePiece('y');
                    break;
                case InputSystem::Action::ROTATE_Z:
                    RotatePiece('z');
                    break;
                case InputSystem::Action::HARD_DROP:
                    InstantDrop();
                    break;
                case InputSystem::Action::HOLD_PIECE:
                    m_holdPiece->TryHoldPiece(m_gameState, *m_audio);
                    break;
                case InputSystem::Action::PAUSE:
                    TogglePause();
                    break;
            }
        }
    }

private:
    // Core systems
    std::unique_ptr<GameTimer> m_timer;
    std::unique_ptr<AudioSystem> m_audio;
    std::unique_ptr<InputSystem> m_input;
    std::unique_ptr<VisualEffects> m_visualEffects;
    std::unique_ptr<CameraSystem> m_camera;
    std::unique_ptr<HoldPieceSystem> m_holdPiece;

    // Game state
    GameState m_gameState;
    bool m_isPaused;

    // DirectX resources
    ComPtr<ID3D11Device> m_device;
    ComPtr<ID3D11DeviceContext> m_context;
    ComPtr<IDXGISwapChain> m_swapChain;
    ComPtr<ID3D11RenderTargetView> m_renderTargetView;
    ComPtr<ID3D11DepthStencilView> m_depthStencilView;

    void UpdateGame(float deltaTime) {
        if (m_gameState.isGameOver)
            return;

        // Update drop timer
        m_gameState.dropTimer += deltaTime;
        if (m_gameState.dropTimer >= m_gameState.dropInterval) {
            m_gameState.dropTimer = 0;
            if (!MovePieceDown()) {
                LockPiece();
            }
        }
    }

    bool MovePieceDown() {
        if (!CheckCollision(0, -1, 0)) {
            m_gameState.currentPiece.position.y -= 1;
            m_audio->PlaySound(AudioSystem::MOVE);
            return true;
        }
        return false;
    }

    void LockPiece() {
        // Add piece to grid
        for (const auto& block : m_gameState.currentPiece.blocks) {
            int x = m_gameState.currentPiece.position.x + block.x;
            int y = m_gameState.currentPiece.position.y + block.y;
            int z = m_gameState.currentPiece.position.z + block.z;
            m_gameState.grid[x][y][z] = true;
        }

        // Visual and audio feedback
        m_visualEffects->EmitPieceLock(m_gameState.currentPiece.position);
        m_audio->PlaySound(AudioSystem::LOCK);

        // Check for completed lines
        CheckLines();

        // Allow hold piece again
        m_holdPiece->OnPieceLocked();

        // Spawn new piece
        SpawnNewPiece();
    }

    void TogglePause() {
        m_isPaused = !m_isPaused;
        if (m_isPaused) {
            m_timer->Stop();
            m_audio->PauseAll();
        } else {
            m_timer->Start();
            m_audio->ResumeAll();
        }
    }

    float GetAspectRatio() const {
        RECT clientRect;
        GetClientRect(m_hwnd, &clientRect);
        return static_cast<float>(clientRect.right - clientRect.left) / 
               static_cast<float>(clientRect.bottom - clientRect.top);
    }

    // Additional implementation details...
};