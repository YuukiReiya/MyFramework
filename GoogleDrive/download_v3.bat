@echo off
set DEBUG_EXE_PATH=%~dp0bin\Debug\net5.0\Downloader.exe
set RESOURCE_ROOT=%~dp0res\config.bat.xml

REM DEBUGバイナリ優先
if exist %DEBUG_EXE_PATH% (

    call %DEBUG_EXE_PATH% %RESOURCE_ROOT%
    exit /b 0
)

set RELEASE_EXE=%~dp0bin\Release\net5.0\Downloader.exe

REM RELEASEビルド
if exist %RELEASE_EXE% (

    call %RELEASE_EXE% %RESOURCE_ROOT%
    exit /b 0
)

REM バイナリが存在しない.
@echo "not found execute file > bin\$(Configuration)\net5.0\*.exe"
exit /b 0