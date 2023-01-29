-- C#のインスタンスを生成
local go=CS.UnityEngine.GameObject()
print("'New Game Object'という名でインスタンス作ったよ")

-- オーバーロード対応
local go2=CS.UnityEngine.GameObject("createInstanceFromLua")
print("createInstanceFromLuaという名でインスタンス作ったよ")

-- 短縮形( 変数格納 )
local GameObject = CS.UnityEngine.GameObject
local go3 = GameObject()
local go3 = GameObject("GameObject4fromLua")

-- 静的(static)変数、静的(static)関数
local UnityEngine=CS.UnityEngine
deltaTime=UnityEngine.Time.deltaTime

-- 表示してみたら丸められてたので二つ用意した。
-- ※精度は下の方が高い.
print(string.format("deltaTime:%f",deltaTime))
print(deltaTime)

-- メンバにアクセスする
local Vector3 = UnityEngine.Vector3

-- メンバ関数にアクセスしてみる.
go:SetActive(false)


-- メンバ変数にアクセスしてみる.
go2.transform.localScale=Vector3.zero
go3.transform.position = Vector3(100,89,43) -- new Vector3 : 成功

-- エイリアス
local alias=CS.test.FromLua

-- ref引数の受け渡し
local b = 1000
result,b = alias.RefMethodSample(123,b)
print(string.format("result:%d,ref b:%d",result,b))

-- out引数の受け渡し
local c = "文字列: from .lua"
b=alias.OutMethodSample(9,c)
print(string.format("result:%d,ref str:%s",b,c))

-- enumの参照
-- ①直指定
local enum1 = CS.test.EnumFromLua.One

-- ②値からキャスト
local enum2 = CS.test.EnumFromLua.__CastFrom(1)

-- ③名前からキャスト
local enum3 = CS.test.EnumFromLua.__CastFrom("Two")

print(string.format("enum1:%s enum2:%s enum3:%s",enum1,enum2,enum3))

-- Delegateの操作

function delegate()
    print("delegate from lua")
end

CS.test.DelegateFromLua.Method = CS.test.DelegateFromLua.Method + delegate
CS.test.DelegateFromLua.Method()
CS.test.DelegateFromLua.Method = CS.test.DelegateFromLua.Method - delegate

-- event操作
function event()
    print("event from lua")
end

CS.test.EventFromLua.Callback("+",event)
CS.test.EventFromLua.Call()
CS.test.EventFromLua.Callback("-",event)

-- 複雑な型を直接引数に渡す (UnityEngine.Vector3)
CS.test.Vector3FromLua.Log({x=1,y=5,z=10})

-- 型の取得
local type = typeof(CS.UnityEngine.GameObject)
print(string.format("typeof:%s",type)) -- UnityEngine.GameObject : [数字] ←で表示される？なんの数列かはよくわからない。

--[[
    【その他】
    * LuaはC#より型の種類が少ないのでint型とfloat型のオーバーロードなどは区別できない ** どちらか一方だけが呼び出される
    * デフォルト引数に対応している
    * 可変長引数に対応している
    * 拡張メソッドに対応している
    * Genericなメソッドに対応していないので非ジェネリックなメソッドでラップする必要がある
]]

--[[
    アトリビュートについて。

    > CSharpCallLua
    このアトリビュートは以下のケースで必要です。
    　* Luaの関数をC#のデリゲートにマッピングする際にデリゲートにつける。
    　* LuaのテーブルをC#のインターフェースにマッピングする際にインターフェースにつける
　　いずれもこのアトリビュートをつけた上でXLua > GenerateCodeを行うことで必要なコード(インターフェース実装など)が自動生成される

    > LuaCallCSharp
    このアトリビュートは、LuaからアクセスされるC#のクラスや構造体につけることで必要なコードが自動生成されます。
    つけなくても動きますが、その場合にはリフレクションを使ってアクセスするため遅くなります。

    > ReflectionUse
    上述の通り、LuaCallCSharpをつけない場合にはリフレクションで呼ばれることになります。
    ただしリフレクションはIL2PPビルドでコードストリッピングされる可能性があります。
    これを防ぐために、コード生成したくないがリフレクションでアクセスしたい場合には、
    LuaCallCSharpの代わりにReflectionUseをつけます。
    これにより対象の型がlink.xmlに書き出されてストリッピングされなくなります。
    LuaからアクセスするC#のコードにはLuaCallCSharpかReflectionUseのどちらからが必ずついているべきです。

    > staticメソッドで指定することもできる
    これらのアトリビュートは上述のように直接付けることもできますが、
    以下のようにアトリビュートをつけたい型を返すstaticメソッドを使うこともできます
    ```cs
    using System;
    using System.Collections.Generic;
    using XLua;

    public static class ExampleEditor
    {
        [LuaCallCSharp]
        public static List<Type> LuaCallCsList = new List<Type>
        {
            // ExampleクラスにLuaCallCSharpアトリビュートを指定したのと同じ効果
            typeof(Example),
        };
    }
    ```
    このstaticメソッドを定義するクラスもstaticにしておく必要がある点に注意してください。
    またこのスクリプトはEditorフォルダに配置することが推奨されています。

    > DoNotGen
    LuaCallCSharpアトリビュートをつけているクラスのメンバの一部をコード生成したくない場合にはDoNotGenアトリビュートを使います。
    これで指定したメソッド、フィールド、プロパティはコード生成の対象外となり、リフレクションでアクセスされることになります。
    指定は以下のように行います。
    ```cs
    public static class ExampleEditor
    {
        [DoNotGen] 
        public static Dictionary<Type, List<string>> DoNotGenList = new Dictionary<Type, List<string>>()
        {
            // ExampleクラスのtestIntフィールドをコード生成対象外とする
            {typeof(Example), new List<string>() {"testInt"}}
        };
    }
    ```
    このstaticメソッドを定義するクラスもstaticにしておく必要がある点に注意してください。
    またこのスクリプトはEditorフォルダに配置することが推奨されています。

    > BlackList
    LuaCallCSharpアトリビュートをつけているクラスのメンバの一部について、リフレクションでもアクセスさせたくないときにはBlackListを使います。
    これで指定したメソッド、フィールド、プロパティにはアクセスできなくなります（例えばフィールドだとnullが返ってきます）。
    ただしそもそも対象のクラスにLuaCallCSharpアトリビュートがついていない場合には効果がないので注意してください。
    指定は以下のように行います。
    ```cs
    public static class ExampleEditor
    {
        [BlackList]
        public static List<List<string>> BlackList = new List<List<string>> {
            // ExampleクラスのtestIntフィールドにアクセスできなくする
            new List<string>{typeof(Example).FullName, "testInt"}
        };
    }
    ```
    このstaticメソッドを定義するクラスもstaticにしておく必要がある点に注意してください。
    またこのスクリプトはEditorフォルダに配置することが推奨されています。

    > リフレクションによるアクセスについて
    さてxLuaではコード生成されていない、かつストリッピングされていない型には、
    リフレクション を介してLuaからアクセスできるということになります。
    このリフレクションによるアクセスは制限することはできませんが、
    NOT_GEN_WARNINGマクロを有効にしておくことでリフレクションによるアクセスが行われたときに警告を出すことができます。
]]