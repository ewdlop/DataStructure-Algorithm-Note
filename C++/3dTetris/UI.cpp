#include "UI.h"
#include <sstream>

UI::UI() : m_d2dFactory(nullptr), m_renderTarget(nullptr), m_brush(nullptr),
           m_writeFactory(nullptr), m_textFormat(nullptr) {}

UI::~UI() {
    Cleanup();
}

bool UI::Initialize(HWND hWnd) {
    // Create Direct2D factory
    HRESULT hr = D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED, &m_d2dFactory);
    if (FAILED(hr)) return false;

    // Get window size
    RECT rc;
    GetClientRect(hWnd, &rc);
    D2D1_SIZE_U size = D2D1::SizeU(rc.right - rc.left, rc.bottom - rc.top);

    // Create render target
    hr = m_d2dFactory->CreateHwndRenderTarget(
        D2D1::RenderTargetProperties(),
        D2D1::HwndRenderTargetProperties(hWnd, size),
        &m_renderTarget
    );
    if (FAILED(hr)) return false;

    // Create brush
    hr = m_renderTarget->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::White), &m_brush);
    if (FAILED(hr)) return false;

    // Create DirectWrite factory
    hr = DWriteCreateFactory(
        DWRITE_FACTORY_TYPE_SHARED,
        __uuidof(IDWriteFactory),
        reinterpret_cast<IUnknown**>(&m_writeFactory)
    );
    if (FAILED(hr)) return false;

    // Create text format
    hr = m_writeFactory->CreateTextFormat(
        L"Segoe UI",
        NULL,
        DWRITE_FONT_WEIGHT_NORMAL,
        DWRITE_FONT_STYLE_NORMAL,
        DWRITE_FONT_STRETCH_NORMAL,
        24,
        L"en-us",
        &m_textFormat
    );
    if (FAILED(hr)) return false;

    // Initialize UI elements
    m_scoreDisplay.rect = D2D1::RectF(10, 10, 200, 40);
    m_levelDisplay.rect = D2D1::RectF(10, 50, 200, 80);
    m_linesDisplay.rect = D2D1::RectF(10, 90, 200, 120);
    m_gameOverText.rect = D2D1::RectF(200, 200, 600, 300);
    m_controlsHelp.rect = D2D1::RectF(10, 500, 300, 580);
    
    return true;
}

void UI::UpdateUIElements(const GameState& gameState) {
    std::wstringstream ss;
    
    // Update score display
    ss << L"Score: " << gameState.score;
    m_scoreDisplay.text = ss.str();
    m_scoreDisplay.color = D2D1::ColorF(D2D1::ColorF::White);
    ss.str(L"");

    // Update level display
    ss << L"Level: " << gameState.level;
    m_levelDisplay.text = ss.str();
    m_levelDisplay.color = D2D1::ColorF(D2D1::ColorF::Yellow);
    ss.str(L"");

    // Update lines display
    ss << L"Lines: " << gameState.linesCleared;
    m_linesDisplay.text = ss.str();
    m_linesDisplay.color = D2D1::ColorF(D2D1::ColorF::Cyan);

    // Update game over text
    if (gameState.isGameOver) {
        m_gameOverText.text = L"GAME OVER\nPress Enter to restart";
        m_gameOverText.color = D2D1::ColorF(D2D1::ColorF::Red);
    } else {
        m_gameOverText.text = L"";
    }

    // Controls help
    m_controlsHelp.text = L"Controls:\nArrows: Move\nX/Y/Z: Rotate\nSpace: Drop";
    m_controlsHelp.color = D2D1::ColorF(D2D1::ColorF::Gray);
}

void UI::DrawText(const std::wstring& text, const D2D1_RECT_F& rect, const D2D1_COLOR_F& color) {
    m_brush->SetColor(color);
    m_renderTarget->DrawText(
        text.c_str(),
        static_cast<UINT32>(text.length()),
        m_textFormat,
        rect,
        m_brush
    );
}

void UI::Render(const GameState& gameState) {
    m_renderTarget->BeginDraw();
    
    // Update UI elements
    UpdateUIElements(gameState);

    // Draw all UI elements
    DrawText(m_scoreDisplay.text, m_scoreDisplay.rect, m_scoreDisplay.color);
    DrawText(m_levelDisplay.text, m_levelDisplay.rect, m_levelDisplay.color);
    DrawText(m_linesDisplay.text, m_linesDisplay.rect, m_linesDisplay.color);
    
    if (!m_gameOverText.text.empty()) {
        DrawText(m_gameOverText.text, m_gameOverText.rect, m_gameOverText.color);
    }
    
    DrawText(m_controlsHelp.text, m_controlsHelp.rect, m_controlsHelp.color);

    m_renderTarget->EndDraw();
}

void UI::Cleanup() {
    if (m_textFormat) m_textFormat->Release();
    if (m_writeFactory) m_writeFactory->Release();
    if (m_brush) m_brush->Release();
    if (m_renderTarget) m_renderTarget->Release();
    if (m_d2dFactory) m_d2dFactory->Release();
}