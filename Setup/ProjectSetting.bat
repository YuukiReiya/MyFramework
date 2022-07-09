@echo off
REM *********************************************************
REM 100MB�𒴂���t�@�C����Git��ɃR�~�b�g�o���Ȃ��B
REM Git LFS�͎g�������Ȃ��̂�Drive�ɕۑ������t�@�C����
REM �_�E�����[�h���ĉ𓀂��A�v���W�F�N�g�ɃR�s�[�����@���Ƃ�B
REM *********************************************************

REM .ini ��͂��s���o�b�`
set INI_ANALYZER_BAT=../bat/GetIni.bat

REM .zip ��/���k�������o�b�`
set ZIP_BAT=../bat/zip.bat

REM �ǂݍ���ini�t�@�C��
set READ_INI=./Setup.ini

REM �𓀑Ώۂ̃t�@�C���p�X
::set READ_ZIP_PATH=./ProjectSetting.zip

REM �𓀌��t�@�C���̍폜 true:!=0
set REPLACEMENT_DELETE=1

REM �f�o�b�O�o�̓t���O "true:!=0"
set DEBUG_FLAG=0

REM zip�t�@�C���̉𓀐�
set TEMP_FILE=.temp

REM ------
REM INI
REM ------

REM CLIENT Unity /Asset/ �܂ł̑��΃p�X
call %INI_ANALYZER_BAT% :READ_INI_VAL "COMMON_PATH" "UNITY_PROJECT_PATH" CL_ROOT_PATH %READ_INI%

REM SERVER .csproj �܂ł̑��΃p�X
call %INI_ANALYZER_BAT% :READ_INI_VAL "COMMON_PATH" "CSHARP_SERVER_PATH" SV_ROOT_PATH %READ_INI%

REM �𓀌��̐ezip(GDrive�ォ�痎�Ƃ��Ă���ProjectSetting�p��.zip)
call %INI_ANALYZER_BAT% :READ_INI_VAL "COMMON_PATH" "ROOT_ZIP" READ_ZIP_PATH %READ_INI%

REM �𓀂���t�@�C��������������𓀂������Ȃ��B
if not exist "%READ_ZIP_PATH%" (
    echo �p�X:"%READ_ZIP_PATH%" �����݂��܂���B
    pause
    exit /b
)

REM �𓀐�̃t�@�C�������ɑ��݂��Ă�����𓀂ł��Ȃ��B
if exist "%TEMP_FILE%" (
    echo �p�X:"%TEMP_FILE%" �����ɑ��݂��Ă��邽�߉𓀂ł��܂���B
    pause
    exit /b
)

REM �x�����ϐ�����
setlocal enabledelayedexpansion

REM �ezip��
call "%ZIP_BAT%" :UNZIP "%TEMP_FILE%" "%READ_ZIP_PATH%"

REM �ǂݍ��񂾃Z�N�V����
set SECTION=

REM �q.zip���
for /f "tokens=1 delims==" %%k in ( %READ_INI% ) do (

  set KEY=%%k
  set FORMAT=!KEY:~0,1!!KEY:~-1,1!
  if "!FORMAT!"=="[]" set SECTION=!KEY!
  
  REM DEBUG�o��
  if not %DEBUG_FLAG%==0 echo ���ǂݍ���ł���l��
  if not %DEBUG_FLAG%==0 echo �Z�N�V����:!SECTION!
  if not %DEBUG_FLAG%==0 echo �L�[:!KEY!
  if not %DEBUG_FLAG%==0 echo �t�H�[�}�b�g:!FORMAT!

  REM [CHILD_ZIP]�Z�N�V�����Ɏq.zip�������Ă���B
  REM ��FORMAT���肵�Ȃ��ƃZ�N�V�����ǂݎ������肳���B
  if "!SECTION!"=="[CHILD_ZIP]" if not "!FORMAT!"=="[]" (

      REM �𓀐�̃t�@�C�������擾
      call %INI_ANALYZER_BAT% :READ_INI_VAL "CHILD_ZIP" "!KEY!" READ_CHILD_ZIP %READ_INI%
      
      REM �𓀐�̃t�@�C���� ("�𓀃t�@�C��/�L�[")
      set DECOMPRESSION_PATH="%TEMP_FILE%/!READ_CHILD_ZIP!"
      
      REM �t�@�C�����������������B
      if exist "!DECOMPRESSION_PATH!" (
        
        REM �ۑ���
        set SAVE_CHILD_PATH="!TEMP_FILE!/!KEY!"

        if not %DEBUG_FLAG%==0 echo �𓀐�:!DECOMPRESSION_PATH!
        if not %DEBUG_FLAG%==0 echo �ۑ���:!SAVE_CHILD_PATH!
        
        REM �g���q�擾.
        call :GET_EXTENSION !DECOMPRESSION_PATH! FILE_EXTENSION

        REM �q.zip ��
        if "!FILE_EXTENSION!"==".zip" call "%ZIP_BAT%" :UNZIP  !SAVE_CHILD_PATH! !DECOMPRESSION_PATH!

        REM �W�J��̃R�s�[�Ώۃt�@�C��
        call :COPY !SAVE_CHILD_PATH! "!KEY!"
      )
  )
  if not %DEBUG_FLAG%==0 echo.
)
)

REM MEMO.
REM �𓀂����t�@�C���𕡐�
REM .nupkg�̒��g�Ŏg�������Ȃ̂�Tool���炢��Git�Ǘ�����Ă����Ȃ̂�Plugin�����B
REM -----
REM (���g) �q.zip�����Ŏ����肽�̂ŏȗ��B
REM -----

REM �t�H���_�폜
if not %REPLACEMENT_DELETE%==0 if exist "%TEMP_FILE%" rmdir %TEMP_FILE% /s /q

set PAUSE_FLAG=0
if not %DEBUG_FLAG%==0 set PAUSE_FLAG=1
if not %ERRORLEVEL%==0 set PAUSE_FLAG=1
if not %PAUSE_FLAG%==0 pause

exit /b

REM <�L�[��> ��n���ăt�@�C��/�t�H���_���R�s�[
REM CREAETED_DIR : �R�s�[���̃t�@�C������������Ă���f�B���N�g��
REM COPY_KEY : �L�[��
:COPY

REM ���x��IN ���b�Z�[�W
if not %DEBUG_FLAG%==0 echo COPY���x��

REM �R�s�[���̃t�@�C������������Ă���f�B���N�g��
set CREAETED_DIR=%~1

REM �L�[
set COPY_KEY=%~2

REM �R�s�[���̃t�@�C��/�t�H���_���擾
call %INI_ANALYZER_BAT% :READ_INI_VAL "CHILD_FILE" "!COPY_KEY!" RESLT_FILE %READ_INI%

REM �R�s�[��̃f�B���N�g��
call %INI_ANALYZER_BAT% :READ_INI_VAL "CHILD_COPY" "!COPY_KEY!" RESLT_DIR %READ_INI%

REM �����œn���ꂽ�f�B���N�g�� �{ �L�[����ǂݍ��񂾃R�s�[���̃t�@�C����
set COPY_PATH=!CREAETED_DIR!/!RESLT_FILE!

REM �𓀃t�@�C���̃p�X�w�肵�Ȃ��ƃ_���I(�e�f�B���N�g��)
if not %DEBUG_FLAG%==0 echo �R�s�[��:!COPY_PATH!
if not %DEBUG_FLAG%==0 echo �R�s�[��:!RESLT_DIR!


REM ����������������
if not "!RESLT_FILE!"=="NULL" if not "!RESLT_DIR!"=="NULL" if exist "!COPY_PATH!" if exist "!RESLT_DIR!" (

  REM �R�s�[
�@xcopy "!COPY_PATH!" "!RESLT_DIR!" /E /Y
  if not %DEBUG_FLAG%==0 echo COPY:"SUCCESS"
  if not %DEBUG_FLAG%==0 echo RESLT_DIR:"!RESLT_DIR!"
  if not %DEBUG_FLAG%==0 echo RESLT_FILE:"!COPY_PATH!"
) else (

  REM ���炩�̗v���ŃR�s�[�o���Ȃ�����
  if not %DEBUG_FLAG%==0 echo COPY:"FAILED"
  if not %DEBUG_FLAG%==0 echo RESLT_DIR:"!RESLT_DIR!"
  if not %DEBUG_FLAG%==0 echo RESLT_FILE:"!COPY_PATH!"
)

exit /b

REM �g���q�擾.
REM call :GET_EXTENSION <����p�X> <�i�[�ϐ�>
:GET_EXTENSION

set EXTENSION=%~x1
set %2=%EXTENSION%

exit /b

REM �x�����ϐ��I��
endlocal
