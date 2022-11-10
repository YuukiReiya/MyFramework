-- Luaの関数をC++側で呼び出す
-- サンプル用のLua

-- C言語側で呼び出すメソッドの定義
function calc(a,b)
    return a+b,a-b,a*b,a/b;
end