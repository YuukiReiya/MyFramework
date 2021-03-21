using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.WSA;
using System;
using System.Linq;
using UnityEditor;
using Packages.Rider.Editor;

/*!
 *  VSのビルドイベントに仕組んだので使わない。
 *  一応使えはするので残しとく。
 */
namespace Utility
{
    /*!
     * NOTE:
     * BuildEventフォルダのSymbolicLink.batを管理者権限で実行するだけ。
     * 
     * TODO:
     * 追加で必要になったら追加していく
     */
    public class SymbolicLink
    {
        private static readonly string ProjectRootPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
        #region BAT
        private const string BatFolderName = "BuildEvent";
        private const string BatName = "SymbolicLink.bat";
        private static readonly string BatPath = Path.Combine(ProjectRootPath, BatFolderName, BatName);
        private const string PluginsName = "Plugins";
        #endregion

        private static readonly string LinkFullPath = Path.Combine(ProjectRootPath, BatFolderName, PluginsName);
        private static readonly string TargetFullPath = Path.Combine(UnityEngine.Application.dataPath, PluginsName);
        private const string FileRoutine = "File";
        private const string DirectoryRoutine = "Directory";

        [Conditional("UNITY_EDITOR")]
        public static void Create(string linkFullPath, string targetFullPath)
        {
            //  参照先チェック
            var routine = GetRoutineName(targetFullPath);
            if (routine == string.Empty)
            {
                UnityEngine.Debug.LogError("シンボリックリンクの参照先が見つかりません。");
                return;
            }

            //  リンク先チェック

            //  プロセス呼出し
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = BatPath;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.Arguments = string.Format(@"{0} {1} {2}", routine, linkFullPath, TargetFullPath);
            var hoge = p.StartInfo.Arguments.Replace(" ", " \n");
            p.StartInfo.Verb = "RunAs";

            UnityEngine.Debug.Log("Argument:" + hoge);
            UnityEngine.Debug.Log("BPath:" + BatPath);
            
            System.Diagnostics.Process.Start(p.StartInfo);
        }

        /// <summary>
        /// ファイル(ディレクトリ)を判定しバッチのルーチン名を返す
        /// </summary>
        /// <param name="path"></param>
        /// <returns>失敗時には"空文字列"を返す</returns>
        private static string GetRoutineName(string path)
        {
            if (File.Exists(path))
            {
                return FileRoutine;
            }
            if(Directory.Exists(path))
            {
                return DirectoryRoutine;
            }
            //  Not Found
            return string.Empty;
        }

        #region PLUGINS_LINK
        /// <summary>
        /// プラグインフォルダのリンクをバッチフォルダに作成
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void CreatePluginsLink()
        {
            Create(
                 LinkFullPath,
                TargetFullPath);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DeletePluginsLink()
        {
            if(!Directory.Exists(LinkFullPath))
            {
                UnityEngine.Debug.LogError("<color=red>シンボリックリンクが作成されていません。</color>\n" +
                    $"<color=red>PATH:</color>{LinkFullPath}");
                return;
            }
            Directory.Delete(LinkFullPath);
        }
#endregion
    }
}