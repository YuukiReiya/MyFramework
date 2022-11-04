/*!
	@file			EntryPoint.cpp
	@brief		ExternalDynamicLinkLibraryプロジェクトで生成した*.dllの実行テスト環境。主にエラーチェックなど。
	@detail		予めプロジェクトにパスを通したのとビルドイベントで依存ファイルの"ExternalDynamicLinkLibrary"をコピーする処理を入れてある。
					※依存プロジェクトが増えたり、名前を変更した場合はパスの通し直しとビルドイベントの編集も必要になる
	@todo		テスト用のこのプロジェクト以外は基本的にdllのプロジェクトになるはずなのでビルド時に依存関係を追加してくれる機能(機構)が欲しい。
					例:XInput.dllのパスを通した別のプロジェクトを用意したら面倒なパス追加処理とビルドイベント登録無しで勝手にやってくれるのがベスト。
*/
#pragma once
#include <iostream>
#include <vector>
#include<lua.hpp>
#include "edlpch.hpp"
#include "../../../ExternalDynamicLinkLibrary/include/Sample/Sample.hpp"
#include "../../../ExternalDynamicLinkLibrary/include/lua_wrapper.hpp"
#define SUCCESS 0
#define FAILED -1

#pragma region メモリリーク特定
#ifndef _CRTDBG_MAP_ALLOC
#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#define	new	new(_NORMAL_BLOCK, __FILE__, __LINE__)
#endif // !_CRTDBG_MAP_ALLOC
#pragma endregion

#if DEBUG||_DEBUG
#define VARARGOUT(var) std::cout<<#var<<":"<<var<<std::endl;
#else
#define VARARGOUT(var)
#endif // DEBUG||_DEBUG

using namespace std;

#define Lua51
#define Lua54_

// 前方宣言
void stack_print(lua_State*);
bool load(lua_State*, string);
int Linear(lua_State*);
int Linear(float, float, float);
int glueFunc(lua_State*);

