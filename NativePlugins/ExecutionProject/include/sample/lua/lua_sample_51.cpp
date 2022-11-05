#include "lua_sample_51.hpp"
#include <iostream>
#include<format>

#if LUA_VERSION_NUM == 501

lua_sample_51::lua_sample_51()
{
}

lua_sample_51::~lua_sample_51()
{
}

void lua_sample_51::stack_print(lua_State*L)
{
	const int num = lua_gettop(L);

	std::cout << "lua_State->stack print" << std::endl;

	if (num == 0) {
		std::cout << "No stack." << std::endl;
		return;
	}

	for (int i = num; i >= 1; i--) {
		/*
			@MEMO	formatはC++20の機能.
		*/
		std::cout << std::format("↑{0}(↓{1}):", i, -num + i - 1);

		int type = lua_type(L, i);
		std::string str;
		switch (type) {
		case LUA_TNIL:
			str = std::string("NIL");
			break;
		case LUA_TBOOLEAN:
			str = std::format("BOOLEAN {}", lua_toboolean(L, i) ? "true" : "false");
			break;
		case LUA_TLIGHTUSERDATA:
			str = std::string("LIGHTUSERDATA");
			break;
		case LUA_TNUMBER:
			str = std::format("NUMBER {}", lua_tonumber(L, i));
			break;
		case LUA_TSTRING:
			str = std::format("STRING {}", lua_tostring(L, i));
			break;
		case LUA_TTABLE:
			str = std::string("TABLE");
			break;
		case LUA_TFUNCTION:
			printf("\n");
			str = std::string("FUNCTION");
			break;
		case LUA_TUSERDATA:
			str = std::string("USERDATA");
			break;
		case LUA_TTHREAD:
			str = std::string("THREAD");
			break;
		}
		std::cout << str << std::endl;
	}
}

void lua_sample_51::get_lua_value_sample()
{
	lua_State* L = luaL_newstate();
	if (!load(L, "resources/lua/getvalue_sample.lua")) {
		return;
	}

	// getglobalメソッドを呼ぶことでスタックに積まれる.
	lua_getglobal(L, "Width");
	lua_getglobal(L, "Height");
	lua_getglobal(L, "Name");
	stack_print(L);

	auto width = static_cast<int>(lua_tonumber(L, 1));
	auto height = static_cast<int>(lua_tonumber(L, 2));
	auto name = static_cast<std::string>(lua_tostring(L, 3));

	std::cout << "load lua value to cpp." << std::endl;
	VARARGOUT(width);
	VARARGOUT(height);
	VARARGOUT(name);
}

bool lua_sample_51::load(lua_State* L, std::string path)
{
	if (luaL_dofile(L, path.c_str()))
	{
		std::cout << lua_tostring(L, lua_gettop(L)) << std::endl;
		lua_close(L);
		std::cout << "failed to load lua. path:" << path << std::endl;
		return false;
	}
	return true;
}

#endif