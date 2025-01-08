#pragma once
#include <d2d1.h>
#include <dwrite.h>
#include <string>
#include "GameState.h"

class UI {
public:
    UI();
    ~UI();

    bool Initialize(HWND hWnd);
    void Render(const GameState& gameState);
    void Cleanup();

private:
    // Direct2D objects
    ID2D1Factory* m_d2dFactory;
    ID2D1HwndRenderTarget* m_renderTarget;
    ID2D1SolidColorBrush* m_brush;
    
    // DirectWrite objects
    IDWriteFactory* m_writeFactory;
    IDWriteTextFormat* m_textFormat;
    
    // UI elements
    struct UIElement {
        D2D1_RECT_F rect;
        std::wstring text;
        D2D1_COLOR_F color;
    };
    
    UIElement m_scoreDisplay;
    UIElement m_levelDisplay;
    UIElement m_linesDisplay;
    UIElement m_gameOverText;
    UIElement m_controlsHelp;
    
    void DrawText(const std::wstring& text, const D2D1_RECT_F& rect, 
                 const D2D1_COLOR_F& color);
    void UpdateUIElements(const GameState& gameState);
};