/*!
	@brief エントリーポイント
	@note  CLIから実行した際に引数を渡せるように用意だけしとく。
	@param[in]:引数の個数
	@param[in]:引数文字列
	@return:正常終了=０ , 異常終了=!０(TODO:エラーコードを設け、クラス分けしたい)
*/
int main(int argNum, const char* argments) 
{
#pragma region メモリリーク
#if defined DEBUG||_DEBUG
	// メモリリーク.
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif // DEBUG||_DEBUG
#pragma endregion

#ifdef Lua51
#pragma region 導入
#if false
	lua_State* L = luaL_newstate();
	lua_close(L);
#endif
#pragma endregion

#pragma region コルーチン
#if false

	lua_State* L = luaL_newstate();

	// コルーチンを使えるようにライブラリをLuaステートに設定する
	luaopen_base(L);

	if (luaL_dofile(L, "resources/lua/test.lua")) {
		cout << lua_tostring(L, lua_gettop(L)) << endl;
		lua_close(L);
		::system("pause");
	}

	// スレッド作成(コルーチン).
	auto co = lua_newthread(L);

	lua_getglobal(co, "step");

	cout << "wait getchar();" << endl;
	while (lua_resume(co,0))
	{
		stack_print(co);
		// Luaで返された結果をstring文字列で取得.
		auto str = lua_tostring(co, lua_gettop(co));
		cout << "str:" << str << endl;
		getchar();
	}

#endif
#pragma endregion

#pragma region 関数
#if false
	lua_State* L = luaL_newstate();

	// ここで呼び出しても無意味.
	// luaファイルを読込終わってからでないと意味ない!
	luaopen_base(L);		// 呼び出しても意味ないよ！！
	/*
	* @example	resources/lua/test.lua:16: attempt to call global 'print' (a nil value)
	* と思ったけど気のせいか…？
	* 初回は問題だった気がしたけど２回目以降は通るようになった。
	* 分からん。
	* 
	* lua側手を入れたらload関数(多分luaL_dofile)呼び出した時点でattempt to call global 'print' (a nil value)を吐くようになった。
	* lpcallで明示的に呼び出さずともdofileでlua処理を実行しているのか…？
	* ※load前にluaopen_baseを呼び出すことでattempt to call global 'print'は解消されたことを確認している。
	* やはりload前に呼び出しておいた方が良さげか。
	*/

	if (!load(L, "resources/lua/test.lua")) {
		::cout << "FAILED to initialize." << endl;
		return FAILED;
	}

	// Lua側で使用するライブラリの準備.
	// ※1 これをしないとLua側で利用可能な標準関数（printなど）が呼び出せない
	// ※2 必ず読込後にしないとダメ！
	//luaL_openlibs(L);	// lua5.0？一応動いた。
	//luaopen_base(L);		// こっちがいい？

	// 呼び出す関数を指定.
	lua_getglobal(L, "print_test");
	// 引数設定.
	lua_pushstring(L, "この文字列をLuaでprintしてください");

	// 現スタックの可視化
	stack_print(L);

	// 関数呼び出し.
	/*
		@param[in] lua_State*
		@param[in] lua関数の引数の数.
		@param[in] lua関数の戻り値の数.
		@param[in] errorfunc? // エラーが起きた時に帰ってくる値？ 謎
						  <br>第4引数は独自のエラーメッセージを取り扱う時に使いますが、デフォルトで良い場合は0を渡します。
	*/
	int ret = lua_pcall(L, 1, 0, 0);

	::cout << "result:" << ret << endl;

	stack_print(L);

#endif
#pragma endregion

#pragma region グルー関数(LuaからC言語の関数を呼び出す)
	lua_State* L = luaL_newstate();
	luaopen_base(L);
	if (!load(L, "resources/lua/test.lua")) {
		::cout << "failed to load lua.";
		return FAILED;
	}

	// lua側にC++のメソッドを登録.
	//
//#define NO_ARG
#ifdef NO_ARG
	lua_register(L, "GlueFuncTest", &glueFunc); // 第二引数に指定しているのがLua側に登録したメソッド名らしいのでC＋＋でAという関数もLua側にBと登録できそう。
	
	lua_getglobal(L, "entry2");
	lua_pcall(L, 0, 0, 0);
	stack_print(L);

#else
	lua_register(L, "Linear", &Linear); // 第二引数に指定しているのがLua側に登録したメソッド名らしいのでC＋＋でAという関数もLua側にBと登録できそう。

	::cout << "Lua呼び出し前" << endl;
	stack_print(L);
	::cout << endl;

	// luaの呼び出し.
	lua_getglobal(L, "entry");

	auto a = static_cast<float>(lua_tonumber(L, 1));
	auto b = static_cast<float>(lua_tonumber(L, 2));
	auto c = static_cast<float>(lua_tonumber(L, 3));

	//auto ret = Linear(a, b, c);
	//::cout << "Linear = " << ret << " a:" << a << " b:" << b << " c:" << c << endl;

	//lua_pcall(L, 3, 1, 0); // No Stack.
	//lua_pcall(L, 0, 0, 0); // 0.2 (Lua第三引数t:0.2)
	//lua_pcall(L, 1, 0, 0); // 002(-001): STRING attempt to call a table value
	lua_pcall(L, 0, 1, 0);

	::cout << "Lua呼び出し後" << endl;
	stack_print(L);
#endif

#pragma endregion


#pragma region テーブル
#if false



#endif
#pragma endregion

#elif defined Lua54

#pragma region 導入
#if false
	lua_State* L = luaL_newstate();
	lua_close(L);
#endif
#pragma endregion

#pragma region コルーチン
#if true
	lua_State* L = luaL_newstate();

	// コルーチンを使えるようにライブラリをLuaステートに設定する
	luaopen_base(L);

	//if (luaL_dofile(L, "resources/lua/test.lua")) {
	//	::cout << lua_tostring(L, lua_gettop(L)) << endl;
	//	lua_close(L);
	//	::system("pause");
	//}

	if (luaL_dofile(L, "resources/lua/Coroutine.lua")) {
		::cout << lua_tostring(L, lua_gettop(L)) << endl;
		lua_close(L);
		:: cout << "FAILED" << endl;
		::system("pause");
	}


	// スレッド作成(コルーチン).
	auto co = lua_newthread(L);

	int* res = new int();

	//::cout << "getglobal=" << getglobal << endl;
	

	stack_print(L);

	::cout << "wait getchar();" << endl;
	//while (lua_resume(co, co, 0, res) == LUA_OK)
	//{
	//	stack_print(co);
	//	getchar();
	//}
	::cout << "end while lua_resume print -> co" << endl;
	stack_print(co);
	::cout << "end while lua_resume print -> L" << endl;
	stack_print(L);
	delete(res);
#endif
#pragma endregion


#else
	


	lua_wrapper* lua = new lua_wrapper();
	lua->initialize();

#pragma region lua_Stateのスタックにデータを積む
	//lua_pushboolean(lua->get_state(), 1);
	//lua_pushnumber(lua->get_state(), 100.0);
	//lua_pushstring(lua->get_state(), "Marupeke");
#pragma endregion

	

	//ファイル読み込み
	if (luaL_dofile(lua->get_state(), "resources/lua/test.lua")) {
		::printf("%s\n", lua_tostring(lua->get_state(), lua_gettop(lua->get_state())));
		lua_close(lua->get_state());
		::system("pause");

		return FAILED;
	}
#pragma region 外部のluaファイルから変数読込
#if true
#define SAMPLE_EXTERNAL_LUA_READ
	//変数読み込み
	lua_getglobal(lua->get_state(), "wndWidth");
	lua_getglobal(lua->get_state(), "wndHeight");
	lua_getglobal(lua->get_state(), "wndName");

	lua->stack_print();
#endif
#pragma endregion

#pragma region 外部のluaファイルから関数を呼ぶ
#ifndef SAMPLE_EXTERNAL_LUA_READ
#if false


	// Luaステート内にある"calc関数"を指定
	lua_getglobal(lua->get_state(), "calc");

	// 引数を設定
	::printf("push\n");
	lua_pushnumber(lua->get_state(), 10);
	lua_pushnumber(lua->get_state(), 20);

	lua->stack_print();

	// 関数実行
	// ※1 スタックに積まれたものはlua_pcallを呼んだときにスタックから取り出され、代わりに戻り値がスタックに積まれる
	// ※2 スタックに積まれた全てのデータがポップされるわけではなく、
	::printf("関数呼び出し\n");
	
	/*
	* @fn			lua_pcall
	*					Luaファイルで定義した関数をC言語側から呼び出す.
	* @brief		Lua関数の呼び出し.
	* @param1 (L) : lua_State
	* @param2 (n) : 引数の数
	* @param3 (r) : 戻り値の数
	* @param4 (f) : 独自のエラーメッセージを取り扱う時に使いますが、デフォルトで良い場合は0を渡します。
	* @return 0 or 1 : エラーハンドル
	* @sa			http://marupeke296.com/LUA_No2_Begin.html
	*/
	if (lua_pcall(lua->get_state(), 2, 4, 0)) {
		::printf("%s\n", lua_tostring(lua->get_state(), lua_gettop(lua->get_state())));
		lua_close(lua->get_state());
		return FAILED;
	}

	::printf("戻り値\n");
	// 複数ある戻り値を取得
	float add_Res = (float)lua_tonumber(lua->get_state(), 1);
	float sub_Res = (float)lua_tonumber(lua->get_state(), 2);
	float mult_Res = (float)lua_tonumber(lua->get_state(), 3);
	float dev_Res = (float)lua_tonumber(lua->get_state(), 4);

	float results[]= {add_Res, sub_Res, mult_Res, dev_Res};
	for (auto& r : results) { VARARGOUT(r); }

	lua->stack_print();
#endif
#endif // !SAMPLE_EXTERNAL_LUA_READ
#pragma endregion

#pragma region コルーチン
	/*
	* @sa http://marupeke296.com/LUA_No3_Coroutine.html
	*/
#if true

	lua_State* L = luaL_newstate();

	// コルーチンを使えるようにライブラリをLuaステートに設定する
	luaopen_base(L);

	// ファイルを開く
	if (luaL_dofile(L, "resources/lua/Coroutine.lua"))
	{
		::printf("%s\n", lua_tostring(L, lua_gettop(L)));
		lua_close(L);
		return FAILED;
	}
	// コルーチン(スレッド)を作成
	lua_State* co = lua_newthread(L);

	// コルーチンステート内にある“step関数”を指定
	lua_getglobal(co, "step");
	int* r = new int;

	try
	{
		while (lua_resume(co,0))
		{

		}
		{

		}
	}
	catch (const std::exception&e)
	{
		cout<<"例外" << endl << e.what() << endl;
	}

#endif
#pragma endregion


	// スタック削除.
	//int num = lua_gettop(lua->get_state());
	//lua_pop(lua->get_state(), num);
	delete (lua);
#endif // Lua54

	//とりあえず空
	::cout << "正常終了" << endl;
	::system("pause");
	return SUCCESS;
}

