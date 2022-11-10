-- Luaのコルーチン
-- サンプル用のLua

-- C言語側で呼び出すメソッドの定義
function step()
    coroutine.yield("step1: そこは広場だった。");
    coroutine.yield("step2: 小さな滑り台があった。");
    coroutine.yield("step3: 昔ここで良く遊んだ事を思い出した。");
end