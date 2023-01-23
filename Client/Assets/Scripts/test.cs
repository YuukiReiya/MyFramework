using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System;
using System.IO;
using System.Linq ;
using LuaFuncSample3=System.Func<System.Action>;

public class test : MonoBehaviour
{
    LuaEnv luaEnv = new LuaEnv();
    const string LUA_RESOURCE_DIR = @"AssetBundles/";

    byte[] Load(ref string filePath)
    {
        if (File.Exists(filePath))
        {
            return File.ReadAllBytes(filePath);
        }
        Debug.LogError($"load error : " + filePath);
        return null;
    }

    void StartLua(string luaFile/* .luaファイル(ディレクトリ:LUA_RESOURCE_DIR) */)
    {
        if (luaEnv == null) 
        {
            Debug.LogError($"Not initialized LuaEnvironment. Arg luaFile:{luaFile}");
            return; 
        }

        luaEnv.AddLoader(Load);
        luaEnv.DoString($"require '{Path.Combine(LUA_RESOURCE_DIR, luaFile)}'");
    }

    #region MyRegion
#if true_
    // Start is called before the first frame update
    void Start()
    {
        luaEnv.DoString("CS.UnityEngine.Debug.Log('hello world')");
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        luaEnv.Dispose();
    }

    [ContextMenu("sample")]
    void sample_()
    {
        luaEnv.DoString("CS.UnityEngine.Debug.Log('hello world')");
        luaEnv.Dispose();
    }
#endif
    #endregion

    #region MyRegion
#if false
    private LuaEnv _luaenv;

    void Start()
    {
        // LuaEnvのインスタンスを生成
        // これはグローバルなものを一つだけ生成することが推奨
        _luaenv = new LuaEnv();

        // 文字列で定義したLuaスクリプトを実行
        _luaenv.DoString("CS.UnityEngine.Debug.Log('Test')");
    }

    private void Update()
    {
        _luaenv.Tick(); // GC？
    }

    private void OnDestroy()
    {
        _luaenv.Dispose();
    }
#endif
    #endregion


    /*
     * さてxLuaには公式に推奨されているスクリプトの読み込み方法があります。
     * まず文字列を直接読み込む方法は非推奨です。
     * 基本的にファイルから読み込みます。
     * またファイルは個別に読み込むのではなく、main.luaのようなエントリポイントになるスクリプトファイルを用意して、
     * そこからまとめて関連する他のスクリプトファイルを読み込むようにします。
     * 例として、sub.luaをmain.luaから読み込んでみます。
     * まずsub.lua.txtを用意してResourcesフォルダに入れておきます。
     */
    #region ロードサンプル
#if false
    LuaEnv luaEnv = new LuaEnv();
    private void Start()
    {
        luaEnv.AddLoader(Load);
        //luaEnv.DoString("require 'Assets/Resources/test1.lua'");
        luaEnv.DoString("require 'AssetBundles/test2.lua'");
    }
#endif
    #endregion

    #region C#からLuaにアクセスする

    #region Globalな値を取得
#if true_
    private void Start()
    {
        GetLuaValSample();
    }
    void GetLuaValSample()
    {
        luaEnv.AddLoader(Load);
        luaEnv.DoString("require 'AssetBundles/get_global_lua_value_toC#_sample.lua'");
        var v = luaEnv.Global.Get<int>($"g_sample_val");
        Debug.Log($"Lua global value = {v}");
    }
#endif
    #endregion

    #region Globalなテーブルを取得
#if true_
    /*
     * テーブルと同じ構造でマッピング
     * Lua側のテーブルに対応する型を持たせる
     */
    class LuaTable
    {
        public int val1;
        public string val2;
    }

    void GetLuaTableSample()
    {
        luaEnv.AddLoader(Load);
        luaEnv.DoString("require 'AssetBundles/get_global_lua_table_toC#_sample.lua'");
        var t = luaEnv.Global.Get<LuaTable>($"g_sample_table");
        Debug.Log($"Lua global table val1:{t.val1} , val2:{t.val2}");
    }
    private void Start()
    {
        GetLuaTableSample();
    }
#endif
    #endregion

