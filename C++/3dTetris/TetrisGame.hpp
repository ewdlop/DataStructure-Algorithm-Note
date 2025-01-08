#include "Game.h"
#include <random>

class TetrisGame {
public:
    TetrisGame() : m_audioSystem(), m_isInitialized(false) {
        ResetGame();
    }

    bool Initialize() {
        if (!m_audioSystem.Initialize()) {
            return false;
        }

        // Start background music
        m_audioSystem.PlaySound(AudioSystem::BACKGROUND_MUSIC, true);
        m_isInitialized = true;
        return true;
    }

    void Update(float deltaTime) {
        if (!m_isInitialized || m_gameState.isGameOver) return;

        // Update drop timer
        m_gameState.dropTimer += deltaTime;
        if (m_gameState.dropTimer >= m_gameState.dropInterval) {
            m_gameState.dropTimer = 0;
            MovePieceDown();
        }
    }

    void MovePieceDown() {
        if (!CheckCollision(0, -1, 0)) {
            m_currentPiece.position.y -= 1;
            m_audioSystem.PlaySound(AudioSystem::MOVE);
        } else {
            LockPiece();
        }
    }

    void MovePiece(int dx, int dz) {
        if (!CheckCollision(dx, 0, dz)) {
            m_currentPiece.position.x += dx;
            m_currentPiece.position.z += dz;
            m_audioSystem.PlaySound(AudioSystem::MOVE);
        }
    }

    void RotatePiece(char axis) {
        TetrisPiece tempPiece = m_currentPiece;
        
        for (auto& block : tempPiece.blocks) {
            float x = block.x;
            float y = block.y;
            float z = block.z;

            switch (axis) {
                case 'x':
                    block.y = -z;
                    block.z = y;
                    break;
                case 'y':
                    block.x = -z;
                    block.z = x;
                    break;
                case 'z':
                    block.x = -y;
                    block.y = x;
                    break;
            }
        }

        if (!CheckCollision(0, 0, 0, tempPiece)) {
            m_currentPiece = tempPiece;
            m_audioSystem.PlaySound(AudioSystem::ROTATE);
        }
    }

    void InstantDrop() {
        while (!CheckCollision(0, -1, 0)) {
            m_currentPiece.position.y -= 1;
        }
        LockPiece();
        m_audioSystem.PlaySound(AudioSystem::DROP);
    }

private:
    AudioSystem m_audioSystem;
    GameState m_gameState;
    TetrisPiece m_currentPiece;
    TetrisPiece m_nextPiece;
    bool m_isInitialized;
    int m_previousLevel;
    std::mt19937 m_rng{std::random_device{}()};

    bool CheckCollision(int dx, int dy, int dz, const TetrisPiece& piece = m_currentPiece) {
        for (const auto& block : piece.blocks) {
            int newX = piece.position.x + block.x + dx;
            int newY = piece.position.y + block.y + dy;
            int newZ = piece.position.z + block.z + dz;

            if (newX < 0 || newX >= GRID_WIDTH ||
                newY < 0 || newY >= GRID_HEIGHT ||
                newZ < 0 || newZ >= GRID_DEPTH) {
                return true;
            }

            if (m_gameState.grid[newX][newY][newZ]) {
                return true;
            }
        }
        return false;
    }

    void LockPiece() {
        for (const auto& block : m_currentPiece.blocks) {
            int x = m_currentPiece.position.x + block.x;
            int y = m_currentPiece.position.y + block.y;
            int z = m_currentPiece.position.z + block.z;
            m_gameState.grid[x][y][z] = true;
        }

        CheckLines();
        SpawnNewPiece();
    }

    void CheckLines() {
        int linesCleared = 0;

        for (int y = 0; y < GRID_HEIGHT; y++) {
            bool layerComplete = true;
            
            for (int x = 0; x < GRID_WIDTH && layerComplete; x++) {
                for (int z = 0; z < GRID_DEPTH && layerComplete; z++) {
                    if (!m_gameState.grid[x][y][z]) {
                        layerComplete = false;
                    }
                }
            }

            if (layerComplete) {
                linesCleared++;
                
                for (int y2 = y; y2 < GRID_HEIGHT - 1; y2++) {
                    for (int x = 0; x < GRID_WIDTH; x++) {
                        for (int z = 0; z < GRID_DEPTH; z++) {
                            m_gameState.grid[x][y2][z] = m_gameState.grid[x][y2 + 1][z];
                        }
                    }
                }

                for (int x = 0; x < GRID_WIDTH; x++) {
                    for (int z = 0; z < GRID_DEPTH; z++) {
                        m_gameState.grid[x][GRID_HEIGHT - 1][z] = false;
                    }
                }

                y--;
            }
        }

        if (linesCleared > 0) {
            m_gameState.linesCleared += linesCleared;
            m_gameState.score += CalculateScore(linesCleared);
            m_previousLevel = m_gameState.level;
            m_gameState.level = m_gameState.linesCleared / 10;
            m_gameState.dropInterval = std::max(0.1f, 1.0f - (m_gameState.level * 0.1f));
            
            m_audioSystem.PlaySound(AudioSystem::LINE_CLEAR);
            if (m_gameState.level > m_previousLevel) {
                m_audioSystem.PlaySound(AudioSystem::LEVEL_UP);
            }
        }
    }

    int CalculateScore(int lines) {
        static const int SCORES[] = {100, 300, 500, 800};
        return SCORES[lines - 1] * (m_gameState.level + 1);
    }

    void SpawnNewPiece() {
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
            {1.0f, 0.0f, 0.0f, 1.0f},  // Red
            {0.0f, 1.0f, 0.0f, 1.0f},  // Green
            {0.0f, 0.0f, 1.0f, 1.0f},  // Blue
            {1.0f, 1.0f, 0.0f, 1.0f},  // Yellow
            {1.0f, 0.0f, 1.0f, 1.0f},  // Magenta
            {0.0f, 1.0f, 1.0f, 1.0f},  // Cyan
            {1.0f, 0.5f, 0.0f, 1.0f}   // Orange
        };

        // Current piece becomes next piece
        m_currentPiece = m_nextPiece;

        // Generate new next piece
        std::uniform_int_distribution<int> pieceDistribution(0, PIECES.size() - 1);
        int pieceIndex = pieceDistribution(m_rng);
        
        m_nextPiece.blocks = PIECES[pieceIndex];
        m_nextPiece.color = COLORS[pieceIndex];
        m_nextPiece.position = XMFLOAT3(GRID_WIDTH/2 - 1, GRID_HEIGHT - 1, GRID_DEPTH/2 - 1);

        // Check for game over
        if (CheckCollision(0, 0, 0, m_currentPiece)) {
            m_gameState.isGameOver = true;
            m_audioSystem.PlaySound(AudioSystem::GAME_OVER);
        }
    }

    void ResetGame() {
        m_gameState = GameState();
        m_previousLevel = 0;
        
        // Clear grid
        memset(m_gameState.grid, 0, sizeof(m_gameState.grid));
        
        // Generate first pieces
        SpawnNewPiece();
        SpawnNewPiece();
    }
};