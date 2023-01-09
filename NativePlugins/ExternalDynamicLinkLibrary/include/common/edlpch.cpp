#include "common/edlpch.hpp"
#include<Windows.h>
#include<tchar.h>

DLL_EXPORT void UNITY_API MsgBoxWin32(DWORD errorcode)
{
    if (errorcode == NO_ERROR)return;

    LPVOID lpMsgBuf;
    FormatMessage(
        FORMAT_MESSAGE_ALLOCATE_BUFFER                                                   //  テキストのメモリ割り当てを要求する
        | FORMAT_MESSAGE_FROM_SYSTEM                                                       //  エラーメッセージはWindowsが用意しているものを使用
        | FORMAT_MESSAGE_IGNORE_INSERTS,                                                   //  次の引数を無視してエラーコードに対するエラーメッセージを作成する
        NULL, errorcode, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),    //  言語を指定
        (LPTSTR)&lpMsgBuf,                                                                                  //  メッセージテキストが保存されるバッファへのポインタ
        0,
        NULL);

    MessageBox(NULL ,static_cast<LPCTSTR>(lpMsgBuf), _TEXT("エラー"), MB_OK | MB_ICONINFORMATION);
    LocalFree(lpMsgBuf);
}
