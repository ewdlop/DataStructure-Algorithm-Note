#pragma once
#include <array>
#include <DirectXMath.h>

using namespace DirectX;

class TetrisPiece {
public:
    // Piece types
    enum class Type {
        I, J, L, O, S, T, Z
    };

    // Each piece defines a 4x4x4 grid of blocks
    static constexpr size_t GRID_SIZE = 4;
    using BlockGrid = std::array<std::array<std::array<bool, GRID_SIZE>, GRID_SIZE>, GRID_SIZE>;

    // Piece definitions (3D)
    static constexpr std::array<BlockGrid, 7> PIECE_DEFINITIONS = {{
        // I Piece (flat)
        {{{
            {{0,0,0,0}, {0,0,0,0}, {1,1,1,1}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}}
        }}},
        
        // J Piece
        {{{
            {{0,0,0,0}, {1,0,0,0}, {1,1,1,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}}
        }}},

        // L Piece
        {{{
            {{0,0,0,0}, {0,0,1,0}, {1,1,1,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}}
        }}},

        // O Piece (cube)
        {{{
            {{0,0,0,0}, {0,1,1,0}, {0,1,1,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}}
        }}},

        // S Piece
        {{{
            {{0,0,0,0}, {0,1,1,0}, {1,1,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}}
        }}},

        // T Piece
        {{{
            {{0,0,0,0}, {0,1,0,0}, {1,1,1,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}}
        }}},

        // Z Piece
        {{{
            {{0,0,0,0}, {1,1,0,0}, {0,1,1,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0}}
        }}}
    }};

    // Colors for each piece type
    static constexpr std::array<XMFLOAT4, 7> PIECE_COLORS = {{
        {1.0f, 0.0f, 0.0f, 1.0f}, // I - Red
        {0.0f, 1.0f, 0.0f, 1.0f}, // J - Green
        {0.0f, 0.0f, 1.0f, 1.0f}, // L - Blue
        {1.0f, 1.0f, 0.0f, 1.0f}, // O - Yellow
        {1.0f, 0.0f, 1.0f, 1.0f}, // S - Magenta
        {0.0f, 1.0f, 1.0f, 1.0f}, // T - Cyan
        {1.0f, 0.5f, 0.0f, 1.0f}  // Z - Orange
    }};

    TetrisPiece(Type type) 
        : m_type(type)
        , m_grid(PIECE_DEFINITIONS[static_cast<size_t>(type)])
        , m_color(PIECE_COLORS[static_cast<size_t>(type)])
        , m_position(0, 0, 0)
        , m_rotation(0) {}

    void RotateX() {
        BlockGrid newGrid{};
        for (size_t z = 0; z < GRID_SIZE; z++) {
            for (size_t y = 0; y < GRID_SIZE; y++) {
                for (size_t x = 0; x < GRID_SIZE; x++) {
                    newGrid[x][GRID_SIZE-1-z][y] = m_grid[x][y][z];
                }
            }
        }
        m_grid = newGrid;
    }

    void RotateY() {
        BlockGrid newGrid{};
        for (size_t z = 0; z < GRID_SIZE; z++) {
            for (size_t y = 0; y < GRID_SIZE; y++) {
                for (size_t x = 0; x < GRID_SIZE; x++) {
                    newGrid[z][y][GRID_SIZE-1-x] = m_grid[x][y][z];
                }
            }
        }
        m_grid = newGrid;
    }

    void RotateZ() {
        BlockGrid newGrid{};
        for (size_t z = 0; z < GRID_SIZE; z++) {
            for (size_t y = 0; y < GRID_SIZE; y++) {
                for (size_t x = 0; x < GRID_SIZE; x++) {
                    newGrid[y][GRID_SIZE-1-x][z] = m_grid[x][y][z];
                }
            }
        }
        m_grid = newGrid;
    }

    // Movement
    void Move(int dx, int dy, int dz) {
        m_position.x += dx;
        m_position.y += dy;
        m_position.z += dz;
    }

    // Getters
    const BlockGrid& GetGrid() const { return m_grid; }
    const XMFLOAT3& GetPosition() const { return m_position; }
    const XMFLOAT4& GetColor() const { return m_color; }
    Type GetType() const { return m_type; }

    // Check if a specific position in the piece's grid is filled
    bool IsFilled(size_t x, size_t y, size_t z) const {
        if (x >= GRID_SIZE || y >= GRID_SIZE || z >= GRID_SIZE) return false;
        return m_grid[x][y][z];
    }

private:
    Type m_type;
    BlockGrid m_grid;
    XMFLOAT4 m_color;
    XMFLOAT3 m_position;
    int m_rotation;
};