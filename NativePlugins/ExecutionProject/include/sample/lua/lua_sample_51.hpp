//参考:http://marupeke296.com/LUA_main.html
#pragma once
#include "Ilua_sample.hpp"
#pragma region Lua ver_5.1
#if LUA_VERSION_NUM == 501
#include <string>
#include "edlpch.hpp"

class lua_sample_51
	:public Ilua_sample
{
public:
	lua_sample_51();
	~lua_sample_51();
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

/*
	@brief	Lua側でC＋＋で定義した関数を呼び出すサンプル用の関数.
	@detail	戻り値無し、引数無し
*/
int PrintCpp(lua_State* L);
/*
	@brief	Lua側でC＋＋で定義した関数を呼び出すサンプル用の関数.
	@detail 戻り値:float型1つ、引数:(int)a,(int)b,(float)t
*/
int Linear(lua_State* L);

#endif
#pragma endregion