    #region Globalな関数を取得
#if true_
    /*
     *  XLua > GenerateCode : コードを生成する必要がある?
     */
    [CSharpCallLua]
    public delegate int LuaFuncSample(int arg);
    [CSharpCallLua]
    public delegate Action LuaFuncSample2();
    [CSharpCallLua]
    public delegate int LuaFuncSample4(ref int b,ref string c);
    void GetLuaFuncSample()
    {
        luaEnv.AddLoader(Load);
        // 引数・戻り値無し
        luaEnv.DoString("require 'AssetBundles/get_global_lua_func_toC#_sample.lua'");
        var func = luaEnv.Global.Get<Action>($"g_sample_func1");
        func.Invoke();

        // 引数・戻り値有り ( デリゲートにマッピング )
        var func2 = luaEnv.Global.Get<LuaFuncSample>("g_sample_func2");
        var ret = func2(2);
        Debug.Log($"g_sample_func2 戻り値 = {ret}");

        // 関数を返す関数
        /*
         *  public delegate Action<型> 型名(引数)
         *  デリゲートでマッピングするやり方.
         */
        //var func3 = luaEnv.Global.Get<LuaFuncSample2>($"g_sample_func3");
        //var func4 = func3.Invoke();

        /*
         *  using <型名> = System.Func<Action<引数型>>;
         *  エイリアスでマッピングするやり方.
         *  ※紹介されていたデリゲートじゃなくても出来たのでメモ代わり.
         */
        var func3 = luaEnv.Global.Get<LuaFuncSample3>($"g_sample_func3");
        var func4 = func3();
        func4.Invoke();

        // 戻り値が複数.
        var func5 = luaEnv.Global.Get<LuaFuncSample4>($"g_sample_func4");
        int arg1 = 0;
        string arg2 = string.Empty;
        ret = func5.Invoke(ref arg1, ref arg2);
        Debug.Log($"戻り値[1]={ret},[2]={arg1},[3]={arg2}");
    }
    private void Start()
    {
        GetLuaFuncSample();
    }
#endif
    #endregion

    #region Globalマッピング
#if true_
    // クラス・構造体のマッピング
    // ※注意※
    // ⚠Lua側に定義する変数と変数名と型を合わせないといけない
    struct LuaMappingSample1
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

    void GetLuaMappingSample()
    {
        StartLua($"get_global_lua_mapping_toC#_sample.lua");

        // 構造体
        var map1 = luaEnv.Global.Get<LuaMappingSample1>($"g_sample_map1");
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

    private void Start()
    {
        GetLuaMappingSample();
    }
#endif
    #endregion

    #endregion

    #region LuaからC#にアクセスする
    // C#インスタンス生成 , 
    // 型を変数に入れておく ,
    // static変数/関数 ,
    // メンバにアクセス
    // ref引数の受け渡し
    // enum
#if true
    /*
     * class 修飾子はpublicにしとかないとluaからアクセスできない点に注意.
     */
    [LuaCallCSharp]
    public class FromLua
    {
        public static int RefMethodSample(int a,ref int b)
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
    public enum EnumFromLua
    {
        One,
        Two
    }

    public class DelegateFromLua
    {
        public static Action Method = () =>
        {
            Debug.Log($"{typeof(DelegateFromLua).Name}:Method from C#");
        };
    }

    public class EventFromLua
    {
        public static event Action Callback;
        public static void Call()
        {
            Callback?.Invoke();
        }
    }

    public class Vector3FromLua
    {
        public static void Log(Vector3 v3)
        {
            Debug.Log($"{v3}");
        }
    }

    void Execute()
    {
        StartLua($"create_instance_lua_fromC#_sample.lua");
    }
    private void Start()
    {
        Execute();
    }
#endif

    #endregion
}
