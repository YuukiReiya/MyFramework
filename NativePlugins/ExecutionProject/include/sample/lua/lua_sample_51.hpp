//�Q�l:http://marupeke296.com/LUA_main.html
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
		@brief	lua_State��Lua�t�@�C�������[�h.
	*/
	bool load(lua_State* L, std::string path);

};

/*
	@brief	Lua����C�{�{�Œ�`�����֐����Ăяo���T���v���p�̊֐�.
	@detail	�߂�l�����A��������
*/
int PrintCpp(lua_State* L);
/*
	@brief	Lua����C�{�{�Œ�`�����֐����Ăяo���T���v���p�̊֐�.
	@detail �߂�l:float�^1�A����:(int)a,(int)b,(float)t
*/
int Linear(lua_State* L);

#endif
#pragma endregion