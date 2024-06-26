#include "lua_sample_51.hpp"
#include <iostream>
#include<format>

#if LUA_VERSION_NUM == 501

int PrintCpp(lua_State* L) 
{
	std::cout << "C++で定義した関数" << std::endl;
	return 0;
}

int Linear(lua_State* L)
{
	float a = static_cast<float>(lua_tonumber(L, 1));
	float b = static_cast<float>(lua_tonumber(L, 2));
	float t = static_cast<float>(lua_tonumber(L, 3));

	// 処理.
	float result = (1.0f - t) * a + t * b;

	/*
		スタック削除.
	*/
	lua_pop(L, lua_gettop(L));

	/*
		スタックに戻り値を積む.
	*/
	lua_pushnumber(L, result);

	// 戻り値の数.
	return 1;
}

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

	lua_close(L);
}

void lua_sample_51::do_lua_method_sample()
{
	lua_State* L = luaL_newstate();
	if (!load(L, "resources/lua/domethod_sample.lua")) {
		return;
	}

	// スタックに呼び出すメソッドを登録しとく.
	lua_getglobal(L, "calc");

	// 引数指定.
	lua_pushnumber(L, 10);
	lua_pushnumber(L, 8);

	// スタック表示.
	stack_print(L);

	// 関数実行.
	if (lua_pcall(L, 2, 4, 0)) 
	{
		std::cout << std::format("{}", lua_gettop(L)) << std::endl;
		lua_close(L);
		return;
	}

	stack_print(L);

	// 戻り値を取得.
	int add = static_cast<int>(lua_tonumber(L, 1));
	int sub = static_cast<int>(lua_tonumber(L, 2));
	int mul = static_cast<int>(lua_tonumber(L, 3));
	float dev = static_cast<float>(lua_tonumber(L, 4));

	std::cout << "calc lua function to cpp." << std::endl;
	VARARGOUT(add);
	VARARGOUT(sub);
	VARARGOUT(mul);
	VARARGOUT(dev);

	lua_close(L);
}

void lua_sample_51::do_lua_coroutine_sample()
{
	auto L = luaL_newstate();
	luaopen_base(L);
	if (!load(L, "resources/lua/docoroutine_sample.lua")) {
		return;
	}

	// コルーチン(スレッド)作成.
	auto co = lua_newthread(L);

	// スタックにコルーチン用のメソッドを登録.
	lua_getglobal(co, "step");

	char c;
	while (lua_resume(co, 0))
	{
		stack_print(co);
		c = getchar();
	}

	lua_close(L);
}

void lua_sample_51::do_lua_table_sample()
{
	lua_State* L = luaL_newstate();
	luaopen_base(L);
	if (!load(L, "resources/lua/dotable_sample.lua")) {
		return;
	}

	lua_close(L);
}

void lua_sample_51::do_cpp_method_sample()
{
	auto L = luaL_newstate();

	/*
		@brief		luaL_Stateに関数を登録.
		@param1:	lua_Stateのポインタ.
		@param2:	Lua側への登録名.
		@param3:	登録する関数ポインタ.
		@hack:		メンバ関数として登録する際の構文が分からなかったのでグローバル関数として用意したものをバインド.
	*/
	//lua_register(L, "func_cpp", &PrintCpp);

	// ↑でバインドした関数を内部的に使っている関数をスタックに登録.
	//lua_getglobal(L, "print_noarg");


	/*
		@brief	線形補完用のメソッドを登録.
		@detail	戻り値:float型一つ、引数:float型3つ
	*/
	lua_register(L, "Linear", &Linear);

	luaopen_base(L);
	if (!load(L, "resources/lua/domethod_cpp_sample.lua")) {
		return;
	}

	lua_close(L);
}

bool lua_sample_51::load(lua_State* L, std::string path)
{
	if (luaL_dofile(L, path.c_str()))
	{
		std::cout << lua_tostring(L, lua_gettop(L)) << std::endl;
		//stack_print(L);
		lua_close(L);
		std::cout << "failed to load lua. path:" << path << std::endl;
		return false;
	}
	return true;
}

#endif