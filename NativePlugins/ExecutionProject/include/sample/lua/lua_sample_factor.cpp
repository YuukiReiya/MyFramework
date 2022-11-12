#include "lua_sample_factor.hpp"
#include "lua_sample_51.hpp"
#include "lua_sample_54.hpp"
#include "lua_sample_dummy.hpp"

Ilua_sample* lua_sample_factor::create()
{
	auto lua_sample =
#if LUA_VERSION_NUM == 501
		// Lua5.1
		new lua_sample_51()
#elif LUA_VERSION_NUM==504
		// Lua5.4
		new lua_sample_54()
#else
		// É_É~Å[.
		new lua_sample_dummy()
#endif
		;
	return lua_sample;
}
