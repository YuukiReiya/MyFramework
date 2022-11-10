-- C++側の関数をLuaで呼び出す
-- サンプル用のLua

-- C言語側nの関数を呼び出すメソッド


-- 線形補完
-- 補完結果をprintで出力.
-- Linear_print=function()
--     while t<= 1.0 do
--         print(Linear(a,b,t));
--         t=t+0.1;
--     end
-- end
a = 10.0;
b = 30.0;
t = 0.0;
while t<= 1.0 do
            print(Linear(a,b,t));
            t=t+0.1;
        end

function print_noarg()

    print("No argment lua function.");
    func_cpp();
end