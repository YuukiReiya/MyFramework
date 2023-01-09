#include "lua_parser.hpp"

DLL_EXPORT lua_wrapper* UNITY_API edllua_create()
{
    return new lua_wrapper();
}

DLL_EXPORT void UNITY_API edllua_destroy(lua_wrapper*instance)
{
    if (instance == nullptr)return;
    delete instance;
    instance = nullptr;
}

DLL_EXPORT void UNITY_API edllua_initialize(lua_wrapper*instance)
{
    if (instance == nullptr)return;
    instance->initialize();
}

DLL_EXPORT void UNITY_API edllua_finalize(lua_wrapper*instance)
{
    if (instance == nullptr)return;
    instance->finalize();
}

DLL_EXPORT bool UNITY_API edllua_load(lua_wrapper* instance, std::string lpFile)
{
    if (instance == nullptr)return false;
    
    return instance->load(lpFile);
}

DLL_EXPORT bool UNITY_API edllua_domethod(lua_wrapper*instance, std::string lpMethod)
{
    if (instance == nullptr || instance->get_state() == nullptr)return false;

    lua_getglobal(instance->get_state(), lpMethod.c_str());
    if (lua_pcall(instance->get_state(), 0, 0, 0) != 0)
    {
        return false;
    }
    return true;
}

DLL_EXPORT char* UNITY_API edllua_getcomment(lua_wrapper* instance, std::string lpMethod)
{
#define RETURN_EMPTY strcpy_s(buff,1, " ");return buff;
    std::string str;
    if (instance == nullptr || instance->get_state() == nullptr
        || lua_getglobal(instance->get_state(), lpMethod.c_str()) != 0)
    {
        str = "";
    }
    else 
    {
        str = static_cast<std::string>(lua_tostring(instance->get_state(), 1));
    }
    char* buff = new char[str.size() + 1];
    strcpy_s(buff, str.size() + 1, str.c_str());
    return buff;
}
