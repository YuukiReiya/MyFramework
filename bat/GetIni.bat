rem https://jitenshatoryokou.com/prototype-2-shortcut-22211.html
rem https://gabekore.org/bat-read-ini

@echo off

REM �x�����ϐ����g�����߂ɂ���������Ă���
REM �x�����ϐ��i%HOGEHOGE%�ł͂Ȃ�!HOGEHOGE!�Ə����j�ɂ��Ă̓O�O���Ă�
setlocal enabledelayedexpansion


:READ_INI_VAL

REM ====================================================================
REM INI�t�@�C�����獀�ڂ�ǂݎ��Ԃ�
REM
REM   ���L�[���擾�ł��Ȃ��ꍇ�́A�擾�ϐ��ɁuERR�v��Ԃ�
REM
REM ====================================================================
REM 
REM ����bat����ȉ��̂悤�ȃR�[�����O�V�[�P���X�ŃR�[������鎖�����҂��Ă���
REM call GetIni.bat  :READ_INI_VAL  "SECTION��"  "KEY��"  GET_VAL  %INI_FILE_FULLPATH%
REM 
REM �����̐���
REM   %0        : (in ) ���̃t�@�C���̖��O         �i��F�uGetIni.bat�v�j
REM   %1        : (in ) ���̃T�u���[�`����         �i��F�u:READ_INI_VAL�v�j
REM   %2 or %~2 : (in ) �Z�N�V������               �i��F�uSECTION���v�j
REM   %3 or %~3 : (in ) �L�[��                     �i��F�uKEY���v�j
REM   %4        : (out) �擾�f�[�^���Z�b�g����ϐ� �i��F�uGET_VAL�v�j
REM   %5        : (in ) ini�t�@�C���̃t���p�X      �i��F�uc:\hoge\foo\fuga.ini�v�j
REM 
REM ���w�肵��SECTION����KEY�����擾�ł��Ȃ��ꍇ�͎擾�f�[�^���Z�b�g����ϐ��ɁuERR�v���Z�b�g����
REM 
REM ====================================================================
REM 
REM for���߂ɂ��Ă̕⑫
REM /F �̎��Ɏg�p�ł���I�v�V����
REM 
REM eol=c       : �s���R�����g�J�n������c�ɂ���B���̕����ȍ~�͒��߂Ƃ��Ė��������
REM skip=n      : �t�@�C���̐擪����n�s�𖳎�����
REM delims=xxx  : �f���~�^��xxx�Ƃ���B���������̎w�肪�\�B�f�t�H���g�̓^�u�ƃX�y�[�X
REM tokens=x,y  : �ǂ̃p�[�g��ϐ��ɑ�����ăR�}���h���ɓn�������w�肷��B
REM usebackq    : �o�b�N�N�H�[�g�i�g`�h�A�t���p���j�ň͂܂ꂽ��������R�}���h�Ƃ��Ď��s����
REM 
REM http://www.atmarkit.co.jp/ait/articles/0106/23/news004_2.html
REM 
REM ====================================================================

REM ------------------------------------------------
REM �t�@�C�����P�s���ǂݏo���āA����
REM ------------------------------------------------
set GETINIVALUE=
set SECTIONNAME=

REM �u%%x�v�ϐ��ɃL�[�����A�u%%y�i�������ō����j�v�ϐ��ɃL�[�̒l�����鎖�����҂��Ă���
REM 
REM �udelims==�v�́A�udelims = (�C�R�[���L��)�v�Ɠǂ߂Ηǂ��B�v�́u=�v����؂蕶���Ƃ��Ă���
REM INI�t�@�C���̃L�[�́uKEYNAME=5�v�݂����Ȍ`�ɂȂ��Ă���̂ŁAKEYNAME��%%x�ցA5��%%y�փZ�b�g���邱�Ƃ��Ӗ����Ă���
REM 
REM �utokens=1,2�v��delims�ŋ�؂���1�ڂ�%%x�ցA2�ڂ�%%y�ւƂ����Ӗ�
REM 
REM %5��INI�t�@�C���̃t���p�X�ŁA����for����INI�t�@�C�������s���擾���Ă���
for /F "eol=; delims== tokens=1,2" %%x in (%5) do (

   REM ------------------------------------------------------
   REM �����̒��o�ɂ���
   REM 
   REM %V%	�ϐ�V�̒l�S��
   REM %V:~m%	m�����ڂ���A�Ō�܂�
   REM %V:~m,n%	m�����ڂ���An������
   REM %V:~m,-n%	m�����ڂ���A�Ō��n������������������
   REM %V:~-m%	��납��m�����ڂ���A�Ō�܂�
   REM %V:~-m,n%	��납��m�����ڂ���An������
   REM %V:~-m,-n%	��납��m�����ڂ���A�Ō��n������������������
   REM %V:c1=c2%	����c1�𕶎�c2�ɒu������B���ꂼ�ꕡ���̕������w�肷�邱�Ƃ��\
   REM 
   REM https://www.upken.jp/kb/kZwpzAqblKfZDjtMXuWuwioeExKNdE.html
   REM ------------------------------------------------------

   REM �擾�����s���L�[�̍s�ł���΁A�L�[��������͂�
   set KEYNAME=%%x

   REM !KEYNAME:~0,1!   : �擪�ꕶ�����擾���擾�����s���Z�N�V�����̍s�ł���΁A[
   REM !KEYNAME:~-1,1!  : �ŏI�ꕶ�����擾���擾�����s���Z�N�V�����̍s�ł���΁A]
   set P=!KEYNAME:~0,1!!KEYNAME:~-1,1!

   REM []�̒��̕������擾�i�Z�N�V�����������҂��Ă���j
   set S=!KEYNAME:~1,-1!

   REM "[]"�̓Z�N�V���������Ӗ�����
   if "!P!"=="[]" set SECTIONNAME=!S!

   REM �Z�N�V�������ƃL�[���������Ŏw�肳�ꂽ���̂ƈ�v�����OK�I
   REM �ŁA�����I��
   if "!SECTIONNAME!"=="%~2" if "!KEYNAME!"=="%~3" (
      set GETINIVALUE=%%y
      goto GET_INI_EXIT
   )
)
REM ------------------------------------------------
REM ���ڂ�������Ȃ��ꍇ�́A�uNULL�v��ϐ��֓���
REM ERR�ł͂Ȃ������Z�b�g�������Ȃ���΁A
REM �uset GETINIVALUE=�v�Ə����Ηǂ�
REM ------------------------------------------------
set GETINIVALUE=NULL



REM setlocal�`endlocal�ԁi�ȉ��Alocal���j�͒ʏ�̃v���O��������Ō����Ƃ���̊֐��̂悤�Ȃ���
REM local�����o�鎞�ɂ�local���̊��ϐ��͏����Ă��܂�
REM �uGETINIVALUE=%GETINIVALUE%�v�́Alocal����%GETINIVALUE%���A
REM �V����GETINIVALUE�փZ�b�g���Ă���ƍl����Ηǂ�
REM 
REM :GET_INI_EXIT���x���̉���endlocal�������̂͋C�����������A���s�^�C�~���O�I�ɂ�����������

:GET_INI_EXIT
endlocal && set GETINIVALUE=%GETINIVALUE%

REM ------------------------------------------------
REM �擾�ϐ����ɃZ�b�g
REM �R�[�����ł��̒l������
REM ------------------------------------------------

set %4=%GETINIVALUE%


:EOF
REM /b �I�v�V�������O���Ƃ���bat���R�[�����Ă���bat�܂Ŏ���
exit /b