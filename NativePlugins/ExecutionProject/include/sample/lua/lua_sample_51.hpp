//�Q�l:http://marupeke296.com/LUA_main.html
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
		@brief	lua_State�̃X�^�b�N��\��.
	*/
	void stack_print(lua_State* L);

	/*
		@brief	Lua�ɂ���O���[�o���l��C����Ŏ擾����T���v��.
	*/
	void get_lua_value_sample();

private:
	lua_State* m_pState = nullptr;

	/*
		@brief	lua_State��Lua�t�@�C�������[�h.
	*/
	bool load(lua_State* L, std::string path);
};
#endif
#pragma endregion