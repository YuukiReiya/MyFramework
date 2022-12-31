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
#include "../../../ExternalDynamicLinkLibrary/include/Sample/Sample.hpp"
#include "../../ExecutionProject/include/sample/lua/lua_sample_factor.hpp"
#include "common/edlpch.hpp"
#define SUCCESS 0
#define FAILED -1

using namespace std;

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

	/*
	* @brief	UTF-8エンコード.
	* @detail	読み込むLuaファイルがutf-8でエンコードされているのでlocaleを設定しておく.
	*/
	setlocale(LC_ALL, ".UTF8");

	auto factor = new lua_sample_factor();
	auto lua = factor->create();
	delete factor;

	/*
	* @brief	Luaで定義した変数の取得仕方のサンプル.
	lua->get_lua_value_sample();
	*/

	/*
	* @brief	Luaで定義した関数をC＋＋で呼び出すサンプル.
	lua->do_lua_method_sample();
	*/

	/*
	* @brief	Luaで行うコルーチンのサンプル.
	lua->do_lua_coroutine_sample();
	*/

	/*
	* @brief	Luaテーブルの挙動サンプル
	lua->do_lua_table_sample();
	*/

	/*
	* @brief	LuaでC++の関数を呼び出すサンプル
	lua->do_cpp_method_sample();
	*/

	// メモリ開放
	delete lua;

	system("pause");
	return SUCCESS;
}
