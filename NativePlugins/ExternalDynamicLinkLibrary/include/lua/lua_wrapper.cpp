#include<lua.hpp>
#include<iostream>
#include"lua_wrapper.hpp"


// WIN32APPの場合はprintfがダメそうなのでOutputDebugStringAを使うらしい。
// ひとまずServer(Linux),Client(x64)なので考えていない.

using namespace std;

 lua_wrapper::lua_wrapper()
{
	pState = nullptr;
    initialize();
}

 lua_wrapper::~lua_wrapper()
{
    finalize();
}

void  lua_wrapper::initialize()
{
	pState = luaL_newstate();
}

void  lua_wrapper::finalize()
{
    // スタックに積まれたデータをポップして削除.
    lua_pop(pState, lua_gettop(pState));
	lua_close(pState);
	pState = nullptr;
}

void  lua_wrapper::stack_print(lua_wrapper* wrapper)
{
    const int num = lua_gettop(wrapper->pState);

    cout << "lua_State->stack print" << endl;

    if (num == 0) {
        cout << "No stack." << endl;
        return;
    }

    for (int i = num; i >= 1; i--) {
        printf("%03d(%04d): ", i, -num + i - 1);
        int type = lua_type(wrapper->pState, i);
        switch (type) {
        case LUA_TNIL:
            printf("NIL\n");
            break;
        case LUA_TBOOLEAN:
            printf("BOOLEAN %s\n", lua_toboolean(wrapper->pState, i) ? "true" : "false");
            break;
        case LUA_TLIGHTUSERDATA:
            printf("LIGHTUSERDATA\n");
            break;
        case LUA_TNUMBER:
            printf("NUMBER %f\n", lua_tonumber(wrapper->pState, i));
            break;
        case LUA_TSTRING:
            printf("STRING %s\n", lua_tostring(wrapper->pState, i));
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

void  lua_wrapper::stack_print()
{
    stack_print(this);
}

bool lua_wrapper::load(std::string lpFile)
{
    if (luaL_dofile(pState, lpFile.c_str()))
    {
        std::cout << lua_tostring(pState, lua_gettop(pState)) << std::endl;
        lua_close(pState);
        std::cout << "failed to load lua. path:" << lpFile << std::endl;

        return false;
    }
    return true;
}

void  lua_wrapper::push()
{
    lua_pushnil(pState);
    //return void ();
}

void  lua_wrapper::push(void* ptr)
{
    //lua_pushboolean(state, ptr);
    //return void ();
}

void lua_wrapper::push(bool value)
{
    lua_pushboolean(pState, value);
}

void lua_wrapper::push(double value)
{
}

void lua_wrapper::push(const char* value)
{
}

void lua_wrapper::push(lua_State* L, int idx)
{
}
