﻿using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Editor.Expansion
{
#if UNITY_EDITOR
    //[InitializeOnLoad]
    public class AssetPostprocessorImpl : AssetPostprocessor
    {
        public const string RootDirectory = "Assets";

        /// <summary>
        /// インポートアセットに対して行う処理.
        /// </summary>
        private static IAssetPostprocess[] ImportPostprocesses =
        {
            new ScriptEncoder(),
        };

        /// <summary>
        /// 削除アセットに対して行う処理.
        /// </summary>
        private static IAssetPostprocess[] DeletePostprocesses =
        {

        };

        /// <summary>
        /// 移動元アセットに対して行う処理.
        /// </summary>
        private static IAssetPostprocess[] MoveSourcePostprocesses =
        {

        };

        /// <summary>
        /// 移動先アセットに対して行う処理.
        /// </summary>
        private static IAssetPostprocess[] MoveDestinationPostprocesses =
        {

        };

        private static IAssetAllPostprocess[] AllPostprocesses =
        {
            new FolderKeep(),
        };

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] moveDestinationAssets, string[] moveSourceAssetPaths)
        {
            // インポートアセット
            foreach (var asset in importedAssets)
            {
                ForEachProcess(asset, ImportPostprocesses);
            }

            // デリートアセット
            foreach (var asset in deletedAssets)
            {
                ForEachProcess(asset, DeletePostprocesses);
            }

            // 移動先アセット
            foreach (var asset in moveDestinationAssets)
            {
                ForEachProcess(asset, MoveDestinationPostprocesses);
            }

            // 移動元アセット
            foreach (var asset in moveSourceAssetPaths)
            {
                ForEachProcess(asset, MoveSourcePostprocesses);
            }

            // 全アセットのPost後
            foreach (var process in AllPostprocesses)
            {
                process.Execute();
            }

            //2019.4以降呼ぶ必要なくなった。
            AssetDatabase.ReleaseCachedFileHandles();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// memo.
        /// ToListで変換しないとForEach(メソッドの方)使えず、ゴリ押しで実装するとネスト増えるので関数分け。
        private static void ForEachProcess(string path, IAssetPostprocess[] processes)
        {
            foreach(var process in processes)
            {
                process.Execute(path);
            }
        }

        public static bool IsValidRootDirectory(string path)
        {
            return RootDirectory == GetRootParentName(path);
        }

        /// <summary>
        /// パス内の一番上の親ディレクトリ名を取得する
        /// <br>NOTE</br>
        /// <br>OnPostprocessAllAssetsでAssetsに含まれていないPackagesフォルダが対象になってしまうことが何度かあったため対策.</br>
        /// <br>※必ず呼び出されているわけでもないので謎。</br>
        /// <br>恐らく2019.4以降で入った新しいアセットパイプラインが原因。←v1だと発生しない。</br>
        /// <br>https://docs.unity3d.com/ja/2019.4/Manual/UpgradeGuide2019LTS.html</br><br></br>
        /// <br>Ex)</br>
        /// <br>Packages/com.unity.sysroot/Editor/AssemblyInfo.cs</br>
        /// <br>Packages/com.unity.sysroot.linux-x86_64/Editor/Unity.Sysroot.cs など</br>
        /// </summary>
        /// <param name="path"></param>
        public static string GetRootParentName(string path)
        {
            var result = new StringBuilder(path);

            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] ==
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                Path.AltDirectorySeparatorChar  //Windows: '/'
#else
                Path.DirectorySeparatorChar //他のOS: '\'
#endif
                )
                {
                    return result.Remove(i, result.Length - i).ToString();
                }
            }
            return string.Empty;
        }
    }
#endif
}