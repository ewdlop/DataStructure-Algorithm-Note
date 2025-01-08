#pragma once
#include <DirectXMath.h>
#include <array>
#include <vector>

using namespace DirectX;


struct GameState {
    // Game stats
    int score;
    int level;
    int linesCleared;
    bool isGameOver;
    
    // Camera
    float cameraPitch;
    float cameraYaw;
    float cameraDistance;
    
    // Current piece
    struct {
        std::vector<XMFLOAT3> blocks;
        XMFLOAT3 position;
        XMFLOAT4 color;
    } currentPiece;
    
    // Next piece preview
    struct {
        std::vector<XMFLOAT3> blocks;
        XMFLOAT4 color;
    } nextPiece;
    
    // Grid state
    static const int GRID_WIDTH = 6;
    static const int GRID_HEIGHT = 12;
    static const int GRID_DEPTH = 6;
    bool grid[GRID_WIDTH][GRID_HEIGHT][GRID_DEPTH];
    
    // Game timing
    float dropTimer;
    float dropInterval;
    
    GameState() :
        score(0),
        level(0),
        linesCleared(0),
        isGameOver(false),
        cameraPitch(0.0f),
        cameraYaw(0.0f),
        cameraDistance(15.0f),
        dropTimer(0.0f),
        dropInterval(1.0f)
    {
        memset(grid, 0, sizeof(grid));
    }
};

class GameState {
public:
    // Game constants
    static constexpr int GRID_WIDTH = 6;
    static constexpr int GRID_HEIGHT = 12;
    static constexpr int GRID_DEPTH = 6;
    static constexpr float INITIAL_DROP_INTERVAL = 1.0f;
    static constexpr float MIN_DROP_INTERVAL = 0.1f;
    static constexpr float DROP_SPEED_INCREASE = 0.1f;
    static constexpr int LINES_PER_LEVEL = 10;
    
    // Score constants
    static constexpr std::array<int, 4> LINE_CLEAR_SCORES = {100, 300, 500, 800};
    
    // Piece colors
    static constexpr std::array<XMFLOAT4, 7> PIECE_COLORS = {{
        {1.0f, 0.0f, 0.0f, 1.0f}, // Red
        {0.0f, 1.0f, 0.0f, 1.0f}, // Green
        {0.0f, 0.0f, 1.0f, 1.0f}, // Blue
        {1.0f, 1.0f, 0.0f, 1.0f}, // Yellow
        {1.0f, 0.0f, 1.0f, 1.0f}, // Magenta
        {0.0f, 1.0f, 1.0f, 1.0f}, // Cyan
        {1.0f, 0.5f, 0.0f, 1.0f}  // Orange
    }};

    // Piece templates
    struct PieceTemplate {
        std::array<XMFLOAT3, 4> blocks;
        int rotationSymmetry; // 1 = no rotation, 2 = 180°, 4 = 90°
    };

    static constexpr std::array<PieceTemplate, 7> PIECE_TEMPLATES = {{
        // I Piece
        {{{0,0,0}, {1,0,0}, {2,0,0}, {3,0,0}}, 2},
        // L Piece
        {{{0,0,0}, {1,0,0}, {2,0,0}, {2,1,0}}, 4},
        // J Piece
        {{{0,0,0}, {1,0,0}, {2,0,0}, {0,1,0}}, 4},
        // O Piece
        {{{0,0,0}, {1,0,0}, {0,1,0}, {1,1,0}}, 1},
        // S Piece
        {{{1,0,0}, {2,0,0}, {0,1,0}, {1,1,0}}, 2},
        // T Piece
        {{{1,0,0}, {0,1,0}, {1,1,0}, {2,1,0}}, 4},
        // Z Piece
        {{{0,0,0}, {1,0,0}, {1,1,0}, {2,1,0}}, 2}
    }};

    // Game grid using 3D std::array
    using GridType = std::array<
        std::array<
            std::array<bool, GRID_DEPTH>,
            GRID_HEIGHT
        >,
        GRID_WIDTH
    >;
    
    GridType grid{};

    struct {
        std::array<XMFLOAT3, 4> blocks;
        XMFLOAT3 position;
        XMFLOAT4 color;
        int type;
        int rotation;
    } currentPiece;

    int score{0};
    int level{0};
    int linesCleared{0};
    bool isGameOver{false};
    float dropTimer{0.0f};
    float dropInterval{INITIAL_DROP_INTERVAL};

    // Helper functions
    constexpr bool IsValidPosition(int x, int y, int z) const {
        return x >= 0 && x < GRID_WIDTH &&
               y >= 0 && y < GRID_HEIGHT &&
               z >= 0 && z < GRID_DEPTH;
    }

    float CalculateDropInterval() const {
        return std::max(MIN_DROP_INTERVAL, 
                       INITIAL_DROP_INTERVAL - (level * DROP_SPEED_INCREASE));
    }

    int CalculateScore(int lines) const {
        return LINE_CLEAR_SCORES[lines - 1] * (level + 1);
    }

    void Reset() {
        grid = GridType{};
        score = 0;
        level = 0;
        linesCleared = 0;
        isGameOver = false;
        dropTimer = 0.0f;
        dropInterval = INITIAL_DROP_INTERVAL;
    }
};