void stack_print(lua_State* state) {
	const int num = lua_gettop(state);

	::cout << "lua_State->stack print" << endl;

	if (num == 0) {
		::cout << "No stack." << endl;
		return;
	}

	for (int i = num; i >= 1; i--) {
		printf("%03d(%04d): ", i, -num + i - 1);
		int type = lua_type(state, i);
		switch (type) {
		case LUA_TNIL:
			printf("NIL\n");
			break;
		case LUA_TBOOLEAN:
			printf("BOOLEAN %s\n", lua_toboolean(state, i) ? "true" : "false");
			break;
		case LUA_TLIGHTUSERDATA:
			printf("LIGHTUSERDATA\n");
			break;
		case LUA_TNUMBER:
			printf("NUMBER %f\n", lua_tonumber(state, i));
			break;
		case LUA_TSTRING:
			printf("STRING %s\n", lua_tostring(state, i));
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

	::cout << endl;
}

bool load(lua_State* L, string path) {

	if (luaL_dofile(L, path.c_str())) {
		::cout << lua_tostring(L, lua_gettop(L)) << endl;
		lua_close(L);
		return false;
	}
	return true;
}

#pragma region グルー関数

// 
int glueFunc(lua_State*) {
	::cout << "Luaから呼び出されたよ" << endl;
	// 戻り値無いので'0'
	return 0;
}

// 線形補完.
// (引数3つ、戻り値float型1つの例)
int Linear(lua_State* L)
{
	// 第一引数.
	float start = static_cast<float>(lua_tonumber(L, 1));
	// 第二引数.
	float end = static_cast<float>(lua_tonumber(L, 2));
	// 第三引数.
	float t = static_cast<float>(lua_tonumber(L, 3));

	// 補完計算？
	auto result = (1.f - t) * start + t * end;

	// Lua側で割り当てた引数をprintする.
	::cout << "線形補完[s]" << start << ",[e]" << end << ",[t]" << t << "[result]" << result << endl;

	// スタック削除.
	lua_pop(L, lua_gettop(L));

	// スタックに戻り値を積む.
	lua_pushnumber(L, result);

	// 戻り値の数を返す.
	return 1;
}

int Linear(float a, float b, float t) 
{
	return (1.f - t) * a + t * b;
}
#pragma endregion
