-- Luaのテーブル
-- サンプル用のLua

-- 0.テーブルとは.
--   C言語でいうクラスや構造体のようなデータのまとまりのこと。
--   Lua + C++の連携ではスタックを仲介し、スタックには関数やテーブルが詰めるためデータ単位,構造のやり取りに使えそう。
tbl={
    Name="Hoge",
    Age=0,
    Hometown="東京"
};


-- 1.テーブルに関数を入れる.
function Func(str)
    print(str);
end

-- ↓のように代入することで'MyFunc'を呼び出しても'Func'を呼び出した挙動をする.
MyFunc=Func;

--[補足]
--宣言と同時に代入を行う.
--※宣言と代入を同時に行う際は無名関数にする点がポイント.
MyFunc=function(str)
    print(str);
end


-- 2.クラス宣言＋オブジェクト生成するNew関数
--   テーブルはCで言うクラスや構造体に近しいものだが、
--   Cと違ってインスタンスの生成が変数単位。つまり
--   複数のインスタンスが必要でも都度同じテーブルを宣言しないといけない。
--   なのでテーブルにインスタンスを生成するメソッドを用意することで対策する。

-- 重複防止のために条件文を判定.
if (PRAGMA_ONCE == nil) then

    -- 定義済み.
    PRAGMA_ONCE = 1;

    -- ※テーブル定義は別ファイル().
    Object={
        --インスタンス生成メソッド
        New=function()
            -- 戻り値にテーブル構成を定義.
            return{
                -- メンバ変数.
                Name="member_value",
                Hash=0,
                
                -- メンバ関数.
                -- ※returnで返すテーブルの中に宣言/定義を含める点に注意.
                print=function(this)
                    print("Name:",this.Name,", Hash:",this.Hash);
                end
            };
        end
    }
end

-- Objectテーブルの宣言された別ファイルの登録指定.
-- ※拡張子まで入れてしまうと挙動が変わるらしいので注意.
--TODO:lua.exeの実行ではエラーは出ないがC++のAPI経由だと"attempt to call global 'require' (a nil value)"が
--     出てしまいコンパイルが通らない."luaopen_base"や"luaopen_libs"の効果はなかった.
--     時間がかかりそうなので一旦諦めてSKIPする.
-- require("requiretable_sample")

obj1=Object.New();
obj2=Object.New();

obj1.Hash=1;
obj2.Hash=2;

obj1.print(obj1);
--thisは省略できる.
--obj2.print();
obj2:print();