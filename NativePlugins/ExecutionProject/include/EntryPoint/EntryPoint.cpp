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
#include "edlpch.hpp"
#include "../../../ExternalDynamicLinkLibrary/include/Sample/Sample.hpp"
#include "../../../ExternalDynamicLinkLibrary/include/lua_wrapper.hpp"
#include "../../ExecutionProject/include/sample/lua/lua_sample_51.hpp"
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

	auto lua =
#if LUA_VERSION_NUM == 501
		new lua_sample_51()
#elif LUA_VERSION_NUM==504

#endif
		;
	/*
	* @brief	Lua�Œ�`�����ϐ��̎擾�d���̃T���v��.
	*/
	lua->get_lua_value_sample();

	system("pause");
	return SUCCESS;
}
