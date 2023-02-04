using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System;
using System.IO;
using System.Linq;
using gLuaFuncSample3 = System.Func<System.Action>;

public class LuaSample : MonoBehaviour
{
    LuaEnv luaEnv = new LuaEnv();
    const string LUA_RESOURCE_DIR = @"Lua/Sample/";

    enum SampleType
    {
        g_Value,
        g_Func,
        g_Table,
        g_Mapping,
        g_Instance,
        g_LoadAdvance,
    }
    [SerializeField] SampleType sampleType = SampleType.g_Value;

    #region Common

    byte[] Load(ref string filePath)
    {
        if (File.Exists(filePath))
        {
            return File.ReadAllBytes(filePath);
        }
        Debug.LogError($"load error : " + filePath);
        return null;
    }

    void DoLua(LuaEnv env, string luaFile/* .luaファイル(ディレクトリ:LUA_RESOURCE_DIR) */)
    {
        if (env == null)
        {
            Debug.LogError($"Not initialized LuaEnvironment. Arg luaFile:{luaFile}");
            return;
        }

        env.AddLoader(Load);
        env.DoString($"require '{Path.Combine(LUA_RESOURCE_DIR, luaFile)}'");
    }
    #endregion

    private void Start()
    {
        DoLua(luaEnv, GetLuaFileName(sampleType));
        Execute();
    }

    private void Update()
    {
        if (luaEnv != null && luaEnv.L != IntPtr.Zero) luaEnv.Tick();
    }

    private void OnDestroy()
    {
        if (luaEnv != null) luaEnv.Dispose();
    }

    string GetLuaFileName(SampleType type)
    {
        return type switch
        {
            SampleType.g_Value => "get_global_lua_value_toC#_sample.lua",
            SampleType.g_Func => "get_global_lua_func_toC#_sample.lua",
            SampleType.g_Table => "get_global_lua_table_toC#_sample.lua",
            SampleType.g_Mapping => "get_global_lua_mapping_toC#_sample.lua",
            SampleType.g_Instance => "create_instance_lua_fromC#_sample.lua",
            SampleType.g_LoadAdvance=>"load_advance_lua_fromC#_sample.lua",
            _ => throw new NotImplementedException(),
        };
    }

    void Execute()
    {
        switch (sampleType)
        {
            case SampleType.g_Value: sample_getLuaVal(); break;
            case SampleType.g_Func: sample_getLuaFunc(); break;
            case SampleType.g_Table: sample_getLuaTable(); break;
            case SampleType.g_Mapping:sample_getLuaMapping();  break;
            case SampleType.g_Instance: break;
            default:
                break;
        }
    }

    [ContextMenu("呼び出し")]
    void Call()
    {
        if (luaEnv != null)
        {
            luaEnv.Dispose();
            luaEnv = null;
        }
        luaEnv = new LuaEnv();
        DoLua(luaEnv, GetLuaFileName(sampleType));
        Execute();
    }

    #region C#からLuaにアクセスする

    #region Globalな値を取得
    void sample_getLuaVal()
        {
            var v = luaEnv.Global.Get<int>($"g_sample_val");
            Debug.Log($"Lua global value = {v}");
        }
    #endregion

    #region Globalなテーブルを取得
    /*
    * テーブルと同じ構造でマッピング
    * Lua側のテーブルに対応する型を持たせる
    */
    class gLuaSampleTable
    {
        public int val1;
        public string val2;
    }

    void sample_getLuaTable()
    {
        var t = luaEnv.Global.Get<gLuaSampleTable>($"g_sample_table");
        Debug.Log($"Lua global table val1:{t.val1} , val2:{t.val2}");
    }
    #endregion

