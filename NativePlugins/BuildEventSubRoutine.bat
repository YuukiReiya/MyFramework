@echo off
REM ====================================================================================
REM VC++�̃r���h�C�x���g�Ɏd���񂾂牺�L�G���[���o���B
REM > MSB3073:VCEnd�̓R�[�h 123 �ŏI�����܂���
REM �ǂ����S�p��������g���Ă���t�H���_�������������Ă���݂����Ȃ̂����A
REM ����Ȃ̍���Ă��Ȃ��B
REM �����炭�A�u�O���ˑ��֌W�v�u�\�[�X�t�@�C���v�u�w�b�_�[�t�@�C���v�u���\�[�X�t�@�C���v
REM ���̕ӂ������������Ă���̂�������Ȃ��B
REM
REM �����Ƃ��Ď��������o�b�`�ɂ��ČĂяo���`�����B
REM ��
REM �o�̓t�@�C��(*.dll)�݂̂̃R�s�[�Ȃ̂Ńt�H���_���R�s�[��������Έ����w�肵�Ă���
REM �r���h�C�x���g�����L�̂悤�ɕύX����B
REM ��������������������������������������������������������������������
REM �yExternalDynamicLinkLibrary �r���h��̃C�x���g�z
REM ```
REM �ύX�O
REM set COPY_TARGET=$(TargetDir)$(TargetFileName)
REM �ύX��
REM set COPY_TARGET=$(TargetDir)
REM ```
REM ====================================================================================
REM �x�����ϐ�����
setlocal enabledelayedexpansion

REM �f�o�b�O�t���O"true:!=0"
set DEBUG_FLAG=0

REM �R�s�[�Ώۂ̃t�@�C��/�t�H���_ : *.dll��������$(OutDir)�������Ɏ��
set COPY_TARGET=%~1
set COPY_TARGET_EXTENSION=%~x1

REM INI�t�@�C���̃f�B���N�g�� : BuildEvent��$(SolutionDir)�������Ɏ��
set INI_DIR=%~2

REM �R�s�[�Ώۂ̃t�@�C�������݂��Ȃ���Ώ����ł��Ȃ�
if not exist "!COPY_TARGET!" (
  echo Copy target is not found: "!COPY_TARGET!"
  exit /b
)

REM ��Ȃ�J�����g�f�B���N�g��
if "!INI_DIR!"=="" ( 
  set INI_DIR="./"
  echo set current directory.
)

REM �f�B���N�g�����Ȃ�
if not exist "!INI_DIR!" (
  echo Directory is not exist: "!INI_DIR!"
  exit /b
)

REM �\�����[�V����������f�B���N�g������T�u�f�B���N�g���܂�ini�t�@�C���������B
for /r %INI_DIR% %%x in (*.ini) do (
  
  REM �ǂݍ��񂾃p�X�̏o��
  if not %DEBUG_FLAG%==0 echo READ_PATH:"%%x"

  REM �R�s�[����
  call :SUB_ROUTINE %%x
)

exit /b

REM .ini ����̃Z�N�V������ǂݍ���
:SUB_ROUTINE

REM .ini ��̓o�b�`
REM ��GetIni.bat�܂ł̑��΃p�X�Ȃ̂Ńp�X��ς�����ύX����K�v������
REM (�r���h�C�x���g����Ăяo���̂�.vcxproj����̑��΃p�X)
set BAT_PATH=..\..\bat\GetIni.bat

REM �ǂݍ��ރZ�N�V����
set TARGET_SECTION=CLONE

REM �ǂݍ��񂾃Z�N�V����
set SECTION=

REM �ǂݍ���.ini
set READ_INI=%~1

REM .ini ��͏���
for /f "tokens=1 delims==" %%k in ( !READ_INI! ) do (
  
  REM �ǂݍ��񂾃L�[
  set KEY=%%k

  REM �ǂݍ��񂾒l���Z�N�V�������ǂ������肷�� [*]
  set FORMAT=!KEY:~0,1!!KEY:~-1,1!

  REM �Z�N�V�������X�V
  if "!FORMAT!"=="[]" set SECTION=!KEY!

  REM �f�o�b�O
  if not %DEBUG_FLAG%==0 echo SECTION: "!SECTION!"
  if not %DEBUG_FLAG%==0 echo KEY: "!KEY!"
  if not %DEBUG_FLAG%==0 echo FORMAT: "!FORMAT!"

  REM �w��̃Z�N�V�����Ȃ�
  if "!SECTION!"=="[!TARGET_SECTION!]" if not "!FORMAT!"=="[]" (

    REM .ini ���
    call %BAT_PATH% :READ_INI_VAL "!TARGET_SECTION!" "!KEY!" VALUE %READ_INI%

    REM �ǂݍ��񂾃f�B���N�g�������݂��Ă�����s
    if exist "!VALUE!" (

      REM �t�@�C��(�t�H���_)�R�s�[
      
      REM �f�B���N�g���̏ꍇ
      if "!COPY_TARGET_EXTENSION!"=="" (

        REM �����񖖔���'\'�̓R�s�[�ΏۂłȂ��Ȃ��Ă��܂����߃p�X������������
        set COPY_TARGET_LAST_CHARA=!COPY_TARGET:~-1,1!
        if "!COPY_TARGET_LAST_CHARA!"=="\" (
          set COPY_TARGET=!COPY_TARGET:~0,-1!
        )
        echo Copy target is directory.
        xcopy "!COPY_TARGET!" "!VALUE!" /y
      REM �t�@�C���̏ꍇ
      ) else (
        echo Copy target is file: "!COPY_TARGET!"
        copy /y "!COPY_TARGET!" "!VALUE!"
      )
    REM �f�B���N�g�������������ꍇ
    ) else (
      echo Copy target is not found: "!VALUE!"
    )
  )
  REM ���s
  if not %DEBUG_FLAG%==0 echo.
)
exit /b

REM �x�����ϐ��I��
endlocal