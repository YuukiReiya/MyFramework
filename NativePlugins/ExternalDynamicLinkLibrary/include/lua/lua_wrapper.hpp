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
	*	@brief	: スタックに積む.
	*	@detail	: nil
	*/
	void  push();
	/*
	*	@func	: push
	*	@brief	: スタックに積む.
	*	@detail	: 軽量ユーザデータ:void*
	*/
	void   push(void* ptr);
	/*
	*	@func	: push
	*	@brief	: スタックに積む.
	*	@detail	: 真偽値。int型(0, 0以外)
	*/
	void  push(bool value);
	/*
	*	@func	: push
	*	@brief	: スタックに積む.
	*	@detail	: デフォルトではdouble型
	*/
	void  push(double value);

	void  push(const char* value);
	void  push(lua_State* L, int idx);
private:
	lua_State* pState;
};