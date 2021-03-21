#define hoge
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using API.NativePlugin;
using System.Runtime.InteropServices;
namespace Sample
{
    public class NativePluginSample : MonoBehaviour
    {
        /* NOTE:
         * 関数内でdllのインスタンスを定義しているがインポートは一度だけで不要になるまで保持していた方が賢い。
         * 下記の関数はあくまで確認用。それようののインポーターを作る必要がありそう…。
         * ※ランタイム実行の場合コンテキストメニューから呼び出せないのでDLLの関数をコンテキストメニューで
         * 呼び出す際に工夫が必要っぽい。
         */
        #region DLL_IMPORT
        /*!
         *  ホットリロードをテストするための関数
         */
        delegate int THotReloadTest();
        private static int HotReloadTest()
        {
            using (var dll = new NativePluginsManager("Assets/Plugins/UnityDLLforCPP.dll"))
            {
                var tHotReloadTest = dll.GetDelegate<THotReloadTest>("HotReloadTest");
                return tHotReloadTest();
            }
            throw new Exception();
        }

        /*! 
         * ただ値渡しして足し算の結果を返すだけの関数
         * <C++定義側>
         * int Add(int a,int b){ return a+b; }
         */
        delegate int TAdd(int a, int b);
        private static int Add(int a, int b)
        {
            using (var dll = new NativePluginsManager("Assets/Plugins/UnityDLLforCPP.dll"))
            {
                var tAdd = dll.GetDelegate<TAdd>("Add");
                return tAdd(a, b);
            }
            throw new Exception();
        }

        /*
         * 引数を参照渡しして足し算した第一引数を返す関数
         * <C++定義側>
         * int Add(int* a,int b)
         * { 
         *      *a+=b;
         *      return *a; 
         *  }
         */
        delegate int TAddPtr(ref int a, int b);
        private static int AddPtr(ref int a, int b)
        {
            using (var dll = new NativePluginsManager("Assets/Plugins/UnityDLLforCPP.dll"))
            {
                var tAddPtr = dll.GetDelegate<TAddPtr>("AddPtr");
                return tAddPtr(ref a, b);
            }
            throw new Exception();
        }

        /*!
         * <C++定義側>
         * int Add(int* ptr,int size,int add)
         * { 
         *      for(int i=0;i<size;++i)
         *      {
         *          ptr[i]+=add;
         *      }
         *      return ptr; 
         *  }
         */
        delegate IntPtr TAddArray(ref int ptr, int size, int add);
        /*!
         * 引数のref intは正直意味が無い
         */
        private static IntPtr AddArray(ref int ptr, int size, int add)
        {
            using (var dll = new NativePluginsManager("Assets/Plugins/UnityDLLforCPP.dll"))
            {
                var tAddArray = dll.GetDelegate<TAddArray>("AddArray");
                return tAddArray(ref ptr, size, add);
            }
            throw new Exception();
        }
        #endregion

        /// <summary>
        /// UnityDLLforCPPで定義した関数の呼び出しテスト関数
        /// </summary>
        [ContextMenu("呼び出しテスト")]
        private void Test()
        {
            //HotReloadTestResult();    //ホットリロード確認用
            //Test1();    //  値渡し
            //Test2();    //  参照渡し
            //Test3();    //  配列のポインタを返す
        }

        /// <summary>
        /// DLLのホットリロードが可能かどうか確認するために定数を返す関数を呼び出してみる
        /// </summary>
        private void HotReloadTestResult()
        {
            Debug.Log($"HotReloadTest = {HotReloadTest()}");
        }

        /// <summary>
        /// 値渡し
        /// </summary>
        private void Test1()
        {
            int a = 1, b = 2;
            int ret = Add(a, b);
            Debug.Log($"パラメーター\na={a}\nb={b}\nret={ret}");
        }

        /// <summary>
        /// 参照渡しを引数にする関数
        /// </summary>
        private void Test2()
        {
            int a = 1, b = 2;
            int ret = AddPtr(ref a, b);
            Debug.Log($"パラメーター\na={a}\nb={b}\nret={ret}");
        }

        /// <summary>
        /// 配列(先頭ptr)を返す関数
        /// NOTE:
        /// ※配列(先頭ptr)を渡したところで配列自体のサイズがわかるわけではないので別途サイズを渡す必要がある
        /// </summary>
        private void Test3()
        {
            //  元の値
            int[] a = { 0, 1, 2, 3 };

            //  加算値
            int b = 5;

            //  IntPtrに配列の先頭ポインタを格納
            IntPtr ptr = AddArray(ref a[0], a.Length, b);

            //  関数の結果を格納する
            int[] result = new int[a.Length];// nullは使えず、元の値と同じだけバッファサイズを確保する必要がある。

            //  配列(result)に関数の処理結果(先頭ポインタ)をコピー
            Marshal.Copy(ptr, result, 0, result.Length);

            string ret = $"配列aの要素に{b}を加算\n";
            foreach (var i in result)
            {
                ret += i + "\n";
            }
            string _a = $"配列a\n";
            foreach (var i in a)
            {
                _a += i + "\n";
            }
            Debug.Log($"{ret}");
            Debug.Log($"{_a}");

            a[0] = 99;
            //  出力結果:result[0]=5 << 変化なし(ポインタ/アドレスがコピーされているわけでは無い)
            //  つまり、a[0]とresult[0]は別のアドレスが割り当てられている。← resultに対してnewしてたので当然といえば当然。
            //  ※↑のためおそらくGCが入ると思われるのでメモリリークはしない
            Debug.Log($"a[0]=99の代入結果\na[0]={a[0]}\nresult={result[0]}");
        }
    }
}