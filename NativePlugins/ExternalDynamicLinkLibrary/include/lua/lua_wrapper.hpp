#pragma once
#include<lua.hpp>

class lua_wrapper 
{
public:
	 lua_wrapper();
	 ~lua_wrapper();
	void  initialize();
	void  finalize();
	static void  stack_print(lua_wrapper* warpper);
	void  stack_print();
	inline lua_State*  get_state() { return pState; }
	bool load(std::string);

	/*	
	*	@func	: push
	*	@brief	: �X�^�b�N�ɐς�.
	*	@detail	: nil
	*/
	void  push();
	/*
	*	@func	: push
	*	@brief	: �X�^�b�N�ɐς�.
	*	@detail	: �y�ʃ��[�U�f�[�^:void*
	*/
	void   push(void* ptr);
	/*
	*	@func	: push
	*	@brief	: �X�^�b�N�ɐς�.
	*	@detail	: �^�U�l�Bint�^(0, 0�ȊO)
	*/
	void  push(bool value);
	/*
	*	@func	: push
	*	@brief	: �X�^�b�N�ɐς�.
	*	@detail	: �f�t�H���g�ł�double�^
	*/
	void  push(double value);

	void  push(const char* value);
	void  push(lua_State* L, int idx);
private:
	lua_State* pState;
};