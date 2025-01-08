#pragma once
#include "GameState.h"
#include "AudioSystem.h"

class HoldPieceSystem {
public:
    HoldPieceSystem() : m_canHold(true) {}

    void Reset() {
        m_heldPiece = std::nullopt;
        m_canHold = true;
    }

    bool TryHoldPiece(GameState& gameState, AudioSystem& audio) {
        if (!m_canHold) return false;

        auto currentPiece = gameState.currentPiece;
        
        if (m_heldPiece) {
            // Swap current piece with held piece
            gameState.currentPiece = *m_heldPiece;
            gameState.currentPiece.position = XMFLOAT3(
                GameState::GRID_WIDTH/2 - 1,
                GameState::GRID_HEIGHT - 1,
                GameState::GRID_DEPTH/2 - 1
            );
        } else {
            // No held piece, spawn new piece
            SpawnNewPiece(gameState);
        }

        // Store current piece
        m_heldPiece = currentPiece;
        // Reset held piece position for display
        m_heldPiece->position = XMFLOAT3(-5.0f, GameState::GRID_HEIGHT - 2, 0.0f);

        m_canHold = false;
        audio.PlaySound(AudioSystem::HOLD);
        return true;
    }

    void OnPieceLocked() {
        m_canHold = true;
    }

    const std::optional<TetrisPiece>& GetHeldPiece() const {
        return m_heldPiece;
    }

private:
    std::optional<TetrisPiece> m_heldPiece;
    bool m_canHold;

    void SpawnNewPiece(GameState& gameState) {
        static const std::vector<std::vector<XMFLOAT3>> PIECES = {
            // I Piece
            {{0,0,0}, {1,0,0}, {2,0,0}, {3,0,0}},
            // L Piece
            {{0,0,0}, {1,0,0}, {2,0,0}, {2,1,0}},
            // J Piece
            {{0,0,0}, {1,0,0}, {2,0,0}, {0,1,0}},
            // O Piece
            {{0,0,0}, {1,0,0}, {0,1,0}, {1,1,0}},
            // S Piece
            {{1,0,0}, {2,0,0}, {0,1,0}, {1,1,0}},
            // T Piece
            {{1,0,0}, {0,1,0}, {1,1,0}, {2,1,0}},
            // Z Piece
            {{0,0,0}, {1,0,0}, {1,1,0}, {2,1,0}}
        };

        static const std::vector<XMFLOAT4> COLORS = {
            {1.0f, 0.0f, 0.0f, 1.0f}, // Red
            {0.0f, 1.0f, 0.0f, 1.0f}, // Green
            {0.0f, 0.0f, 1.0f, 1.0f}, // Blue
            {1.0f, 1.0f, 0.0f, 1.0f}, // Yellow
            {1.0f, 0.0f, 1.0f, 1.0f}, // Magenta
            {0.0f, 1.0f, 1.0f, 1.0f}, // Cyan
            {1.0f, 0.5f, 0.0f, 1.0f}  // Orange
        };

        std::random_device rd;
        std::mt19937 gen(rd());
        std::uniform_int_distribution<> dis(0, PIECES.size() - 1);

        int pieceIndex = dis(gen);
        gameState.currentPiece.blocks = PIECES[pieceIndex];
        gameState.currentPiece.color = COLORS[pieceIndex];
        gameState.currentPiece.position = XMFLOAT3(
            GameState::GRID_WIDTH/2 - 1,
            GameState::GRID_HEIGHT - 1,
            GameState::GRID_DEPTH/2 - 1
        );
        gameState.currentPiece.rotation = 0;
    }
};