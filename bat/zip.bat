@echo off
REM https://cheshire-wara.com/powershell/ps-help/compress-archive-help/

REM ********************************
REM �Ăяo������
REM call <this.bat> :<���x����> <�ۑ���p�X> <�Ώۃt�@�C��>
REM ********************************

REM ���x��
set CALL_FUNC_LABEL=%~1

REM ���k��̕ۑ��t�@�C���p�X
set SAVE_ZIP_FILE_PATH=%~2

REM �𓀐�̕ۑ��t�@�C���p�X
set SAVE_UNZIP_FILE_PATH=%~2

REM ���k�Ώۂ̃t�@�C��/�t�H���_�p�X
set ZIP_COMPRESS_FILE_PATH=%~3

REM �𓀑Ώۂ̃t�@�C��/�t�H���_�p�X
set UNZIP_EXPAND_FILE_PATH=%~3

REM �𓀑Ώۂ̃t�@�C��/�t�H���_�p�X�̊g���q
set UNZIP_EXPAND_FILE_EXTENSION=%~x3

REM ���x���֔��
call %CALL_FUNC_LABEL% 
exit /b

REM ���k
:ZIP

REM -Path:ZIP�t�@�C��(�A�[�J�C�u)�ɒǉ������(���k)���̃t�@�C���p�X���w��

REM -DestinationPath:ZIP�t�@�C��(�A�[�J�C�u)�̏o�̓p�X�B

REM -Force:���[�U�[�̊m�F�����߂��A�����R�}���h���s

REM -Update:�A�[�J�C�u���̃t�@�C�����X�V

REM -CompressionLevel:���k���x��
REM Fastest:�g�p�\�ȍő��̈��k���@�A��������:�Z �� �t�@�C���T�C�Y:��
REM NoCompression:�����k �� �t�@�C���T�C�Y:�ő�(���k���ĂȂ����c)
REM Optimal:�t�@�C���T�C�Y�ɏ������Ԃ����A��������:�� �� �t�@�C���T�C�Y:�ŏ� 
REM �f�t�H���g���ƁuOptimal�v 

call powershell -command "Compress-Archive -Path '%ZIP_COMPRESS_FILE_PATH%' -CompressionLevel Optimal -DestinationPath '%SAVE_ZIP_FILE_PATH%' -Force -Update"

exit /b

REM ��
set 
:UNZIP
setlocal enabledelayedexpansion
  REM �g���q��.zip���m�F
  if not "%UNZIP_EXPAND_FILE_EXTENSION%"==".zip" (
      REM �g���q��.zip�łȂ����.zip�ɂ��ăt�@�C�������邩�Ċm�F
      set TEMP_UNZIP_EXPAND_FILE_NAME=%UNZIP_EXPAND_FILE_PATH%
      set UNZIP_EXPAND_FILE_PATH=!TEMP_UNZIP_EXPAND_FILE_NAME!.zip

      REM .zip�t�@�C�������݂��Ȃ���Ή𓀂ł��Ȃ��̂ŏ������Ȃ�
      if not exist "!UNZIP_EXPAND_FILE_PATH!" (
        echo �𓀃t�@�C����������܂���ł����B:"!UNZIP_EXPAND_FILE_PATH!"
        exit /b
      )
  )

call powershell -command "Expand-Archive -Path '!UNZIP_EXPAND_FILE_PATH!' -DestinationPath '%SAVE_UNZIP_FILE_PATH%'"
endlocal
exit /b
