void Render() {
    // Clear the back buffer
    float ClearColor[4] = { 0.0f, 0.2f, 0.4f, 1.0f };
    g_pImmediateContext->ClearRenderTargetView(g_pRenderTargetView, ClearColor);

    // Setup matrices
    XMMATRIX world = XMMatrixIdentity();
    XMMATRIX view = XMMatrixLookAtLH(
        XMVectorSet(0.0f, 5.0f, -15.0f, 0.0f),
        XMVectorSet(0.0f, 0.0f, 0.0f, 0.0f),
        XMVectorSet(0.0f, 1.0f, 0.0f, 0.0f)
    );
    XMMATRIX projection = XMMatrixPerspectiveFovLH(XM_PIDIV4, WINDOW_WIDTH / (FLOAT)WINDOW_HEIGHT, 0.01f, 100.0f);

    // Setup depth stencil state
    ID3D11DepthStencilState* pDSState = nullptr;
    D3D11_DEPTH_STENCIL_DESC dsDesc = {};
    dsDesc.DepthEnable = TRUE;
    dsDesc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ALL;
    dsDesc.DepthFunc = D3D11_COMPARISON_LESS;
    g_pd3dDevice->CreateDepthStencilState(&dsDesc, &pDSState);
    g_pImmediateContext->OMSetDepthStencilState(pDSState, 1);

    // Create depth stencil texture
    ID3D11Texture2D* pDepthStencil = nullptr;
    D3D11_TEXTURE2D_DESC descDepth = {};
    descDepth.Width = WINDOW_WIDTH;
    descDepth.Height = WINDOW_HEIGHT;
    descDepth.MipLevels = 1;
    descDepth.ArraySize = 1;
    descDepth.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
    descDepth.SampleDesc.Count = 1;
    descDepth.SampleDesc.Quality = 0;
    descDepth.Usage = D3D11_USAGE_DEFAULT;
    descDepth.BindFlags = D3D11_BIND_DEPTH_STENCIL;
    g_pd3dDevice->CreateTexture2D(&descDepth, nullptr, &pDepthStencil);

    // Create depth stencil view
    ID3D11DepthStencilView* pDSV = nullptr;
    g_pd3dDevice->CreateDepthStencilView(pDepthStencil, nullptr, &pDSV);
    g_pImmediateContext->OMSetRenderTargets(1, &g_pRenderTargetView, pDSV);
    g_pImmediateContext->ClearDepthStencilView(pDSV, D3D11_CLEAR_DEPTH, 1.0f, 0);

    // Update constant buffer
    ConstantBuffer cb;
    cb.mWorld = XMMatrixTranspose(world);
    cb.mView = XMMatrixTranspose(view);
    cb.mProjection = XMMatrixTranspose(projection);
    g_pImmediateContext->UpdateSubresource(g_pConstantBuffer, 0, nullptr, &cb, 0, 0);

    // Set vertex buffer
    UINT stride = sizeof(Vertex);
    UINT offset = 0;
    g_pImmediateContext->IASetVertexBuffers(0, 1, &g_pVertexBuffer, &stride, &offset);
    g_pImmediateContext->IASetIndexBuffer(g_pIndexBuffer, DXGI_FORMAT_R16_UINT, 0);
    g_pImmediateContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

    // Set shaders and constant buffers
    g_pImmediateContext->VSSetShader(g_pVertexShader, nullptr, 0);
    g_pImmediateContext->VSSetConstantBuffers(0, 1, &g_pConstantBuffer);
    g_pImmediateContext->PSSetShader(g_pPixelShader, nullptr, 0);
    g_pImmediateContext->IASetInputLayout(g_pVertexLayout);

    // Render game grid
    for (int x = 0; x < GRID_WIDTH; x++) {
        for (int y = 0; y < GRID_HEIGHT; y++) {
            for (int z = 0; z < GRID_DEPTH; z++) {
                if (g_gameGrid[x][y][z]) {
                    world = XMMatrixTranslation(
                        x - GRID_WIDTH/2.0f,
                        y,
                        z - GRID_DEPTH/2.0f
                    );
                    cb.mWorld = XMMatrixTranspose(world);
                    g_pImmediateContext->UpdateSubresource(g_pConstantBuffer, 0, nullptr, &cb, 0, 0);
                    g_pImmediateContext->DrawIndexed(36, 0, 0);
                }
            }
        }
    }

    // Render current piece
    for (const auto& block : g_currentPiece.blocks) {
        world = XMMatrixTranslation(
            g_currentPiece.position.x + block.x - GRID_WIDTH/2.0f,
            g_currentPiece.position.y + block.y,
            g_currentPiece.position.z + block.z - GRID_DEPTH/2.0f
        );
        cb.mWorld = XMMatrixTranspose(world);
        g_pImmediateContext->UpdateSubresource(g_pConstantBuffer, 0, nullptr, &cb, 0, 0);
        g_pImmediateContext->DrawIndexed(36, 0, 0);
    }

    // Cleanup depth stencil resources
    if (pDSV) pDSV->Release();
    if (pDepthStencil) pDepthStencil->Release();
    if (pDSState) pDSState->Release();

    // Present the frame
    g_pSwapChain->Present(0, 0);
}