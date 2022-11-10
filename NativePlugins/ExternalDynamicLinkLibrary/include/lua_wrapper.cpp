#include "edlpch.hpp"
#include<lua.hpp>

// WIN32APPの場合はprintfがダメそうなのでOutputDebugStringAを使うらしい。
// ひとまずServer(Linux),Client(x64)なので考えていない.

using namespace std;

CALL lua_wrapper::lua_wrapper()
{
	state = nullptr;
    initialize();
}

CALL lua_wrapper::~lua_wrapper()
{
    finalize();
}

void CALL lua_wrapper::initialize()
{
	state = luaL_newstate();
}

void CALL lua_wrapper::finalize()
{
    // スタックに積まれたデータをポップして削除.
    lua_pop(state, lua_gettop(state));
	lua_close(state);
	state = nullptr;
}

void CALL lua_wrapper::stack_print(lua_wrapper* wrapper)
{
    const int num = lua_gettop(wrapper->state);

    cout << "lua_State->stack print" << endl;

    if (num == 0) {
        cout << "No stack." << endl;
        return;
    }

    for (int i = num; i >= 1; i--) {
        printf("%03d(%04d): ", i, -num + i - 1);
        int type = lua_type(wrapper->state, i);
        switch (type) {
        case LUA_TNIL:
            printf("NIL\n");
            break;
        case LUA_TBOOLEAN:
            printf("BOOLEAN %s\n", lua_toboolean(wrapper->state, i) ? "true" : "false");
            break;
        case LUA_TLIGHTUSERDATA:
            printf("LIGHTUSERDATA\n");
            break;
        case LUA_TNUMBER:
            printf("NUMBER %f\n", lua_tonumber(wrapper->state, i));
            break;
        case LUA_TSTRING:
            printf("STRING %s\n", lua_tostring(wrapper->state, i));
            break;
        case LUA_TTABLE:
            printf("TABLE\n");
            break;
        case LUA_TFUNCTION:
            printf("FUNCTION\n");
            break;
        case LUA_TUSERDATA:
            printf("USERDATA\n");
            break;
        case LUA_TTHREAD:
            printf("THREAD\n");
            break;
        }
    }

    cout << endl;
}

void CALL lua_wrapper::stack_print()
{
    stack_print(this);
}

void CALL lua_wrapper::push()
{
    lua_pushnil(state);
    //return void CALL();
}

void CALL lua_wrapper::push(void* ptr)
{
    //lua_pushboolean(state, ptr);
    //return void CALL();
}
