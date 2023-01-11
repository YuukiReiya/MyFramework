#pragma once
#include"common/edlpch.hpp"
#include"lua/lua_wrapper.hpp"
#include<string>
/*
*	@brief	インスタンス作成
*	@detail	new
*/
DLL_EXPORT lua_wrapper* UNITY_API edllua_create();
/*
*	@brief	インスタンスの破棄
*	@detail	delete
*/
DLL_EXPORT void UNITY_API edllua_destroy(lua_wrapper*);
/*
*	@brief	初期化
*/
DLL_EXPORT void UNITY_API edllua_initialize(lua_wrapper*);
/*
*	@brief	終了処理
*/
DLL_EXPORT void UNITY_API edllua_finalize(lua_wrapper*);
/*
*	@brief	ロード処理
*/
DLL_EXPORT bool UNITY_API edllua_load(lua_wrapper*,std::string);
/*
*	@brief	メソッドの呼び出し
*/
DLL_EXPORT bool UNITY_API edllua_domethod(lua_wrapper*, std::string);
/*
*	@brief	luaファイルにある文字列を読み込む
*	@detail	lua:決め打ちの関数を呼び出して返り値で返却された文字列をそのまま返す.
*/
DLL_EXPORT char* UNITY_API edllua_getcomment(lua_wrapper*, std::string);


#ifdef _MSC_VER
//#define DISABLE_C4996   __pragma(warning(push)) __pragma(warning(disable:4996))
//#define ENABLE_C4996    __pragma(warning(pop))

//#pragma warning(disable:4996)
#else
#define DISABLE_C4996
#define ENABLE_C4996
#endif
DLL_EXPORT char* UNITY_API char_test() 
{
	std::string str = "Hello! あs";
	//char* buffer = new char[str.size() + 1]; // 終端文字分 +1
	 char* buffer = new char[str.size()]; // 終端文字分 +1
	//std::strcpy(buffer, str.c_str());
	// クラッシュ↓
	//strcpy_s(buffer, sizeof(&buffer), str.c_str());
	//strcpy_s(buffer, str.size() + 1, str.c_str());
	 //size_t size = str.size() + 1;
	strcpy_s(buffer, str.size() + 1, str.c_str());
	//strcpy_s(buffer, 7, str.c_str());
	return buffer;
}

DLL_EXPORT void UNITY_API set(char*buff)
//DLL_EXPORT void UNITY_API set(char16_t* buff)
{
	std::string str = "aaaあああｊｊｊｊok";
	//std::string str = "aaa_ok";
	strcpy_s(buff, str.size() + 1, str.c_str());
	// ↑大丈夫.

	// ↓ダメ.
	//std::u16string str = u"aaaあああｊｊｊｊok";
	////std::char_traits<char16_t>::copy(buff, str.c_str(), str.size() + 1); // コピー

	//size_t size = (str.size() + 1) * sizeof(char16_t);
	//buff = (char16_t*)malloc(size);//メモリ確保
	//std::memcpy(buff, str.c_str(), size);//コピー
}
