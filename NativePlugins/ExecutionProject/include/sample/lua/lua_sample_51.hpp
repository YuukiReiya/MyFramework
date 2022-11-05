//参考:http://marupeke296.com/LUA_main.html
#pragma once
#include <lua.hpp>
#include <string>
#include "edlpch.hpp"

#pragma region Lua ver_5.1
#if LUA_VERSION_NUM == 501
class lua_sample_51
{
public:
	lua_sample_51();
	~lua_sample_51();
	/*
		@brief	lua_Stateのスタックを表示.
	*/
	void stack_print(lua_State* L);

	/*
		@brief	Luaにあるグローバル値をC言語で取得するサンプル.
	*/
	void get_lua_value_sample();

private:
	lua_State* m_pState = nullptr;

	/*
		@brief	lua_StateにLuaファイルをロード.
	*/
	bool load(lua_State* L, std::string path);
};
#endif
#pragma endregion