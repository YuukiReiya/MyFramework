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

function print_test(str)
    print(str);
end

-- コルーチン呼び出しのテスト.
function step()
coroutine.yield("そこは広場だった。")
coroutine.yield("小さな滑り台があった。")
coroutine.yield("昔ここで良く遊んだことを思い出した。")
end

----------------------------------------------------------
-- テーブル
----------------------------------------------------------
-- ① テーブルに関数を入れる
tbl = {
    Name = "Hoge",
    Age = 20,
    Hometown = "北海道",
    TblFunc = function(str)
        print("tbl > "+str);
    end
};

-- ② クラス宣言＋オブジェクト生成をするNew関数
-- 引用:「インスタンスを2つ作れば2つ分の個別データになりますよね。Lua側のテーブルでも同じことをしようと思ったら、テーブルをその度に作る必要があるわけです。ですから、「テーブルを作る関数」をLua側に設定する必要があります。」
-- どういう意味？ ＝〉C言語で捉えてたけどLuaでの書き方ってこと？無気がする。。。確かに関数ないと同じテーブル複数作らないと定義できないかも。
-- 呼び出すLua_State変えればインスタンスも別で立ちそうだけどそれじゃダメなの？
-- 一つのLua_Stateで完結させたい場合なら…って考えたけど、Lua入門初心者だから現状での影響など分からず最適解も出ない。
-- ひとまずその体があってると仮定して読み進める。
-- ※GoFでいうFactor？
tblFactor={
    New=function()
        return {
            Name = "def",
            Age = 0,
            Hometown = "def"
        };
    end
};
obj1 = tblFactor.New();
obj2 = tblFactor.New();

-- ↑ここまででC++でいうクラスっぽく書けるよねって話。
-- メンバ変数を参照するクラスメソッドの定義法に関して。

--[[
class={
    New=function()
        return {
            Name = "def",
            Age = 0,
            Hometown = "def",

            print = function()
                print(Name,Age,Hometown);
            end
        };
    end
};
obj3 = class.New();
-- 「print関数を新しく定義して、テーブル内の変数を使っているように見えるのですが、
-- これは残念ですがうまく動きません。
-- エラーにはならないのですが、print関数内で「Name, Age, Hometown」という3つの変数が空宣言されたとみなされるので、
-- 出力結果は全部「nil」になってしまいます。」
obj3.print(); -- 要するに関数内のローカル変数としてメンバとは別に宣言されてしまうためダメらしい…多分解釈的にも合ってそう。
]]

-- じゃあどうやってメンバにアクセスするのか.
-- → 「thisポインタ」使え。
class = {
    New = function()
        return{
            Name = "def",
            Age = 0,
            Hometown = "def",
            
            print = function(this)
                print(this.Name,this.Age,this.Hometown);
            end
        };
    end
};
obj3 = class.New();
obj3.print(obj3);

-- ③クラスっぽい部分だけを外部ファイルに置いて呼び出す
-- 「クラスっぽいことはできるようになったわけですが、
-- 毎回これをLuaファイルに書くのはもちろんシンドイわけです。
-- 外部ファイルにこの宣言部分だけを記述して、別のLuaファイルで
-- はそれを呼び出して使えるようにすると便利です。
-- C言語でいう「#include」が欲しいというわけです。」
-- >分からないことはない、いや分かる。

-- Luaには「require」というC言語の#includeに相当するライブラリ関数が
-- あります。先程のクラス宣言部分をClassData.luaファイルに保存した
-- として、使う側では次のようにrequire関数でそれを宣言します：

--[[
    -- 使用例
    require("ClassData"); -- 別フォルダに入れている時は、そこまでの相対パスを使って指定 例）"require("code/ClassData")"

    instance = Data.New();
    instance:print();
]]

--[[
    ※ファイル名を指定するのですが「拡張子をつけない」事に注意して下さい。
    付けると別挙動になってしまいます。

    ただし、同じLuaステートで同じファイルがrequireされる度にコードが走ります。
    これは大量のLuaファイルを扱うときに死活問題となります。
    C++の場合は、

    C言語での重複防止
```cpp
    #ifndef IKD_OX_DATA_H
    #define IKD_OX_DATA_H

    ...

    #endif
```


    requireの重複防止(ClassData.lua)
```lua
if (IKD_OX_LUA_DATA_H == nil) {
    IKD_OX_LUA_DATA_H = 1

    Data = {
        ...
    };
}
```
という対処が必要になるかもしれません。
> まだ「同じLuaステートで同じファイルがrequireされる度にコードが走ります」これが必要になるケースが
　分からないので必要性がわからない…
　追々かなって思ってる。
]]

--
--　

----------------------------------------------------------
--  LuaからC言語の関数を呼び出す
----------------------------------------------------------

--------------------
-- フロー
--------------------
--[[
  ① C言語側でLuaステートに関数を教えるのが第1歩


]]

--① C言語側でLuaステートに関数を教えるのが第1歩
--[[
```lua
start = 10.0f;
goul = 30.0f;

--補間しよう
t = 0.0;
while t <= 1.0 do
    print(Linear(start, end, t));
    t = t + 0.1;
end
```
上記のLuaはLinear関数が存在しないためエラー.

解消方法としては以下
① require関数を使ってLiniear関数が定義されているファイルを読み込む
② C言語側で関数を登録する
本件は②のやり方.

※Luaで読み込めるC言語の関数型は下記の1種類だけ※
戻り値がint型の引数lua_State*型 - "グルー関数(GlueFunction glue:くっつける)"と呼ぶ
```cpp
int <メソッド名>(lua_State*){return 0;}
```

```cpp
// 呼び出し
lua_State* = luaL_newstate();
// lua_Stateに関数"FuncC"を登録
lua_register(L,"FuncC",&FuncC);
int FuncC(lua_State*L){ printf("hoge"); }
```

以下は引数にfloat型三つ、戻り値もfloat型で返したい場合の関数を登録するための方法。

lua側の呼び出し想定 - 線形補完関数
```lua
Linear(10,30,0.3);
```

C(++)側の定義
```cpp
int Linear(lua_State*L)
{
    // 引数
    float start = (float)lua_tonumber(L,1); // Arg1
    float end = (float)lua_tonumber(L,2); // Arg2
    float t = (float)lua_tonumber(L,3); // Arg3

    // 補完計算部.
    float res = (1.0 - t) * start + t * end;

    // スタック削除.
    lua_pop(L,lua_gettop(L));

    // スタックに戻り値を積む.
    lua_pushnumber(L,res);

    // 関数の戻り値としてスタックに積んだ(lua側に定義したい関数の)戻り値の数を返すようにする
    return 1;
}
```

]]

function entry()

-- 「Linear」メソッドがC++側で定義されていることを想定して呼び出す。
-- Arg1:開始地点
-- Arg2:終了地点
-- Arg3:現在地点
return Linear(10,30,0.3);
   --result = Linear(1,10,0.2);
   --print("LuaResult:",result);
--return result;

-- ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑
-- C言語側:int Linear(lua_State*)の引数と一致しない.
-- Linear();



end

function entry2()

-- GlueFunction(引数無し)C言語定義の関数呼び出しテスト
GlueFuncTest();

end