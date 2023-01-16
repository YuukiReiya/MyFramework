@echo off
set DEBUG_EXE_PATH=%~dp0GoogleDriveManager\bin\Debug\net5.0\Downloader.exe
set RESOURCE_ROOT=%~dp0res\project_setting.xml

REM DEBUG�o�C�i���D��
if exist %DEBUG_EXE_PATH% (

    cd %~dp0GoogleDriveManager\bin\Debug\net5.0\
    call %DEBUG_EXE_PATH% %RESOURCE_ROOT% false
    pause
    exit /b 0
)

set RELEASE_EXE=%~dp0GoogleDriveManager\bin\Release\net5.0\Downloader.exe

REM RELEASE�r���h
if exist %RELEASE_EXE% (

    cd %~dp0GoogleDriveManager\bin\Release\net5.0\
    call %RELEASE_EXE% %RESOURCE_ROOT% false
    pause
    exit /b 0
)

REM �o�C�i�������݂��Ȃ�.
@echo "not found execute file > GoogleDriveManager\bin\$(Configuration)\net5.0\*.exe"
pause
exit /b 0