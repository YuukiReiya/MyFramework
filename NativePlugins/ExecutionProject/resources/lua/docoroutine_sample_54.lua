-- Luaのコルーチン
-- サンプル用のLua

-- C言語側で呼び出すメソッドの定義
-- C言語で呼び出される前提なら"coroutine.create"でコルーチンを作っておく必要はない.
function step()
    print("1st");
    coroutine.yield("step1: そこは広場だった。");
    print("2nd");
    coroutine.yield("step2: 小さな滑り台があった。");
    print("3rd");
    coroutine.yield("step3: 昔ここで良く遊んだ事を思い出した。");
end

--[[
参考
https://gist.github.com/hayajo/da35d970d095710e8ec8

CMD 
Lua構文チェック.
lua.exe <*.lua>

    
-- lambdaでコルーチンにメソッドを登録.
co = coroutine.create(function()
    coroutine.yield("step1: そこは広場だった。");
    coroutine.yield("step2: 小さな滑り台があった。");
    coroutine.yield("step3: 昔ここで良く遊んだ事を思い出した。");
end)


print("--lambda--")
print(coroutine.resume(co));
print(coroutine.resume(co));
print(coroutine.resume(co));
print(coroutine.resume(co)); -- 空出力.

print("--coroutine.create(step)--")
co_f = coroutine.create(step);
print(coroutine.resume(co_f));
print(coroutine.resume(co_f));
print(coroutine.resume(co_f));
print(coroutine.resume(co_f)); -- 空出力.
]]
