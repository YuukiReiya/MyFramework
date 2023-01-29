-- グローバル関数

--[[
引数 & 戻り値無しのサンプル関数.
]]
function g_sample_func1()
    print("lua グローバル関数の呼び出し")
    CS.UnityEngine.Debug.Log("Debug.Logで呼び出される？ from get_global_lua_func_toC#.lua")
end

--[[
引数 & 戻り値有りのサンプル関数.
]]
function g_sample_func2(a)
    print("lua グローバル関数の呼び出し : 引数 * 100を返却")
    return a * 100
end

--[[
戻り値が関数のサンプル関数.
]]
function g_sample_func3()
    print("lua グローバル関数の呼び出し : 戻り値が関数")
    return result_sample_func
end

-- 返り値となる関数
function result_sample_func()
    print("call result_sample_func")
end

--[[
戻り値が複数のサンプル関数.
]]
function g_sample_func4()
    print("lua グローバル関数の呼び出し : 戻り値が複数")
    return 1,5,"戻り値複数のテスト"
end