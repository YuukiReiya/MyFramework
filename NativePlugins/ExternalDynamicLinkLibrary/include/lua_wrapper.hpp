#pragma once
#include<lua.hpp>
//#define DLL_EXPORT  extern "C" __declspec(dllexport)
//#define UNITY_API __stdcall


// �Ăяo���K���x86/x64�Ő؂�ւ���
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
	*	@brief	: �X�^�b�N�ɐς�.
	*	@detail	: nil
	*/
	void CALL push();
	/*
	*	@func	: push
	*	@brief	: �X�^�b�N�ɐς�.
	*	@detail	: �y�ʃ��[�U�f�[�^:void*
	*/
	void  CALL push(void* ptr);
	/*
	*	@func	: push
	*	@brief	: �X�^�b�N�ɐς�.
	*	@detail	: �^�U�l�Bint�^(0, 0�ȊO)
	*/
	void CALL push(bool value);
	/*
	*	@func	: push
	*	@brief	: �X�^�b�N�ɐς�.
	*	@detail	: �f�t�H���g�ł�double�^
	*/
	void CALL push(double value);

	void CALL push(const char* value);
	void CALL push(lua_State* L, int idx);
private:
	lua_State* state;
};