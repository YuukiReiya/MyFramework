// プリコンパイル済みヘッダー.
#pragma once
#include <stdlib.h>
#include <iostream>

#pragma region メモリリーク特定
#if defined DEBUG || defined _DEBUG
#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#define	new	new(_NORMAL_BLOCK, __FILE__, __LINE__)
#endif
#pragma endregion

#ifndef EDL_PCH_HPP
#define EDL_PCH_HPP

// プリコンパイルするヘッダーをここに追加します
#include "lua_wrapper.hpp"

#endif //EDL_PCH_HPP