#include "edlpch.hpp"
#include<lua.hpp>

__cdecl lua_wrapper::lua_wrapper()
{
	state = nullptr;
}

__cdecl lua_wrapper::~lua_wrapper()
{
}

void __cdecl lua_wrapper::initialize()
{
	state = luaL_newstate();
}

void __cdecl lua_wrapper::finalize()
{
	lua_close(state);
	state = nullptr;
}
