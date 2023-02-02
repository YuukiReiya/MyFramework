-- 参考:https://qiita.com/mod_poppo/items/ef3d8a6fe03f7f426426

--[[
    local modname=...;
    local M={};
    _G[modname] = M;
    
    print(string.format("modname=%s",modname)) -- 出力:modname=Lua/Sample/load_advance_lua_fromC#.lua
]]


--[[
    モジュール・ファイル・チャンク読み込み
]]

--package.path

-- 1. require テーブル内の関数呼び出し.
--module = require("load_sample01.lua");
mod01 = require("Lua/Sample/load_sample01.lua");
mod01:Call();

-- 2. loadfile ファイルをchunkとしてコンパイル.
mod02 = loadfile("Lua/Sample/load_sample02.lua")
mod02();

-- 3. load 文字列にチャンクを設定し渡す.
str_chunk=[[
    local modName=...;
    function module()
        print(string.format("[string_chunk]:%s",modName));
    end
    local a=114;
    print('aaaa'..a);
    module();
]]
chunk=load(str_chunk);
chunk();

-- 4. loadfile
--mod03=loadfile("Lua/Sample/load_sample03.lua")
--local f=mod03();
--f.func();
dofile("Lua/Sample/load_sample03.lua")

-------------------------------------------------------------------------------------------------------

-- Luaをモジュールではなく単純に実行するには'dofile'関数を使う.
-- [処理フロー]
-- 1.ファイルを読み取り、バイトコードへコンパイル.
-- 2.コンパイルしたバイトコードの実行.
-- ※Luaではコンパイルと実行は別々に行うことができる.
--   また、コンパイル結果は普通の関数.
function dofile(fileName)
    
    local chunk,err = loadfile(fileName) -- loadfile:ファイルに対してコンパイルのみを行う.

    -- 正常に読み込めた.
    if chunk ~= nil then 
        -- 実行.
        chunk()
    -- エラー.
    else
        return nil,err
    end
end


-- コードをファイル以外で読み込む場合(例:文字列)、'loadfile'の代わりに'load'(Lua5.2以降) or 'loadstring'(Lua5.1)を使う.
-- 'eval'関数(文字列のその場コンパイル&実行)の代替コード例.

-- [Lua5.2以降]
--assert(load("local a = 1 + 1;print(a)"))()

-- [Lua5.1]
--assert(loadstring("local a = 1 + 1;print(a)"))()

-------------------------------------------------------------------------------------------------------
-- チャンク.
-------------------------------------------------------------------------------------------------------
-- Luaのソースコードの一塊(ファイルや文字列)、またはそれらコンパイル結果の関数のことを"チャンク"と呼ぶ.
-- トップレベルの'local'で宣言した変数のスコープは1つのチャンクで完結するため、異なるチャンクの間では共有されない。

--[[
    例：Luaの対話環境（REPL）においては1行4が1つのチャンクに相当する。
    　　そのため、先の行で定義したローカル変数は後の行で参照できない。

    Luaの対話環境)
    > local a = 123 -- 先の行で定義したローカル変数は
    > print(a) -- 後の行で参照できない
    nil
]]

--[[
    例：LuaTeXの \directlua の中で定義したローカル変数は後の \directlua から参照できない。
    　　1つの \directlua 呼び出しが1つのチャンクに相当するからである。
    \directlua{local a = 123; tex.print(tostring(a))} % --> 123
    \directlua{tex.print(tostring(a))} % --> nil
    \bye
]]

-- チャンクの引数と返り値.
-- チャンクの引数は可変長引数.
--[[
    コード例)
        local arg1, arg2, arg3 = ...
        print(arg1, arg2, arg3)
        local args_table = table.pack(...)
        print("Number of arguments: ", #args_table)

    出力例)
        $ lua hoge.lua foo bar
        foo bar nil
        Number of arguments:    2
        $ lua hoge.lua a b c d
        a   b   c
        Number of arguments:    4
]]