//http://marupeke296.com/LUA_No6_CreateWindow.html
#include <windows.h>
#include <tchar.h>
#include <lua.hpp>
#include <vector>
TCHAR gName[100] = _T("Luaによるウィンドウ作成サンプルプログラム");

#pragma region クラス

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

#pragma region 変数
std::vector<Object*>objects;
#pragma endregion


#pragma region Lua

//前方宣言.
int getWindow(lua_State*);
int createObject(lua_State*);
int setObject(lua_State*);

/*
    @brief  ウィンドウ操作テーブルをLua側に渡す.
*/
int getWindow(lua_State* L) 
{
    lua_newtable(L);

    // 関数テーブルを作る.
    lua_pushcfunction(L, &setObject);
    lua_setfield(L, 1, "setObject");//   第一引数
    return 1;
}

/*
    @brief  オブジェクト作成関数.
*/
int createObject(lua_State* L)
{
    const auto name = lua_tostring(L, 1);
    Object* obj = nullptr;

    if (strcmp(name, "Line") == 0) {
        // ラインオブジェクト生成
        int x1 = (int)lua_tonumber(L, 2);
        int y1 = (int)lua_tonumber(L, 3);
        int x2 = (int)lua_tonumber(L, 4);
        int y2 = (int)lua_tonumber(L, 5);

        // ラインオブジェクトを作る
        obj = new Line(x1, y1, x2, y2);
    }

    lua_pop(L, lua_gettop(L));

    // オブジェクトをテーブルに登録.
    lua_newtable(L);
    lua_pushlightuserdata(L, obj);
    lua_setfield(L, 1, "pointer"); //   第一引数 

    return 1;
}

/*
    @brief  ウィジェットオブジェクトを登録.
*/
int setObject(lua_State* L) 
{
    //  第一引数にウィンドウオブジェクトのテーブル.
    //  第二引数に設定したいウィジェットのテーブルが入っている想定.
    lua_getfield(L, 2, "pointer");
    auto obj = (Object*)(lua_topointer(L, lua_gettop(L)));

    // グローバル変数のコンテナにデータを追加.
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

    // Lua内の初期化関数を呼ぶ（エラー処理を省略しています）
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

    //'init()呼び出し'
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

    // テーブルからパラメータ取得
    lua_getfield(L, 1, "width");  // テーブルに含まれる幅値をスタックに積む
    lua_getfield(L, 1, "height"); // テーブルに含まれる高さ値をスタックに積む
    int width = 800, height = 600; // 一応初期化しとく.
    if (lua_type(L, 2) == LUA_TNUMBER)
        width = (int)lua_tonumber(L, 2);
    if (lua_type(L, 3) == LUA_TNUMBER)
        height = (int)lua_tonumber(L, 3);

    // スタックをきれいにしとく.
    lua_pop(L, lua_gettop(L));

    // C言語のメソッドをLua側に登録.
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

    // Luaステートはもういらないので解放
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

    // メッセージ ループ
    char str[512];
    sprintf_s<512>(str, "Window Width = %d, Window Height = %d", width, height);
    do {
        if (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE)) {
            DispatchMessage(&msg);
        }
        HDC hDC = GetDC(hWnd);
        ::TextOutA(hDC, 0, 0, str, (int)strlen(str));

        // オブジェクトの描画.
        for (auto it : objects)it->draw(hDC);

    } while (msg.message != WM_QUIT);

    return 0;
}