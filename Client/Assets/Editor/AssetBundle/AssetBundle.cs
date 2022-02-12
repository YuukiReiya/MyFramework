using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Editor.Expansion
{
    public class AssetBundle
    {
        private static string DefaultPath
        {
            get
            {
                // Assets/../
                return Path.Combine(
                    Directory.GetParent(Application.dataPath).FullName,
                    @"AssetBundles");
            }
        }

        [MenuItem("AssetBundle/簡易ビルド")]
        public static void NormalBuild()
        {
            Build(string.Empty, BuildAssetBundleOptions.None);
        }
        public static void Build(string directory, AssetBundleBuild[] assetbundles, BuildAssetBundleOptions option, BuildTarget platform = BuildTarget.NoTarget)
        {
            if (string.IsNullOrEmpty(directory))
            {
                directory = DefaultPath;
                if (!Directory.Exists(DefaultPath))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            if (!IsBuildable(directory)) return;

            var message = $"以下の設定でアセットバンドルをビルドします.{Environment.NewLine}出力先:{Path.GetFullPath(directory)}{Environment.NewLine}オプション:{option}{Environment.NewLine}プラットフォーム:{platform}";

            if (EditorUtility.DisplayDialog($"アセットバンドルビルド", message, "はい", "いいえ"))
            {
                BuildPipeline.BuildAssetBundles(directory, assetbundles, option, platform);
            }
        }

        public static void Build(string directory, BuildAssetBundleOptions option, BuildTarget platform = BuildTarget.NoTarget)
        {
            if (string.IsNullOrEmpty(directory))
            {
                directory = DefaultPath;
                if (!Directory.Exists(DefaultPath))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            if (!IsBuildable(directory)) return;

            var message = $"以下の設定でアセットバンドルをビルドします.{Environment.NewLine}出力先:{Path.GetFullPath(directory)}{Environment.NewLine}オプション:{option}{Environment.NewLine}プラットフォーム:{platform}";

            if (EditorUtility.DisplayDialog($"アセットバンドルビルド", message, "はい", "いいえ"))
            {
                BuildPipeline.BuildAssetBundles(directory, option, platform);
            }
        }

        public static bool IsBuildable(string directory)
        {
            if (!Directory.Exists(directory))
            {
                EditorUtility.DisplayDialog(@"ERROR", $"Directory not exists.{Environment.NewLine}directory:\"{directory}{Environment.NewLine}絶対パス:{Path.GetFullPath(directory)}\"", @"OK");
                return false;
            }

            return true;
        }
    }
}