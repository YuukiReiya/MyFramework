//以下過去の遺物のコピペ:https://github.com/YuukiReiya/Template-UnityProject/blob/Develop/ColorScheme/UnityDLLforCPP/UnityDLLforCPP/Sample.hpp

//C言語の関数として認識させることでネームマングリングを回避
//※C++をCにラップして呼び出しているから実質"DLLforC"ということに注意!
//ポインタの扱い:https://qiita.com/RyukiTANAKA96/items/000b11330c31435fcfae
//前提知識:https://www.wagavulin.jp/entry/2017/02/09/215036
//参考1:https://qiita.com/tan-y/items/64711b244cf294d6bb9d
#pragma once
#include <iostream>
#define DLL_EXPORT  extern "C" __declspec(dllexport)
#define UNITY_API __stdcall
//	Unity上でPtrの呼び出しがどのようにされるか検証するために下記サンプルを用意

//	値渡し
DLL_EXPORT int UNITY_API Add(int, int);

//	引数に参照渡し
DLL_EXPORT int UNITY_API AddPtr(int*, int);

//	配列全体に値を加算する
DLL_EXPORT int* UNITY_API AddArray(int*, size_t, int);

DLL_EXPORT int UNITY_API HotReloadTest();

template<typename T, std::size_t N>
static inline std::size_t GetArraySize(const T(&)[N]) { return N; }