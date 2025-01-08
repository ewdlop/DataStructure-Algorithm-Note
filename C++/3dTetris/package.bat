@echo off
REM package.bat - Create distributable package

set BUILD_DIR=build
set PACKAGE_DIR=package
set CONFIG=Release
set VERSION=1.0.0

REM Clean package directory
if exist %PACKAGE_DIR% rmdir /S /Q %PACKAGE_DIR%
mkdir %PACKAGE_DIR%

REM Copy executable and dependencies
xcopy /Y %BUILD_DIR%\bin\%CONFIG%\Tetris3D.exe %PACKAGE_DIR%
xcopy /E /I /Y %BUILD_DIR%\bin\%CONFIG%\assets %PACKAGE_DIR%\assets
xcopy /E /I /Y %BUILD_DIR%\bin\%CONFIG%\shaders %PACKAGE_DIR%\shaders

REM Create ZIP file
powershell Compress-Archive -Path %PACKAGE_DIR%\* -DestinationPath Tetris3D-%VERSION%.zip

echo Package creation complete