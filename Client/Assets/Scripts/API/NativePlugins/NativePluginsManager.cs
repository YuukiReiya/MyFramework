//参考:https://qiita.com/tan-y/items/9cf3d233df1a379802b2
using System;
using System.Runtime.InteropServices;

/*NOTE:
 * DllImport属性を使ってしまうとDLLを更新するたびにUnityの再起動が必要になるので
 * WindowsのAPIである"LoadLibrary"と"FreeLibrary"を使って明示的に読み込みと開放を行う。
 * これでDLLをビルドするたびに再起動が必要なくなる。
 * 
 * TODO:
 * IUnityInterfacesの取得に対応していない。
 * 必要になったら呼べるように改修するが、レンダリング周りやGPUに関してはUnity側のものを
 * 使ったほうがいい気がする…
 */
namespace API.NativePlugin
{

    public class NativePluginsManager:IDisposable
    {
        private IntPtr dll = IntPtr.Zero;
        public NativePluginsManager(string dllPath)
        {
            dll = LoadLibrary(dllPath);
        }

        public void Dispose()
        {
            if (dll != IntPtr.Zero)
            {
                FreeLibrary(dll);
                dll = IntPtr.Zero;
            }
        }
        public T GetDelegate<T>(string funcName)
        {
            if (dll != IntPtr.Zero)
            {
                return Marshal.GetDelegateForFunctionPointer<T>(GetProcAddress(dll, funcName));
            }
            return default(T);
        }

        /* NOTE:
         * "kernel32"のDLLはDllImportしているので無いとは思うがパスを変えたり削除したりしようとエラー(警告)
         * が出るはず。
         */
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    }

}