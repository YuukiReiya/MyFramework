using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor.VersionControl;
//ビルドイベント
namespace API.Process
{
    public class ProcessBuilder : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string BatFolderName = "BuildEvent";
        private const string SubRoutinesName = "SubRoutines.bat";
        private const string PluginsFileName = "PLUGINS_PATH.txt";
        private const string PluginsFolderName = "Plugins";
        private const string EnvironmentUserVariables = "%USERNAME%";
        private const char Slash = '/';

        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLinkA(
        string lpSymlinkFileName, string lpTargetFileName, uint dwFlags);
        [DllImport("kernel32.dll")]
        static extern uint GetLastError();
        public enum LinkType : uint
        {
            File = 0x0,
            Directory = 0x1,
        }
        /// <summary>
        /// プラグインパスの入っているファイルの相対パス
        /// </summary>
        private string PluginsFilePath
        {
            get
            {
                string path = string.Empty;
                var rootDir = Directory.GetParent(Directory.GetCurrentDirectory());//.sln .csprojがある階層(Assetsの一階層上)の親ディレクトリ ← ../../Assets
                if (rootDir != null)
                {
                    path = Path.Combine(rootDir.FullName, BatFolderName, PluginsFileName);
                }
                //UNC サーバーや共有名のルートを含むルート ディレクトリの場合
                else
                {
                }
                return path;
            }
        }
        public int callbackOrder { get { return 0; } }
        private bool IsDevelopment(BuildReport report)
        {
#if UNITY_EDITOR
            return (report.summary.options & BuildOptions.Development) != 0;
#else
            return false;
#endif
        }
        //  ビルド前実行
        public void OnPreprocessBuild(BuildReport report)
        {
            if(IsDevelopment(report))
            {

            }
        }

        //ビルド後実行
        public void OnPostprocessBuild(BuildReport report)
        {
            if(IsDevelopment(report))
            {

            }
        }

        //プラグインのパスが書かれたテキストのパスを書き換える
        public void SetPluginsPath()
        {
            using (var sw = new StreamWriter(PluginsFilePath))
            {
                //  ユーザー名部分を環境変数で置換したPluginsフォルダまでのパス
                var path = Path.Combine(
                    string.Join(Path.AltDirectorySeparatorChar.ToString(),
                    Application.dataPath.
                    //  階層区切りで文字列を分割
                    Split(Path.AltDirectorySeparatorChar).
                    //  ユーザー名と一致した階層をユーザー環境変数(%USERNAME%)に置換
                    Select(str => str == Environment.UserName ? EnvironmentUserVariables : str)),
                    //  Pluginsフォルダの指定
                    PluginsFolderName);

                //Pluginsフォルダまでのパスを対象のファイルに書き込む
                sw.Write(path);
            }
        }

        public void Test()
        {
            CreateSymbolicLink("これだお", Path.Combine(Application.dataPath, "Plugins"), LinkType.Directory);
                Debug.LogError(GetLastError());
        }

        //overrap
        public void CreateSymbolicLink(string symbolicLinkName, string targetPath, LinkType type)
        {
            if (!CreateSymbolicLinkA(symbolicLinkName, targetPath, (uint)type))
            {
                Debug.LogError("<color=red>失敗</color>");
                Debug.LogError("<color=red>" + targetPath + "</color>");
            }
        }
    }
}