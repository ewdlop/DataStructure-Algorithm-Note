@echo off
REM build.bat - Main build script

set BUILD_DIR=build
set CONFIG=Release
set PLATFORM=x64

REM Parse arguments
:parse
if "%~1"=="" goto :end_parse
if /i "%~1"=="debug" set CONFIG=Debug
if /i "%~1"=="release" set CONFIG=Release
if /i "%~1"=="x86" set PLATFORM=Win32
if /i "%~1"=="x64" set PLATFORM=x64
shift
goto :parse
:end_parse

REM Create build directory
if not exist %BUILD_DIR% mkdir %BUILD_DIR%

REM Generate project files
cmake -B %BUILD_DIR% -A %PLATFORM% -S .

REM Build the project
cmake --build %BUILD_DIR% --config %CONFIG%

REM Copy assets
xcopy /E /I /Y assets %BUILD_DIR%\bin\%CONFIG%\assets
xcopy /E /I /Y shaders %BUILD_DIR%\bin\%CONFIG%\shaders

echo Build complete for %CONFIG% %PLATFORM%