#pragma once
#include<lua.hpp>
//#define DLL_EXPORT  extern "C" __declspec(dllexport)
//#define UNITY_API __stdcall



class lua_wrapper 
{
public:
	__cdecl lua_wrapper();
	__cdecl ~lua_wrapper();
	void __cdecl initialize();
	void __cdecl finalize();

private:
	lua_State* state;
};