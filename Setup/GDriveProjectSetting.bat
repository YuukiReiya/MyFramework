@echo off
REM **************************************************************
REM GDrive����v���W�F�N�g�ݒ�p�̃t�@�C���𗎂Ƃ���
REM �Y���t�@�C���ɃR�s�[���Ă���B
REM �t�@�C���ǂݍ��݂̊֌W��p�X�w�肵�Ď��s���邱�Ƃ͏o���Ȃ����߁A
REM �J�����g�f�B���N�g�����ړ����Ď��s����B
REM ��Unity���N�����Ă���Ƌ��L�t�@�C����xcopy�̃A�N�Z�X�����Ƃꂸ
REM �G���[�ɂȂ邽�ߕK�������グ��O�A�������͕��Ă�����s���邱�ƁB
REM **************************************************************

REM ���s�t�@�C���̏ꏊ�ɔ��
cd Exe/

REM GDrive����v���W�F�N�g�𗎂Ƃ��Ă���
call GDrive_File_Download.exe

REM �o�b�`�̂���t�H���_�ɖ߂�
cd ../

REM ���Ƃ��Ă����t�@�C����W�J���A�Y���t�H���_�֕���
call ProjectSetting.bat

set ROOT_ZIP=ProjectSetting.zip

REM ���Ƃ��Ă����t�@�C�����폜����
if exist "%ROOT_ZIP%" del "%ROOT_ZIP%" /s /q