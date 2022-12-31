#pragma once
#include <lua.hpp>

#pragma region �ϐ���:�l�̏o��
#if DEBUG||_DEBUG
#define VARARGOUT(var) std::cout<<#var<<":"<<var<<std::endl;
#else
#define VARARGOUT(var)
#endif // DEBUG||_DEBUG
#pragma endregion

__interface Ilua_sample
{
	/*
		@brief	lua_State�̃X�^�b�N��\��.
	*/
	void stack_print(lua_State* L);
	/*
		@brief	Lua�ɂ���O���[�o���l��C����Ŏ擾����T���v��.
	*/
	void get_lua_value_sample();
	/*
		@brief	Lua�̊֐���C�{�{����ĂԃT���v��.
	*/
	void do_lua_method_sample();
	/*
		@brief	Lua�ŌĂяo���R���[�`���̃T���v��.
	*/
	void do_lua_coroutine_sample();
	/*
		@brief	Lua�ɂ�����Table�����̃T���v��.
	*/
	void do_lua_table_sample();
	/*
		@brief	C++�̊֐���Lua����Ăяo��.
	*/
	void do_cpp_method_sample();
};