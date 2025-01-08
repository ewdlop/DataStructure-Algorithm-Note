// Add these to your global variables
struct GameState {
    int score;
    int level;
    int linesCleared;
    bool isGameOver;
    float cameraPitch;
    float cameraYaw;
} g_gameState;

// Audio resources
struct AudioData g_moveSound;
struct AudioData g_rotateSound;
struct AudioData g_lockSound;
struct AudioData g_clearSound;
struct AudioData g_gameOverSound;
struct AudioData g_bgMusic;

// Piece templates

void Update(float deltaTime) {
    if (g_gameState.isGameOver)
        return;

    // Update drop timer based on level
    float dropInterval = max(0.1f, DROP_INTERVAL - (g_gameState.level * 0.1f));
    g_dropTimer += deltaTime;
    
    if (g_dropTimer >= dropInterval) {
        g_dropTimer = 0.0f;
        
        // Try to move piece down
        if (!CheckCollision(
            g_currentPiece.position.x,
            g_currentPiece.position.y - 1,
            g_currentPiece.position.z))
        {
            g_currentPiece.position.y -= 1;
        }
        else {
            LockPiece();
        }
    }

    // Update camera position based on rotation
    XMMATRIX rotationMatrix = XMMatrixRotationRollPitchYaw(
        g_gameState.cameraPitch,
        g_gameState.cameraYaw,
        0.0f
    );
    
    XMVECTOR basePos = XMVectorSet(0.0f, 0.0f, -g_cameraDistance, 0.0f);
    XMVECTOR rotatedPos = XMVector3Transform(basePos, rotationMatrix);
    XMVECTOR targetPos = XMVectorSet(0.0f, GRID_HEIGHT/2.0f, 0.0f, 0.0f);
    
    g_cameraPos = rotatedPos + targetPos;
    g_cameraTarget = targetPos;
    g_cameraUp = XMVectorSet(0.0f, 1.0f, 0.0f, 0.0f);
}

const std::vector<std::vector<XMFLOAT3>> PIECE_TEMPLATES = {
    // I Piece
    {
        XMFLOAT3(0, 0, 0),
        XMFLOAT3(1, 0, 0),
        XMFLOAT3(2, 0, 0),
        XMFLOAT3(3, 0, 0)
    },
    // L Piece
    {
        XMFLOAT3(0, 0, 0),
        XMFLOAT3(0, 1, 0),
        XMFLOAT3(0, 2, 0),
        XMFLOAT3(1, 2, 0)
    },
    // T Piece
    {
        XMFLOAT3(0, 0, 0),
        XMFLOAT3(1, 0, 0),
        XMFLOAT3(2, 0, 0),
        XMFLOAT3(1, 1, 0)
    }
    // Add more piece templates as needed
};

bool CheckCollision(float x, float y, float z) {
    // Check bounds
    if (x < 0 || x >= GRID_WIDTH || y < 0 || z < 0 || z >= GRID_DEPTH)
        return true;

    // Check grid occupation
    if (y < GRID_HEIGHT)
        return g_gameGrid[(int)x][(int)y][(int)z];
        
    return false;
}

void RotatePiece(char axis) {
    TetrisPiece temp = g_currentPiece;
    
    // Rotate blocks around specified axis
    for (auto& block : temp.blocks) {
        float x = block.x, y = block.y, z = block.z;
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

    // Check if rotation is valid
    bool valid = true;
    for (const auto& block : temp.blocks) {
        if (CheckCollision(
            temp.position.x + block.x,
            temp.position.y + block.y,
            temp.position.z + block.z)) {
            valid = false;
            break;
        }
    }

    if (valid) {
        g_currentPiece = temp;
        PlaySound(g_rotateSound);
    }
}

void CheckLines() {
    int linesCleared = 0;
    
    for (int y = 0; y < GRID_HEIGHT; y++) {
        bool layerFull = true;
        for (int x = 0; x < GRID_WIDTH && layerFull; x++) {
            for (int z = 0; z < GRID_DEPTH && layerFull; z++) {
                if (!g_gameGrid[x][y][z])
                    layerFull = false;
            }
        }

        if (layerFull) {
            linesCleared++;
            
            // Clear layer
            for (int x = 0; x < GRID_WIDTH; x++) {
                for (int z = 0; z < GRID_DEPTH; z++) {
                    for (int j = y; j < GRID_HEIGHT - 1; j++) {
                        g_gameGrid[x][j][z] = g_gameGrid[x][j + 1][z];
                    }
                    g_gameGrid[x][GRID_HEIGHT - 1][z] = false;
                }
            }
        }
    }

    if (linesCleared > 0) {
        PlaySound(g_clearSound);
        g_gameState.linesCleared += linesCleared;
        g_gameState.score += linesCleared * linesCleared * 100 * (g_gameState.level + 1);
        g_gameState.level = g_gameState.linesCleared / 10;
    }
}

void SpawnNewPiece() {
    // Select random piece template
    int pieceIndex = rand() % PIECE_TEMPLATES.size();
    g_currentPiece.blocks = PIECE_TEMPLATES[pieceIndex];
    g_currentPiece.position = XMFLOAT3(GRID_WIDTH/2, GRID_HEIGHT-1, GRID_DEPTH/2);
    
    // Random color for the piece
    float r = (float)(rand() % 100) / 100.0f;
    float g = (float)(rand() % 100) / 100.0f;
    float b = (float)(rand() % 100) / 100.0f;
    g_currentPiece.color = XMFLOAT4(r, g, b, 1.0f);

    // Check for game over
    for (const auto& block : g_currentPiece.blocks) {
        if (CheckCollision(
            g_currentPiece.position.x + block.x,
            g_currentPiece.position.y + block.y,
            g_currentPiece.position.z + block.z)) {
            g_gameState.isGameOver = true;
            PlaySound(g_gameOverSound);
            break;
        }
    }
}

void LockPiece() {
    for (const auto& block : g_currentPiece.blocks) {
        int x = (int)(g_currentPiece.position.x + block.x);
        int y = (int)(g_currentPiece.position.y + block.y);
        int z = (int)(g_currentPiece.position.z + block.z);
        
        if (x >= 0 && x < GRID_WIDTH && y >= 0 && y < GRID_HEIGHT && z >= 0 && z < GRID_DEPTH) {
            g_gameGrid[x][y][z] = true;
        }
    }
    PlaySound(g_lockSound);
    CheckLines();
    SpawnNewPiece();
}

void ResetGame() {
    // Clear grid
    memset(g_gameGrid, 0, sizeof(g_gameGrid));
    
    // Reset game state
    g_gameState.score = 0;
    g_gameState.level = 0;
    g_gameState.linesCleared = 0;
    g_gameState.isGameOver = false;
    g_gameState.cameraPitch = 0.0f;
    g_gameState.cameraYaw = 0.0f;
    
    // Spawn first piece
    SpawnNewPiece();
}