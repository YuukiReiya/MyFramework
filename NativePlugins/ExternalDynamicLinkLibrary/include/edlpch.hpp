// �v���R���p�C���ς݃w�b�_�[.
#pragma once
#include <stdlib.h>
#include <iostream>

#pragma region ���������[�N����
#if defined DEBUG || defined _DEBUG
#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#define	new	new(_NORMAL_BLOCK, __FILE__, __LINE__)
#endif
#pragma endregion

#ifndef EDL_PCH_HPP
#define EDL_PCH_HPP

// �v���R���p�C������w�b�_�[�������ɒǉ����܂�
#include "lua_wrapper.hpp"

#endif //EDL_PCH_HPP