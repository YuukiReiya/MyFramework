#pragma once
#include <lua.hpp>

#pragma region 変数名:値の出力
#if DEBUG||_DEBUG
#define VARARGOUT(var) std::cout<<#var<<":"<<var<<std::endl;
#else
#define VARARGOUT(var)
#endif // DEBUG||_DEBUG
#pragma endregion

__interface Ilua_sample
{
	/*
		@brief	lua_Stateのスタックを表示.
	*/
	void stack_print(lua_State* L);
	/*
		@brief	Luaにあるグローバル値をC言語で取得するサンプル.
	*/
	void get_lua_value_sample();
	/*
		@brief	Luaの関数をC＋＋から呼ぶサンプル.
	*/
	void do_lua_method_sample();
	/*
		@brief	Luaで呼び出すコルーチンのサンプル.
	*/
	void do_lua_coroutine_sample();
	/*
		@brief	LuaにおけるTable挙動のサンプル.
	*/
	void do_lua_table_sample();
	/*
		@brief	C++の関数をLuaから呼び出す.
	*/
	void do_cpp_method_sample();
};