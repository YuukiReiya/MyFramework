@echo off
REM DEBUG�o�� true!=0
set DEBUG_FLAG=0

REM ���ɑ��݂���t�@�C�����㏑�����邩 true!=0
set IS_OVERRIDE=1

REM �o�b�`�̃p�X
set INI_READER_PATH=%~dp0..\bat\GetIni.bat

REM �ǂݍ��ݐ��ini�t�@�C��
set INI_FILE_PATH=util.ini

REM ini�̓ǂݍ��ރZ�N�V������
REM CS
set INI_SECTION_CS=DESTINATION_DIRECTORY_CS
REM GRPC
set INI_SECTION_GRPC=DESTINATION_DIRECTORY_GRPC

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

REM �o�͐�f�B���N�g�����폜���ăN���[���ȏ�Ԃɂ��Ă����B
call :REMOVE_COMPILED_DIRECTORY

REM �t�H���_���ɂ���S�t�@�C���ɑ΂��đ��点��̂Œx�����ϐ����g��
setlocal enabledelayedexpansion

REM �R���p�C��
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

REM �ȈՃG���[����.
REM ��������G���[���L���b�`������pause���Ăяo���B
if not %ERRORLEVEL%==0 (
  echo.
  echo �R���p�C���G���[���������܂����B
  pause
)

REM �����v���Z�X
call :CS_SUB_ROUTINE
call :GRPC_SUB_ROUTINE

REM �������̃t�H���_�͎c���Ȃ�
call :REMOVE_COMPILED_DIRECTORY

REM �ȈՃG���[����.
REM ��������G���[���L���b�`������pause���Ăяo���B
if not %ERRORLEVEL%==0 (
  echo.
  echo �����v���Z�X�֘A�ŃG���[���������܂����B
  pause
)


exit /b

:CS_SUB_ROUTINE
set COPY_DIRECTORY=
for /f "tokens=1 delims==" %%k in (%INI_FILE_PATH%) do (

  set KEY=%%k
  set FORMAT=!KEY:~0,1!!KEY:~-1,1!

  REM DEBUG�o��
  if not %DEBUG_FLAG%==0 echo [CSharp]���ǂݍ���ł���l��
  if not %DEBUG_FLAG%==0 echo [CSharp]�L�[:!KEY!
  if not %DEBUG_FLAG%==0 echo [CSharp]�t�H�[�}�b�g:!FORMAT!

  if not "!FORMAT!"=="[]" (
    call %INI_READER_PATH% :READ_INI_VAL "%INI_SECTION_CS%" !KEY! COPY_DIRECTORY %INI_FILE_PATH%
    if not %DEBUG_FLAG%==0 echo [CSharp]�t�@�C������ǂݎ�����f�B���N�g��:!COPY_DIRECTORY!


    REM �R�s�[��̃f�B���N�g�������邩�m�F

    if not exist !COPY_DIRECTORY! (
      if not "!COPY_DIRECTORY!"=="NULL" (
          mkdir !COPY_DIRECTORY!
          REM �R�s�[��f�B���N�g�������݂��Ȃ��킯�Ȃ��͂��Ȃ̂Ńf�o�b�O�t���O�֌W�Ȃ��R�s�[��f�B���N�g�����Ȃ������ꍇ��echo�Œʒm����.
          echo [CSharp]�R�s�[��f�B���N�g�������݂��Ȃ������̂ō쐬���܂����B : !COPY_DIRECTORY!
      )
    )
    if not !COPY_DIRECTORY!==NULL (
      call :CS_COPY_LOOP
    )
  )
)
exit /b

:GRPC_SUB_ROUTINE
set COPY_DIRECTORY=
for /f "tokens=1 delims==" %%k in (%INI_FILE_PATH%) do (

  set KEY=%%k
  set FORMAT=!KEY:~0,1!!KEY:~-1,1!

  REM DEBUG�o��
  if not %DEBUG_FLAG%==0 echo [GRPC]���ǂݍ���ł���l��
  if not %DEBUG_FLAG%==0 echo [GRPC]�L�[:!KEY!
  if not %DEBUG_FLAG%==0 echo [GRPC]�t�H�[�}�b�g:!FORMAT!

  if not "!FORMAT!"=="[]" (
    call %INI_READER_PATH% :READ_INI_VAL "%INI_SECTION_GRPC%" !KEY! COPY_DIRECTORY %INI_FILE_PATH%
    if not %DEBUG_FLAG%==0 echo [GRPC]�t�@�C������ǂݎ�����f�B���N�g��:!COPY_DIRECTORY!




    REM �R�s�[��̃f�B���N�g�������邩�m�F
    if not exist !COPY_DIRECTORY! (
      if not "!COPY_DIRECTORY!"=="NULL" (
          mkdir !COPY_DIRECTORY!
      REM �R�s�[��f�B���N�g�������݂��Ȃ��킯�Ȃ��͂��Ȃ̂Ńf�o�b�O�t���O�֌W�Ȃ��R�s�[��f�B���N�g�����Ȃ������ꍇ��echo�Œʒm����.
          echo [GRPC]�R�s�[��f�B���N�g�������݂��Ȃ������̂ō쐬���܂����B : !COPY_DIRECTORY!
      )
    )
    if not !COPY_DIRECTORY!==NULL (
      call :GRPC_COPY_LOOP
    )
  )
)
exit /b

REM �R���p�C���o���Ă�΃t�@�C�����o���Ă�͂��Ȃ̂ŊY����CS�t�@�C����S�R�s�[
:CS_COPY_LOOP
for %%b in (%COMPILED_OUT_CSHARP_FOLDER%\*) do (
  set COPY_FILE_PATH=%%b
  if not %DEBUG_FLAG%==0 echo [CSharp]�R���p�C����̓ǂݍ��񂾃t�@�C����:!COPY_FILE_PATH!
  if not %DEBUG_FLAG%==0 echo [CSharp]�R�s�[��̏������ރf�B���N�g��:!COPY_DIRECTORY!
  if !COPY_DIRECTORY!==NULL (
    if not %DEBUG_FLAG%==0 echo [CSharp]�R�s�[��f�B���N�g����"NULL"�Ȃ̂ŃR�s�[���܂���B
  ) else (
    if exist "!COPY_FILE_PATH!" copy /y "!COPY_FILE_PATH!" "!COPY_DIRECTORY!"  
  )
) 
exit /b

REM �R���p�C���o���Ă�΃t�@�C�����o���Ă�͂��Ȃ̂ŊY����GRPC�t�@�C����S�R�s�[
:GRPC_COPY_LOOP
for %%b in (%COMPILED_OUT_GRPC_FOLDER%\*) do (
  set COPY_FILE_PATH=%%b
  if not %DEBUG_FLAG%==0 echo [GRPC]�R���p�C����̓ǂݍ��񂾃t�@�C����:!COPY_FILE_PATH!
  if not %DEBUG_FLAG%==0 echo [GRPC]�R�s�[��̏������ރf�B���N�g��:!COPY_DIRECTORY!
  if !COPY_DIRECTORY!==NULL (
    if not DEBUG_FLAG==0 echo [GRPC]�R�s�[��f�B���N�g����"NULL"�Ȃ̂ŃR�s�[���܂���B
  ) else (
    if exist "!COPY_FILE_PATH!" copy /y "!COPY_FILE_PATH!" "!COPY_DIRECTORY!"  
  )

) 
exit /b

REM �x�����ϐ����g���̂̓R�R�ŏI���
endlocal

REM �R���p�C�����ʂ̏o�̓f�B���N�g�����폜����
:REMOVE_COMPILED_DIRECTORY
if exist "%COMPILED_OUT_DIRECTORY%" rmdir /s /q "%COMPILED_OUT_DIRECTORY%"
exit /b