    #region Globalな関数を取得
    /*
 *  XLua > GenerateCode : コードを生成する必要がある?
 */
    [CSharpCallLua]
    public delegate int gLuaFuncSample(int arg);
    [CSharpCallLua]
    public delegate Action gLuaFuncSample2();
    [CSharpCallLua]
    public delegate int gLuaFuncSample4(ref int b, ref string c);
    void sample_getLuaFunc()
    {
        // 引数・戻り値無し
        var func = luaEnv.Global.Get<Action>($"g_sample_func1");
        func.Invoke();

        // 引数・戻り値有り ( デリゲートにマッピング )
        var func2 = luaEnv.Global.Get<gLuaFuncSample>("g_sample_func2");
        var ret = func2(2);
        Debug.Log($"g_sample_func2 戻り値 = {ret}");

        // 関数を返す関数
        /*
         *  public delegate Action<型> 型名(引数)
         *  デリゲートでマッピングするやり方.
         */
        var func3map = luaEnv.Global.Get<gLuaFuncSample2>($"g_sample_func3");
        func3map.Invoke();

        /*
         *  using <型名> = System.Func<Action<引数型>>;
         *  エイリアスでマッピングするやり方.
         *  ※紹介されていたデリゲートじゃなくても出来たのでメモ代わり.
         */
        var func3 = luaEnv.Global.Get<gLuaFuncSample3>($"g_sample_func3");
        var func4 = func3();
        func4.Invoke();

        // 戻り値が複数.
        var func5 = luaEnv.Global.Get<gLuaFuncSample4>($"g_sample_func4");
        int arg1 = 0;
        string arg2 = string.Empty;
        ret = func5.Invoke(ref arg1, ref arg2);
        Debug.Log($"戻り値[1]={ret},[2]={arg1},[3]={arg2}");
    }
    #endregion

    #region Globalマッピング

    // クラス・構造体のマッピング
    // ※注意※
    // ⚠Lua側に定義する変数と変数名と型を合わせないといけない
    struct gLuaMappingSample1
    {
        public int a;
        public uint b;
        public string c;
    }

    // インターフェースのマッピング
    // "XLua >  Generate Code"をしなくても動作した？何でかよくわからない
    [CSharpCallLua]
    public interface ILuaMappingSample
    {
        int Hoge { get; set; }
        string Fuga { get; set; }
        string Foo();
    }

    void sample_getLuaMapping()
    {
        // 構造体
        var map1 = luaEnv.Global.Get<gLuaMappingSample1>($"g_sample_map1");
        Debug.Log($"マッピングサンプル1 [1]:{map1.a} , [2]:{map1.b} , [3]:{map1.c}");

        // インターフェース
        var map2 = luaEnv.Global.Get<ILuaMappingSample>($"g_sample_map2");
        Debug.Log($"マッピングサンプル2 Hoge:{map2.Hoge} , Fuga:{map2.Fuga} , Foo:{map2.Foo()}");

        // リスト(配列)
        var map3 = luaEnv.Global.Get<List<string>>($"g_sample_map3");
        Debug.Log($"マッピングサンプル3 Count:{map3.Count}{Environment.NewLine}{string.Join(Environment.NewLine, map3)}");

        // 連想配列
        /*
         * ↑のリストと同じLuaテーブルを読み込んだ場合 Key(int)の値は'1~n'順番に入る
         */
        var map4 = luaEnv.Global.Get<Dictionary<int, string>>($"g_sample_map3");
        Debug.Log($"マッピングサンプル4 Count:{map4.Count}{Environment.NewLine}{string.Join(Environment.NewLine, map4.Select(x => { return $"Key:{x.Key} Value:{x.Value}"; }))}");

        var map5 = luaEnv.Global.Get<Dictionary<string, string>>($"g_sample_map4");
        Debug.Log($"マッピングサンプル5 Count:{map5.Count}{Environment.NewLine}{string.Join(Environment.NewLine, map5.Select(x => { return $"Key:{x.Key} Value:{x.Value}"; }))}");
    }
    #endregion

    #endregion

    #region LuaからC#にアクセスする
    // C#インスタンス生成 , 
    // 型を変数に入れておく ,
    // static変数/関数 ,
    // メンバにアクセス
    // ref引数の受け渡し
    // enum
    /*
     * class 修飾子はpublicにしとかないとluaからアクセスできない点に注意.
     */
    [LuaCallCSharp]
    public class gFromLua
    {
        public static int RefMethodSample(int a, ref int b)
        {
            b += a;
            return a + 1;
        }
        public static int OutMethodSample(int a, out string str)
        {
            str = "サンプルテキスト";
            return a + 2;
        }
    }
    [LuaCallCSharp]
    public enum gEnumFromLua
    {
        One,
        Two
    }

    public class gDelegateFromLua
    {
        public static Action Method = () =>
        {
            Debug.Log($"{typeof(gDelegateFromLua).Name}:Method from C#");
        };
    }
    public class gEventFromLua
    {
        public static event Action Callback;
        public static void Call()
        {
            Callback?.Invoke();
        }
    }
    public class gVector3FromLua
    {
        public static void Log(Vector3 v3)
        {
            Debug.Log($"{v3}");
        }
    }
    #endregion

}