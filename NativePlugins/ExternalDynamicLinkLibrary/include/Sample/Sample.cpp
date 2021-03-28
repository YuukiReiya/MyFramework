//以下過去の遺物のコピペ:https://github.com/YuukiReiya/Template-UnityProject/blob/Develop/ColorScheme/UnityDLLforCPP/UnityDLLforCPP/Sample.cpp

#include "Sample.hpp"
#include <iostream>
using namespace std;

DLL_EXPORT int UNITY_API Add(int a, int b)
{
	return a + b;
}

DLL_EXPORT int UNITY_API AddPtr(int* a, int b)
{
	*a += b;
	return *a;
}

DLL_EXPORT int* UNITY_API AddArray(int* ptr, size_t size, int add)
{
	for (size_t i = 0; i < size; ++i)
	{
		ptr[i] += add;
	}
	return ptr;
}

DLL_EXPORT int UNITY_API HotReloadTest()
{
	//	UnityEditor起動中にビルドしてDLLがリロードされれば返り値を変更した場合に取得できる値が変わる
	return 10;
}