@echo off
REM run.bat - Run the game

set BUILD_DIR=build
set CONFIG=Release

REM Check if built
if not exist %BUILD_DIR%\bin\%CONFIG%\Tetris3D.exe (
    echo Game not built. Building now...
    call build.bat %CONFIG%
)

REM Run the game
start "" "%BUILD_DIR%\bin\%CONFIG%\Tetris3D.exe"