/*!
	@file			EntryPoint.cpp
	@brief		ExternalDynamicLinkLibrary�v���W�F�N�g�Ő�������*.dll�̎��s�e�X�g���B��ɃG���[�`�F�b�N�ȂǁB
	@detail		�\�߃v���W�F�N�g�Ƀp�X��ʂ����̂ƃr���h�C�x���g�ňˑ��t�@�C����"ExternalDynamicLinkLibrary"���R�s�[���鏈�������Ă���B
					���ˑ��v���W�F�N�g����������A���O��ύX�����ꍇ�̓p�X�̒ʂ������ƃr���h�C�x���g�̕ҏW���K�v�ɂȂ�
	@todo		�e�X�g�p�̂��̃v���W�F�N�g�ȊO�͊�{�I��dll�̃v���W�F�N�g�ɂȂ�͂��Ȃ̂Ńr���h���Ɉˑ��֌W��ǉ����Ă����@�\(�@�\)���~�����B
					��:XInput.dll�̃p�X��ʂ����ʂ̃v���W�F�N�g��p�ӂ�����ʓ|�ȃp�X�ǉ������ƃr���h�C�x���g�o�^�����ŏ���ɂ���Ă����̂��x�X�g�B
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
	@brief �G���g���[�|�C���g
	@note  CLI������s�����ۂɈ�����n����悤�ɗp�ӂ������Ƃ��B
	@param[in]:�����̌�
	@param[in]:����������
	@return:����I��=�O , �ُ�I��=!�O(TODO:�G���[�R�[�h��݂��A�N���X����������)
*/
int main(int argNum, const char* argments)
{
#pragma region ���������[�N
#if defined DEBUG||_DEBUG
	// ���������[�N.
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif // DEBUG||_DEBUG
#pragma endregion

	/*
	* @brief	UTF-8�G���R�[�h.
	* @detail	�ǂݍ���Lua�t�@�C����utf-8�ŃG���R�[�h����Ă���̂�locale��ݒ肵�Ă���.
	*/
	setlocale(LC_ALL, ".UTF8");

	auto factor = new lua_sample_factor();
	auto lua = factor->create();
	delete factor;

	/*
	* @brief	Lua�Œ�`�����ϐ��̎擾�d���̃T���v��.
	lua->get_lua_value_sample();
	*/

	/*
	* @brief	Lua�Œ�`�����֐���C�{�{�ŌĂяo���T���v��.
	lua->do_lua_method_sample();
	*/

	/*
	* @brief	Lua�ōs���R���[�`���̃T���v��.
	lua->do_lua_coroutine_sample();
	*/

	/*
	* @brief	Lua�e�[�u���̋����T���v��
	lua->do_lua_table_sample();
	*/

	/*
	* @brief	Lua��C++�̊֐����Ăяo���T���v��
	lua->do_cpp_method_sample();
	*/

	// �������J��
	delete lua;

	system("pause");
	return SUCCESS;
}
