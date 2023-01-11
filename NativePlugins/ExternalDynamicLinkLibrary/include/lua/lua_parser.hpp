#pragma once
#include"common/edlpch.hpp"
#include"lua/lua_wrapper.hpp"
#include<string>
/*
*	@brief	�C���X�^���X�쐬
*	@detail	new
*/
DLL_EXPORT lua_wrapper* UNITY_API edllua_create();
/*
*	@brief	�C���X�^���X�̔j��
*	@detail	delete
*/
DLL_EXPORT void UNITY_API edllua_destroy(lua_wrapper*);
/*
*	@brief	������
*/
DLL_EXPORT void UNITY_API edllua_initialize(lua_wrapper*);
/*
*	@brief	�I������
*/
DLL_EXPORT void UNITY_API edllua_finalize(lua_wrapper*);
/*
*	@brief	���[�h����
*/
DLL_EXPORT bool UNITY_API edllua_load(lua_wrapper*,std::string);
/*
*	@brief	���\�b�h�̌Ăяo��
*/
DLL_EXPORT bool UNITY_API edllua_domethod(lua_wrapper*, std::string);
/*
*	@brief	lua�t�@�C���ɂ��镶�����ǂݍ���
*	@detail	lua:���ߑł��̊֐����Ăяo���ĕԂ�l�ŕԋp���ꂽ����������̂܂ܕԂ�.
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
	std::string str = "Hello! ��s";
	//char* buffer = new char[str.size() + 1]; // �I�[������ +1
	 char* buffer = new char[str.size()]; // �I�[������ +1
	//std::strcpy(buffer, str.c_str());
	// �N���b�V����
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
	std::string str = "aaa��������������ok";
	//std::string str = "aaa_ok";
	strcpy_s(buff, str.size() + 1, str.c_str());
	// �����v.

	// ���_��.
	//std::u16string str = u"aaa��������������ok";
	////std::char_traits<char16_t>::copy(buff, str.c_str(), str.size() + 1); // �R�s�[

	//size_t size = (str.size() + 1) * sizeof(char16_t);
	//buff = (char16_t*)malloc(size);//�������m��
	//std::memcpy(buff, str.c_str(), size);//�R�s�[
}
