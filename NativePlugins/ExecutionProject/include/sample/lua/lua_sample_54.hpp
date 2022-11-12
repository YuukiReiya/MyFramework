#pragma once
#include"Ilua_sample.hpp"
#pragma region Lua ver_5.4
#if LUA_VERSION_NUM == 504
#include <string>
#include "edlpch.hpp"

class lua_sample_54 :
    public Ilua_sample
{
public:
    lua_sample_54() {}
    ~lua_sample_54() {}

	void stack_print(lua_State* L)override;
	void get_lua_value_sample()override;
	void do_lua_method_sample()override;
	void do_lua_coroutine_sample()override;
	void do_lua_table_sample()override;
	void do_cpp_method_sample()override;

private:
	/*
	@brief	lua_StateにLuaファイルをロード.
	*/
	bool load(lua_State* L, std::string path);

};

#endif
#pragma endregion