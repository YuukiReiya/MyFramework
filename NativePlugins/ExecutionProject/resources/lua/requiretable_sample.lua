-- Lua用のテーブルを外部ファイルに定義する.
-- サンプル用のLua

--TODO:C++側からrequireを呼ぼうとするとエラーになる。
--対策思いつかないのでしばらく放置.

-- 宣言用のテーブル
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
                --print("Name:",this.Name,string.format("Hash:%d",this.Hash));
                print("Name:",this.Name,", Hash:",this.Hash);
            end
        };
    end
};
