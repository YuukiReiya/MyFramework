--[[
    ※注意※
    ⚠Lua側に定義する変数と変数名と型を合わせないといけない
]] 
g_sample_map1={
    a=-100,
    b=37,
    c="構造体マッピングサンプル"
}

-- インターフェースのマッピングサンプル
g_sample_map2={
    Hoge=92,
    Fuga="インターフェースマッピングサンプル",
    Foo=function(self)
        return "インターフェース関数文字列"
    end
}

-- リスト(配列)のマッピングサンプル
g_sample_map3={
    "あいうえお",
    "かきくけこ",
    "さしすせそ",
    "たちつてと",
    "なにぬねの"
}

-- 連想配列(Dictionary)のマッピングサンプル
g_sample_map4={}
g_sample_map4["a"]="あいうえお"
g_sample_map4["b"]="かきくけこ"
g_sample_map4["c"]="さしすせそ"
g_sample_map4["d"]="たちつてと"
g_sample_map4["e"]="なにぬねの"

