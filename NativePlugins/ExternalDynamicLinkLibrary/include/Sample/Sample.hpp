//�ȉ��ߋ��̈╨�̃R�s�y:https://github.com/YuukiReiya/Template-UnityProject/blob/Develop/ColorScheme/UnityDLLforCPP/UnityDLLforCPP/Sample.hpp

//C����̊֐��Ƃ��ĔF�������邱�ƂŃl�[���}���O�����O�����
//��C++��C�Ƀ��b�v���ČĂяo���Ă��邩�����"DLLforC"�Ƃ������Ƃɒ���!
//�|�C���^�̈���:https://qiita.com/RyukiTANAKA96/items/000b11330c31435fcfae
//�O��m��:https://www.wagavulin.jp/entry/2017/02/09/215036
//�Q�l1:https://qiita.com/tan-y/items/64711b244cf294d6bb9d
#pragma once
#include <iostream>
#define DLL_EXPORT  extern "C" __declspec(dllexport)
#define UNITY_API __stdcall
//	Unity���Ptr�̌Ăяo�����ǂ̂悤�ɂ���邩���؂��邽�߂ɉ��L�T���v����p��

//	�l�n��
DLL_EXPORT int UNITY_API Add(int, int);

//	�����ɎQ�Ɠn��
DLL_EXPORT int UNITY_API AddPtr(int*, int);

//	�z��S�̂ɒl�����Z����
DLL_EXPORT int* UNITY_API AddArray(int*, size_t, int);

DLL_EXPORT int UNITY_API HotReloadTest();

template<typename T, std::size_t N>
static inline std::size_t GetArraySize(const T(&)[N]) { return N; }