#pragma once
#include<lua.hpp>
//#define DLL_EXPORT  extern "C" __declspec(dllexport)
//#define UNITY_API __stdcall


// 呼び出し規約をx86/x64で切り替える
// 64bit
#if _M_X64
#define CALL __cdecl
// 32bit
#else
#define CALL
#endif
class lua_wrapper 
{
public:
	CALL lua_wrapper();
	CALL ~lua_wrapper();
	void CALL initialize();
	void CALL finalize();
	static void CALL stack_print(lua_wrapper* warpper);
	void CALL stack_print();
	inline lua_State* CALL get_state() { return state; }

	/*	
	*	@func	: push
	*	@brief	: スタックに積む.
	*	@detail	: nil
	*/
	void CALL push();
	/*
	*	@func	: push
	*	@brief	: スタックに積む.
	*	@detail	: 軽量ユーザデータ:void*
	*/
	void  CALL push(void* ptr);
	/*
	*	@func	: push
	*	@brief	: スタックに積む.
	*	@detail	: 真偽値。int型(0, 0以外)
	*/
	void CALL push(bool value);
	/*
	*	@func	: push
	*	@brief	: スタックに積む.
	*	@detail	: デフォルトではdouble型
	*/
	void CALL push(double value);

	void CALL push(const char* value);
	void CALL push(lua_State* L, int idx);
private:
	lua_State* state;
};