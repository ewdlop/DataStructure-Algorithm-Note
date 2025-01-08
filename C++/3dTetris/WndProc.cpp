LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
    switch (message) {
        case WM_KEYDOWN:
            if (g_gameState.isGameOver) {
                if (wParam == VK_RETURN) {
                    ResetGame();
                }
                break;
            }
            
            switch (wParam) {
                case VK_LEFT:  // Move left
                    if (!CheckCollision(
                        g_currentPiece.position.x - 1,
                        g_currentPiece.position.y,
                        g_currentPiece.position.z))
                    {
                        g_currentPiece.position.x -= 1;
                        PlaySound(g_moveSound);
                    }
                    break;

                case VK_RIGHT:  // Move right
                    if (!CheckCollision(
                        g_currentPiece.position.x + 1,
                        g_currentPiece.position.y,
                        g_currentPiece.position.z))
                    {
                        g_currentPiece.position.x += 1;
                        PlaySound(g_moveSound);
                    }
                    break;

                case VK_UP:  // Move forward
                    if (!CheckCollision(
                        g_currentPiece.position.x,
                        g_currentPiece.position.y,
                        g_currentPiece.position.z - 1))
                    {
                        g_currentPiece.position.z -= 1;
                        PlaySound(g_moveSound);
                    }
                    break;

                case VK_DOWN:  // Move backward
                    if (!CheckCollision(
                        g_currentPiece.position.x,
                        g_currentPiece.position.y,
                        g_currentPiece.position.z + 1))
                    {
                        g_currentPiece.position.z += 1;
                        PlaySound(g_moveSound);
                    }
                    break;

                case 'X':  // Rotate around X axis
                    RotatePiece('x');
                    break;

                case 'Y':  // Rotate around Y axis
                    RotatePiece('y');
                    break;

                case 'Z':  // Rotate around Z axis
                    RotatePiece('z');
                    break;

                case VK_SPACE:  // Drop piece
                    while (!CheckCollision(
                        g_currentPiece.position.x,
                        g_currentPiece.position.y - 1,
                        g_currentPiece.position.z))
                    {
                        g_currentPiece.position.y -= 1;
                    }
                    LockPiece();
                    break;

                case VK_RETURN:  // Reset game
                    ResetGame();
                    break;
            }
            break;

        case WM_MOUSEWHEEL:
            // Camera zoom
            g_cameraDistance -= (float)GET_WHEEL_DELTA_WPARAM(wParam) / 120.0f;
            g_cameraDistance = max(5.0f, min(g_cameraDistance, 30.0f));
            break;

        case WM_MOUSEMOVE:
            if (wParam & MK_RBUTTON) {
                // Camera rotation
                int xPos = GET_X_LPARAM(lParam);
                int yPos = GET_Y_LPARAM(lParam);
                float dx = (float)(xPos - g_lastMousePos.x) * 0.005f;
                float dy = (float)(yPos - g_lastMousePos.y) * 0.005f;
                g_gameState.cameraYaw += dx;
                g_gameState.cameraPitch += dy;
                g_gameState.cameraPitch = max(-XM_PIDIV2, min(g_gameState.cameraPitch, XM_PIDIV2));
                g_lastMousePos.x = xPos;
                g_lastMousePos.y = yPos;
            }
            break;

        case WM_DESTROY:
            PostQuitMessage(0);
            return 0;
    }
    return DefWindowProc(hWnd, message, wParam, lParam);
}