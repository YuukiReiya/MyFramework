// �v���R���p�C���ς݃w�b�_�[.
#pragma once
#include <stdlib.h>
#include <iostream>
#include <Windows.h>

#pragma region ���������[�N����
#if defined DEBUG || defined _DEBUG
#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#define	new	new(_NORMAL_BLOCK, __FILE__, __LINE__)
#endif
#pragma endregion

#pragma region DLL(P/Invoke)
#define DLL_EXPORT  extern "C" __declspec(dllexport)
#define UNITY_API __stdcall

/*
	@sample
	DLL_EXPORT <�^> UNITY_API <�֐���>( ���� ) { ��` }
*/

#pragma endregion


#pragma region �ϐ���:�l�̏o��
#if DEBUG||_DEBUG
#define VARARGOUT(var) std::cout<<#var<<":"<<var<<std::endl;
#else
#define VARARGOUT(var)
#endif // DEBUG||_DEBUG
#pragma endregion

/*
	@brief	�_�~�[����
	@detail	P/Invoke�p�̃_�~�[����.
				"dllexport"�̒�`���ꂽ�֐�������Ȃ���".lib"���o�����Ƀ����J�[�G���[���o��B
				�֐��̎������S�ۂ����΍폜���Ă��܂��Ă���.
*/
DLL_EXPORT inline void UNITY_API dummy() {}

/*
	@brief	Win32�̃G���[�R�[�h�����b�Z�[�W�Ƃ��ă_�C�A���O�ɏo��
*/
DLL_EXPORT void UNITY_API MsgBoxWin32(DWORD errorcode);
