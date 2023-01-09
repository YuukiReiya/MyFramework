// プリコンパイル済みヘッダー.
#pragma once
#include <stdlib.h>
#include <iostream>
#include <Windows.h>

#pragma region メモリリーク特定
#if defined DEBUG || defined _DEBUG
#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#define	new	new(_NORMAL_BLOCK, __FILE__, __LINE__)
#endif
#pragma endregion

#pragma region DLL(P/Invoke)
#define DLL_EXPORT  extern "C" __declspec(dllexport)
#define UNITY_API __stdcall

/*
	@sample
	DLL_EXPORT <型> UNITY_API <関数名>( 引数 ) { 定義 }
*/

#pragma endregion


#pragma region 変数名:値の出力
#if DEBUG||_DEBUG
#define VARARGOUT(var) std::cout<<#var<<":"<<var<<std::endl;
#else
#define VARARGOUT(var)
#endif // DEBUG||_DEBUG
#pragma endregion

/*
	@brief	ダミー処理
	@detail	P/Invoke用のダミー処理.
				"dllexport"の定義された関数が一つもないと".lib"が出来ずにリンカーエラーが出る。
				関数の実装が担保されれば削除してしまっていい.
*/
DLL_EXPORT inline void UNITY_API dummy() {}

/*
	@brief	Win32のエラーコードをメッセージとしてダイアログに出す
*/
DLL_EXPORT void UNITY_API MsgBoxWin32(DWORD errorcode);
