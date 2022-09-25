/*!
	@file			EntryPoint.cpp
	@brief		ExternalDynamicLinkLibrary�v���W�F�N�g�Ő�������*.dll�̎��s�e�X�g���B��ɃG���[�`�F�b�N�ȂǁB
	@detail		�\�߃v���W�F�N�g�Ƀp�X��ʂ����̂ƃr���h�C�x���g�ňˑ��t�@�C����"ExternalDynamicLinkLibrary"���R�s�[���鏈�������Ă���B
					���ˑ��v���W�F�N�g����������A���O��ύX�����ꍇ�̓p�X�̒ʂ������ƃr���h�C�x���g�̕ҏW���K�v�ɂȂ�
	@todo		�e�X�g�p�̂��̃v���W�F�N�g�ȊO�͊�{�I��dll�̃v���W�F�N�g�ɂȂ�͂��Ȃ̂Ńr���h���Ɉˑ��֌W��ǉ����Ă����@�\(�@�\)���~�����B
					��:XInput.dll�̃p�X��ʂ����ʂ̃v���W�F�N�g��p�ӂ�����ʓ|�ȃp�X�ǉ������ƃr���h�C�x���g�o�^�����ŏ���ɂ���Ă����̂��x�X�g�B
*/
#pragma once
#include <iostream>
#include <vector>
#include<lua.hpp>
#include "edlpch.hpp"
#include "../../../ExternalDynamicLinkLibrary/include/Sample/Sample.hpp"
#include "../../../ExternalDynamicLinkLibrary/include/lua_wrapper.hpp"
#define SUCCESS 0
#define FAILED -1

#pragma region ���������[�N����
#ifndef _CRTDBG_MAP_ALLOC
#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#define	new	new(_NORMAL_BLOCK, __FILE__, __LINE__)
#endif // !_CRTDBG_MAP_ALLOC
#pragma endregion

#if DEBUG||_DEBUG
#define VARARGOUT(var) std::cout<<#var<<":"<<var<<std::endl;
#else
#define VARARGOUT(var)
#endif // DEBUG||_DEBUG

using namespace std;

/*!
	@brief �G���g���[�|�C���g
	@note  CLI������s�����ۂɈ�����n����悤�ɗp�ӂ������Ƃ��B
	@param[in]:�����̌�
	@param[in]:����������
	@return:����I��=�O , �ُ�I��=!�O(TODO:�G���[�R�[�h��݂��A�N���X����������)
*/
int main(int argNum, const char* argments) 
{
#if defined DEBUG||_DEBUG
	// ���������[�N.
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif // DEBUG||_DEBUG

	lua_wrapper* lua = new lua_wrapper();
	lua->initialize();

#pragma region lua_State�̃X�^�b�N�Ƀf�[�^��ς�
	//lua_pushboolean(lua->get_state(), 1);
	//lua_pushnumber(lua->get_state(), 100.0);
	//lua_pushstring(lua->get_state(), "Marupeke");
#pragma endregion

	

	//�t�@�C���ǂݍ���
	if (luaL_dofile(lua->get_state(), "resources/lua/test.lua")) {
		::printf("%s\n", lua_tostring(lua->get_state(), lua_gettop(lua->get_state())));
		lua_close(lua->get_state());
		::system("pause");

		return FAILED;
	}
#pragma region �O����lua�t�@�C������ϐ��Ǎ�
#if true
#define SAMPLE_EXTERNAL_LUA_READ
	//�ϐ��ǂݍ���
	lua_getglobal(lua->get_state(), "wndWidth");
	lua_getglobal(lua->get_state(), "wndHeight");
	lua_getglobal(lua->get_state(), "wndName");

	lua->stack_print();
#endif
#pragma endregion

#pragma region �O����lua�t�@�C������֐����Ă�
#ifndef SAMPLE_EXTERNAL_LUA_READ
#if false


	// Lua�X�e�[�g���ɂ���"calc�֐�"���w��
	lua_getglobal(lua->get_state(), "calc");

	// ������ݒ�
	::printf("push\n");
	lua_pushnumber(lua->get_state(), 10);
	lua_pushnumber(lua->get_state(), 20);

	lua->stack_print();

	// �֐����s
	// ��1 �X�^�b�N�ɐς܂ꂽ���̂�lua_pcall���Ă񂾂Ƃ��ɃX�^�b�N������o����A����ɖ߂�l���X�^�b�N�ɐς܂��
	// ��2 �X�^�b�N�ɐς܂ꂽ�S�Ẵf�[�^���|�b�v�����킯�ł͂Ȃ��A
	::printf("�֐��Ăяo��\n");
	
	/*
	* @fn			lua_pcall
	*					Lua�t�@�C���Œ�`�����֐���C���ꑤ����Ăяo��.
	* @brief		Lua�֐��̌Ăяo��.
	* @param1 (L) : lua_State
	* @param2 (n) : �����̐�
	* @param3 (r) : �߂�l�̐�
	* @param4 (f) : �Ǝ��̃G���[���b�Z�[�W����舵�����Ɏg���܂����A�f�t�H���g�ŗǂ��ꍇ��0��n���܂��B
	* @return 0 or 1 : �G���[�n���h��
	* @sa			http://marupeke296.com/LUA_No2_Begin.html
	*/
	if (lua_pcall(lua->get_state(), 2, 4, 0)) {
		::printf("%s\n", lua_tostring(lua->get_state(), lua_gettop(lua->get_state())));
		lua_close(lua->get_state());
		return FAILED;
	}

	::printf("�߂�l\n");
	// ��������߂�l���擾
	float add_Res = (float)lua_tonumber(lua->get_state(), 1);
	float sub_Res = (float)lua_tonumber(lua->get_state(), 2);
	float mult_Res = (float)lua_tonumber(lua->get_state(), 3);
	float dev_Res = (float)lua_tonumber(lua->get_state(), 4);

	float results[]= {add_Res, sub_Res, mult_Res, dev_Res};
	for (auto& r : results) { VARARGOUT(r); }

	lua->stack_print();
#endif
#endif // !SAMPLE_EXTERNAL_LUA_READ
#pragma endregion

#pragma region �R���[�`��
	/*
	* @sa http://marupeke296.com/LUA_No3_Coroutine.html
	*/
#if false

	lua_State* L = luaL_newstate();

	// �R���[�`�����g����悤�Ƀ��C�u������Lua�X�e�[�g�ɐݒ肷��
	luaopen_base(L);

	// �t�@�C�����J��
	if (luaL_dofile(L, "resources/lua/Coroutine.lua"))
	{
		::printf("%s\n", lua_tostring(L, lua_gettop(L)));
		lua_close(L);
		return FAILED;
	}
	// �R���[�`��(�X���b�h)���쐬
	lua_State* co = lua_newthread(L);

	// �R���[�`���X�e�[�g���ɂ���gstep�֐��h���w��
	lua_getglobal(co, "step");
	int* r = new int;

	try
	{
		while (lua_resume(L, co, 0, r))
		{

		}
	}
	catch (const std::exception&e)
	{
		cout<<"��O" << endl << e.what() << endl;
	}

#endif
#pragma endregion


	// �X�^�b�N�폜.
	//int num = lua_gettop(lua->get_state());
	//lua_pop(lua->get_state(), num);
	delete (lua);

	//�Ƃ肠������
	::system("pause");
	return SUCCESS;
}
