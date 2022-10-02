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

#define Lua51_
#define Lua54

// �O���錾
void stack_print(lua_State*);

/*!
	@brief �G���g���[�|�C���g
	@note  CLI������s�����ۂɈ�����n����悤�ɗp�ӂ������Ƃ��B
	@param[in]:�����̌�
	@param[in]:����������
	@return:����I��=�O , �ُ�I��=!�O(TODO:�G���[�R�[�h��݂��A�N���X����������)
*/
int main(int argNum, const char* argments) 
{
#pragma region ���������[�N
#if defined DEBUG||_DEBUG
	// ���������[�N.
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif // DEBUG||_DEBUG
#pragma endregion

#ifdef Lua51
#pragma region ����
#if false
	lua_State* L = luaL_newstate();
	lua_close(L);
#endif
#pragma endregion

#pragma region �R���[�`��
#if true

	lua_State* L = luaL_newstate();

	// �R���[�`�����g����悤�Ƀ��C�u������Lua�X�e�[�g�ɐݒ肷��
	luaopen_base(L);

	if (luaL_dofile(L, "resources/lua/test.lua")) {
		cout << lua_tostring(L, lua_gettop(L)) << endl;
		lua_close(L);
		::system("pause");
	}

	// �X���b�h�쐬(�R���[�`��).
	auto co = lua_newthread(L);

	lua_getglobal(co, "step");

	cout << "wait getchar();" << endl;
	while (lua_resume(co,0))
	{
		stack_print(co);
		getchar();
	}

#endif
#pragma endregion

#elif defined Lua54

#pragma region ����
#if false
	lua_State* L = luaL_newstate();
	lua_close(L);
#endif
#pragma endregion

#pragma region �R���[�`��
#if true
	lua_State* L = luaL_newstate();

	// �R���[�`�����g����悤�Ƀ��C�u������Lua�X�e�[�g�ɐݒ肷��
	luaopen_base(L);

	//if (luaL_dofile(L, "resources/lua/test.lua")) {
	//	::cout << lua_tostring(L, lua_gettop(L)) << endl;
	//	lua_close(L);
	//	::system("pause");
	//}

	if (luaL_dofile(L, "resources/lua/Coroutine.lua")) {
		::cout << lua_tostring(L, lua_gettop(L)) << endl;
		lua_close(L);
		:: cout << "FAILED" << endl;
		::system("pause");
	}


	// �X���b�h�쐬(�R���[�`��).
	auto co = lua_newthread(L);

	auto getglobal=lua_getglobal(co, "step");
	int* res = new int();

	::cout << "getglobal=" << getglobal << endl;
	

	stack_print(L);

	::cout << "wait getchar();" << endl;
	while (lua_resume(co, co, 0, res) == LUA_OK)
	{
		stack_print(co);
		getchar();
	}
	::cout << "end while lua_resume print -> co" << endl;
	stack_print(co);
	::cout << "end while lua_resume print -> L" << endl;
	stack_print(L);
	delete(res);
#endif
#pragma endregion


#else
	


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
#if true

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
		while (lua_resume(co,0))
		{

		}
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
#endif // Lua51

	//�Ƃ肠������
	::system("pause");
	return SUCCESS;
}

void stack_print(lua_State* state) {
	const int num = lua_gettop(state);

	::cout << "lua_State->stack print" << endl;

	if (num == 0) {
		::cout << "No stack." << endl;
		return;
	}

	for (int i = num; i >= 1; i--) {
		printf("%03d(%04d): ", i, -num + i - 1);
		int type = lua_type(state, i);
		switch (type) {
		case LUA_TNIL:
			printf("NIL\n");
			break;
		case LUA_TBOOLEAN:
			printf("BOOLEAN %s\n", lua_toboolean(state, i) ? "true" : "false");
			break;
		case LUA_TLIGHTUSERDATA:
			printf("LIGHTUSERDATA\n");
			break;
		case LUA_TNUMBER:
			printf("NUMBER %f\n", lua_tonumber(state, i));
			break;
		case LUA_TSTRING:
			printf("STRING %s\n", lua_tostring(state, i));
			break;
		case LUA_TTABLE:
			printf("TABLE\n");
			break;
		case LUA_TFUNCTION:
			printf("FUNCTION\n");
			break;
		case LUA_TUSERDATA:
			printf("USERDATA\n");
			break;
		case LUA_TTHREAD:
			printf("THREAD\n");
			break;
		}
	}

	::cout << endl;
}