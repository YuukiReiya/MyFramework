//http://marupeke296.com/LUA_No6_CreateWindow.html
#include <windows.h>
#include <tchar.h>
#include <lua.hpp>
#include <vector>
TCHAR gName[100] = _T("Lua�ɂ��E�B���h�E�쐬�T���v���v���O����");

#pragma region �N���X

class Object {
protected:
    char name[16];
public:
    Object() { strcpy_s<16>(name, "Object"); }
    virtual ~Object(){}

    const auto getName()const { return name; }
    virtual void draw(HDC hDC) = 0;
};

class Line :public Object {
public:
    Line(int x1, int y1, int x2, int y2) :
        x1(x1), y1(y1), x2(x2), y2(y2)
    {
        strcpy_s<16>(name, "Line");
    }
    virtual ~Line() {}
    void draw(HDC hDC)override {
        MoveToEx(hDC, x1, y1, NULL);
        LineTo(hDC, x2, y2);
    }
private:
    int x1, y1, x2, y2;
};
#pragma endregion

#pragma region �ϐ�
std::vector<Object*>objects;
#pragma endregion


#pragma region Lua

//�O���錾.
int getWindow(lua_State*);
int createObject(lua_State*);
int setObject(lua_State*);

/*
    @brief  �E�B���h�E����e�[�u����Lua���ɓn��.
*/
int getWindow(lua_State* L) 
{
    lua_newtable(L);

    // �֐��e�[�u�������.
    lua_pushcfunction(L, &setObject);
    lua_setfield(L, 1, "setObject");//   ������
    return 1;
}

/*
    @brief  �I�u�W�F�N�g�쐬�֐�.
*/
int createObject(lua_State* L)
{
    const auto name = lua_tostring(L, 1);
    Object* obj = nullptr;

    if (strcmp(name, "Line") == 0) {
        // ���C���I�u�W�F�N�g����
        int x1 = (int)lua_tonumber(L, 2);
        int y1 = (int)lua_tonumber(L, 3);
        int x2 = (int)lua_tonumber(L, 4);
        int y2 = (int)lua_tonumber(L, 5);

        // ���C���I�u�W�F�N�g�����
        obj = new Line(x1, y1, x2, y2);
    }

    lua_pop(L, lua_gettop(L));

    // �I�u�W�F�N�g���e�[�u���ɓo�^.
    lua_newtable(L);
    lua_pushlightuserdata(L, obj);
    lua_setfield(L, 1, "pointer"); //   ������ 

    return 1;
}

/*
    @brief  �E�B�W�F�b�g�I�u�W�F�N�g��o�^.
*/
int setObject(lua_State* L) 
{
    //  �������ɃE�B���h�E�I�u�W�F�N�g�̃e�[�u��.
    //  �������ɐݒ肵�����E�B�W�F�b�g�̃e�[�u���������Ă���z��.
    lua_getfield(L, 2, "pointer");
    auto obj = (Object*)(lua_topointer(L, lua_gettop(L)));

    // �O���[�o���ϐ��̃R���e�i�Ƀf�[�^��ǉ�.
    objects.push_back(obj);

    return 0;
}

#pragma endregion


LRESULT CALLBACK WndProc(HWND hWnd, UINT mes, WPARAM wParam, LPARAM lParam) {
    switch (mes) {
    case WM_DESTROY:
        PostQuitMessage(0);
        return 0;
    }
    return DefWindowProc(hWnd, mes, wParam, lParam);
}

int APIENTRY _tWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPTSTR lpCmdLine, int nCmdShow) {
    MSG msg; HWND hWnd;

    lua_State* L = luaL_newstate();
    luaL_openlibs(L);

    // Lua���̏������֐����Ăԁi�G���[�������ȗ����Ă��܂��j
    int result;
    if (result = luaL_dofile(L, "resources/dowindow_setting.lua"))
    {
        char msg[512];
        sprintf_s<512>(msg, "%d:failed to load lua.\n%s", result, lua_tostring(L, -1));
        MessageBoxA(NULL, msg, "failed", 0);
        lua_close(L);
        return -1;
    }
    lua_getglobal(L, "init");
    if (result = lua_type(L, -1) == LUA_TNIL) {
        MessageBoxA(NULL, "not found init function.", "failed", 0);
        lua_close(L);
        return -1;
    }

    //'init()�Ăяo��'
    if (result = lua_pcall(L, 0, 1, 0)) { 
        char msg[512];
        sprintf_s<512>(msg, "failed to call function.\n%s", lua_tostring(L, - 1));
        MessageBoxA(NULL, msg, "failed", 0);
        lua_close(L);
        return -1;
    }

    if (lua_type(L, -1) != LUA_TTABLE)
    {
        char msg[512];
        sprintf_s<512>(msg, "error. not return 'init()' method to table.\n%s", lua_tostring(L, -1));
        MessageBoxA(NULL, msg, "failed", 0);
        lua_close(L);
        return -1;
    }

    // �e�[�u������p�����[�^�擾
    lua_getfield(L, 1, "width");  // �e�[�u���Ɋ܂܂�镝�l���X�^�b�N�ɐς�
    lua_getfield(L, 1, "height"); // �e�[�u���Ɋ܂܂�鍂���l���X�^�b�N�ɐς�
    int width = 800, height = 600; // �ꉞ���������Ƃ�.
    if (lua_type(L, 2) == LUA_TNUMBER)
        width = (int)lua_tonumber(L, 2);
    if (lua_type(L, 3) == LUA_TNUMBER)
        height = (int)lua_tonumber(L, 3);

    // �X�^�b�N�����ꂢ�ɂ��Ƃ�.
    lua_pop(L, lua_gettop(L));

    // C����̃��\�b�h��Lua���ɓo�^.
    lua_register(L, "getWindow", &getWindow);
    lua_register(L, "createObject", &createObject);

    lua_getglobal(L, "setup");
    if (lua_pcall(L, 0, 0, 0)) {
        char msg[512];
        sprintf_s<512>(msg, "error. call function 'setup()' method.\n%s", lua_tostring(L, -1));
        MessageBoxA(NULL, msg, "failed", 0);
        lua_close(L);
        return -1;
    }

    // Lua�X�e�[�g�͂�������Ȃ��̂ŉ��
    lua_close(L);

    WNDCLASSEX wcex = { sizeof(WNDCLASSEX), CS_HREDRAW | CS_VREDRAW, WndProc, 0, 0, hInstance, NULL, NULL, (HBRUSH)(COLOR_WINDOW + 1), NULL, (TCHAR*)gName, NULL };
    if (!RegisterClassEx(&wcex))
        return 0;

    RECT r = { 0, 0, width, height };
    ::AdjustWindowRect(&r, WS_OVERLAPPEDWINDOW, FALSE);
    r.right -= r.left;
    r.bottom -= r.top;
    if (!(hWnd = CreateWindow(gName, gName, WS_OVERLAPPEDWINDOW, CW_USEDEFAULT, 0, r.right, r.bottom, NULL, NULL, hInstance, NULL)))
        return 0;

    ShowWindow(hWnd, nCmdShow);

    // ���b�Z�[�W ���[�v
    char str[512];
    sprintf_s<512>(str, "Window Width = %d, Window Height = %d", width, height);
    do {
        if (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE)) {
            DispatchMessage(&msg);
        }
        HDC hDC = GetDC(hWnd);
        ::TextOutA(hDC, 0, 0, str, (int)strlen(str));

        // �I�u�W�F�N�g�̕`��.
        for (auto it : objects)it->draw(hDC);

    } while (msg.message != WM_QUIT);

    return 0;
}