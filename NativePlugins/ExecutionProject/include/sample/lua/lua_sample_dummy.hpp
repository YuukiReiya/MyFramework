#pragma once
#include "Ilua_sample.hpp"
#include <iostream>

class lua_sample_dummy :public Ilua_sample
{
public:
	lua_sample_dummy() {};
	~lua_sample_dummy() {};

#define __dummy_func_print__ std::cout << "dummy_" << __func__ << std::endl;
	void stack_print(lua_State*)override { __dummy_func_print__ };
	void get_lua_value_sample()override { __dummy_func_print__ };
	void do_lua_method_sample()override { __dummy_func_print__ };
	void do_lua_coroutine_sample()override { __dummy_func_print__ };
	void do_lua_table_sample()override { __dummy_func_print__ };
	void do_cpp_method_sample()override { __dummy_func_print__ };
private:

};
