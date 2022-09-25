-- Lua側のグローバル変数をC言語側で呼び出せるようにテスト.
wndWidth=640;
wndHeight=480;
wndName="Marupeke";

-- C言語側で関数呼び出しのテスト.
function add(a,b)
    return a+b;
end
function calc(a,b)
    return a+b,a-b,a*b,a/b;
end

-- コルーチン呼び出しのテスト.
function step()
coroutine.yield("そこは広場だった。")
coroutine.yield("小さな滑り台があった。")
coroutine.yield("昔ここで良く遊んだことを思い出した。")
end