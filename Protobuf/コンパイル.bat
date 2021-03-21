@echo off
REM DEBUG�o�� true!=0
set DEBUG_FLAG=0

REM �o�b�`�t�H���_������J�����g�f�B���N�g���̃p�X
set CURRENT_DIR_PATH=%~dp0

REM �R���p�C������t�@�C��������t�H���_
set SOURCE_DIRECTORY=sources\

REM �R���p�C���Ώۂ̃t�@�C���g���q
set SOURCE_FILE_EXTENSION=.proto

REM �R���p�C�����ʂ̊i�[��e�f�B���N�g��
set COMPILED_OUT_DIRECTORY=compiled\

REM �R���p�C������C#�t�@�C���̊i�[��t�H���_
set COMPILED_OUT_CSHARP_FOLDER=%COMPILED_OUT_DIRECTORY%csharp

REM �R���p�C������grpc(����.cs�`��)�̊i�[��t�H���_
set COMPILED_OUT_GRPC_FOLDER=%COMPILED_OUT_DIRECTORY%grpc

REM �v���O�C���t�@�C���̃p�X
REM �������͐�΃p�X����Ȃ��ƃ_���Ȃ̂ŃJ�����g����̑��΃p�X���΃p�X�ɕϊ�
set GRPC_PLUGINS_PATH=%CURRENT_DIR_PATH%grpc_csharp_plugin.exe

REM �t�H���_���ɂ���S�t�@�C���ɑ΂��đ��点��̂Œx�����ϐ����g��
setlocal enabledelayedexpansion

REM �o�͐�f�B���N�g�����폜���ăN���[���ȏ�Ԃɂ��Ă����B
if exist "%COMPILED_OUT_DIRECTORY%" rmdir /s /q "%COMPILED_OUT_DIRECTORY%"

REM �R���p�C������t�@�C����T��
for %%a in (%SOURCE_DIRECTORY%*) do (

  set READ_FILE_PATH=%%a
  if not DEBUG_FLAG==0 echo �ǂݍ��񂾃t�@�C����:!READ_FILE_PATH!
  REM �g���q
  set EXTENSION=%%~xa
  if not DEBUG_FLAG==0 echo �t�@�C���̊g���q:!EXTENSION!

  REM �g���q��".proto"�Ȃ�R���p�C���Ώ�
  if "!EXTENSION!"=="%SOURCE_FILE_EXTENSION%" (

    
    REM �o�͐�̃f�B���N�g�����Ȃ���΍��
    if not exist "%COMPILED_OUT_CSHARP_FOLDER%" mkdir "%COMPILED_OUT_CSHARP_FOLDER%"
    if not exist "%COMPILED_OUT_GRPC_FOLDER%" mkdir "%COMPILED_OUT_GRPC_FOLDER%"
    
    REM �R���p�C��
    if not DEBUG_FLAG==0 echo ���̃t�@�C�����R���p�C���B: !READ_FILE_PATH!
    protoc -I . --csharp_out="%COMPILED_OUT_CSHARP_FOLDER%" --grpc_out="%COMPILED_OUT_GRPC_FOLDER%" "!READ_FILE_PATH!" --plugin=protoc-gen-grpc="%GRPC_PLUGINS_PATH%"

  REM �g���q��".proto"�łȂ����߃R���p�C���ΏۊO
  ) else (
    if not DEBUG_FLAG==0 echo �g���q���Ⴄ�̂ŃX�L�b�v�B: !READ_FILE_PATH!
  )
) 

REM �x�����ϐ����g���̂̓R�R�ŏI���
endlocal

REM �ȈՃG���[����.
REM ��������G���[���L���b�`������pause���Ăяo���B
if not %ERRORLEVEL%==0 (
  echo.
  echo �G���[���������܂����B
  pause
)

