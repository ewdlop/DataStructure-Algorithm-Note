#pragma once
#include "GameState.h"
#include <array>
#include <optional>

class PieceMechanics {
public:
    static constexpr std::array<XMFLOAT3, 5> WALL_KICK_TESTS = {{
        {0, 0, 0},   // Original position
        {1, 0, 0},   // Right
        {-1, 0, 0},  // Left
        {0, 0, 1},   // Forward
        {0, 0, -1}   // Backward
    }};

    // Rotation matrices as constexpr
    static constexpr std::array<std::array<int, 9>, 4> ROTATION_MATRICES = {{
        // 0 degrees (identity)
        {{ 1, 0, 0,
           0, 1, 0,
           0, 0, 1 }},
        // 90 degrees
        {{ 0, -1, 0,
           1,  0, 0,
           0,  0, 1 }},
        // 180 degrees
        {{-1,  0, 0,
           0, -1, 0,
           0,  0, 1 }},
        // 270 degrees
        {{ 0,  1, 0,
          -1,  0, 0,
           0,  0, 1 }}
    }};

    static constexpr XMFLOAT3 RotatePoint(const XMFLOAT3& point, const std::array<int, 9>& matrix) {
        return XMFLOAT3(
            point.x * matrix[0] + point.y * matrix[1] + point.z * matrix[2],
            point.x * matrix[3] + point.y * matrix[4] + point.z * matrix[5],
            point.x * matrix[6] + point.y * matrix[7] + point.z * matrix[8]
        );
    }

    static std::optional<GameState::PieceTemplate> TryRotation(
        const GameState::PieceTemplate& piece,
        int currentRotation,
        const GameState::GridType& grid,
        const XMFLOAT3& position) 
    {
        // Skip rotation for pieces with rotational symmetry of 1
        if (piece.rotationSymmetry == 1) return std::nullopt;

        int nextRotation = (currentRotation + 1) % piece.rotationSymmetry;
        auto& rotMatrix = ROTATION_MATRICES[nextRotation];

        GameState::PieceTemplate rotated = piece;
        for (auto& block : rotated.blocks) {
            block = RotatePoint(block, rotMatrix);
        }

        // Try wall kicks
        for (const auto& kick : WALL_KICK_TESTS) {
            XMFLOAT3 testPos = {
                position.x + kick.x,
                position.y + kick.y,
                position.z + kick.z
            };

            if (IsValidPosition(rotated, grid, testPos)) {
                return rotated;
            }
        }

        return std::nullopt;
    }

    static std::optional<XMFLOAT3> GetGhostPosition(
        const GameState::PieceTemplate& piece,
        const GameState::GridType& grid,
        const XMFLOAT3& position)
    {
        XMFLOAT3 ghostPos = position;
        while (IsValidPosition(piece, grid, {ghostPos.x, ghostPos.y - 1, ghostPos.z})) {
            ghostPos.y -= 1;
        }
        return ghostPos;
    }

private:
    static bool IsValidPosition(
        const GameState::PieceTemplate& piece,
        const GameState::GridType& grid,
        const XMFLOAT3& position)
    {
        for (const auto& block : piece.blocks) {
            int x = static_cast<int>(position.x + block.x);
            int y = static_cast<int>(position.y + block.y);
            int z = static_cast<int>(position.z + block.z);

            if (!GameState::IsValidPosition(x, y, z) || grid[x][y][z]) {
                return false;
            }
        }
        return true;
    }
};

#pragma once
#include <DirectXMath.h>
#include <vector>
#include <array>

using namespace DirectX;

struct TetrisPiece {
    std::vector<XMFLOAT3> blocks;
    XMFLOAT3 position;
    XMFLOAT4 color;
    int rotation = 0;  // 0-3 for each 90-degree rotation
    int type;
};

// class PieceMechanics {
// public:
//     static constexpr std::array<XMFLOAT3, 5> WALL_KICK_TESTS = {
//         XMFLOAT3(0, 0, 0),   // Original position
//         XMFLOAT3(1, 0, 0),   // Right
//         XMFLOAT3(-1, 0, 0),  // Left
//         XMFLOAT3(0, 0, 1),   // Forward
//         XMFLOAT3(0, 0, -1)   // Backward
//     };

//     static TetrisPiece GetGhostPiece(const TetrisPiece& piece, const GameState& gameState) {
//         TetrisPiece ghost = piece;
//         ghost.color = XMFLOAT4(ghost.color.x * 0.3f, ghost.color.y * 0.3f, 
//                               ghost.color.z * 0.3f, 0.5f);

//         // Drop ghost piece to lowest valid position
//         while (!CheckCollision(ghost, 0, -1, 0, gameState)) {
//             ghost.position.y -= 1.0f;
//         }
//         return ghost;
//     }

//     static bool TryRotation(TetrisPiece& piece, char axis, const GameState& gameState) {
//         TetrisPiece rotated = piece;
//         RotatePiece(rotated, axis);

//         // Try wall kick positions
//         for (const auto& offset : WALL_KICK_TESTS) {
//             rotated.position.x = piece.position.x + offset.x;
//             rotated.position.y = piece.position.y + offset.y;
//             rotated.position.z = piece.position.z + offset.z;

//             if (!CheckCollision(rotated, 0, 0, 0, gameState)) {
//                 piece = rotated;
//                 return true;
//             }
//         }
//         return false;
//     }

//     static void RotatePiece(TetrisPiece& piece, char axis) {
//         for (auto& block : piece.blocks) {
//             float x = block.x;
//             float y = block.y;
//             float z = block.z;

//             switch (axis) {
//                 case 'x':
//                     block.y = -z;
//                     block.z = y;
//                     piece.rotation = (piece.rotation + 1) % 4;
//                     break;
//                 case 'y':
//                     block.x = -z;
//                     block.z = x;
//                     piece.rotation = (piece.rotation + 1) % 4;
//                     break;
//                 case 'z':
//                     block.x = -y;
//                     block.y = x;
//                     piece.rotation = (piece.rotation + 1) % 4;
//                     break;
//             }
//         }
//     }

// private:
//     static bool CheckCollision(const TetrisPiece& piece, int dx, int dy, int dz, 
//                              const GameState& gameState) {
//         for (const auto& block : piece.blocks) {
//             int newX = piece.position.x + block.x + dx;
//             int newY = piece.position.y + block.y + dy;
//             int newZ = piece.position.z + block.z + dz;

//             if (newX < 0 || newX >= GameState::GRID_WIDTH ||
//                 newY < 0 || newY >= GameState::GRID_HEIGHT ||
//                 newZ < 0 || newZ >= GameState::GRID_DEPTH) {
//                 return true;
//             }

//             if (gameState.grid[newX][newY][newZ]) {
//                 return true;
//             }
//         }
//         return false;
//     }
// };