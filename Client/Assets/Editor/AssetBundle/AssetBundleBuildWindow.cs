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
//using UnityEditorInternal;

/// MEMO:スクリプト側からのラベル設定.
/// https://qiita.com/kado34/items/da821a4c043e0ecc7957
namespace Editor.Expansion
{
    public class AssetBundleBuildWindow : EditorWindow
    {
        private const string WndTitle = @"AssetBundleBuild";
        private Color MainColor = Color.white;
        private GUIStyle style = new GUIStyle();
        private bool isExclusive = false;
        private BuildTarget platform = BuildTarget.NoTarget;
        private BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
        //private AssetBundleBuild assetBundleBuild;
        private string outputPath;
        private string assetBundleName;
        //https://kan-kikuchi.hatenablog.com/entry/ReorderableList
        //private ReorderableList entoryMapReorderableList;
        private List<AssetBundleBuild> assetBundleBuildEntoryMapList = new List<AssetBundleBuild>();
        private readonly Dictionary<BuildAssetBundleOptions, string> optionsMessageDic = new Dictionary<BuildAssetBundleOptions, string>()
        {
            // None
            {
                BuildAssetBundleOptions.None,
                "特別なオプション無しにアセットバンドルを作成します."
            },
            // UncompressedAssetBundle
            {
                BuildAssetBundleOptions.UncompressedAssetBundle,
                "アセットバンドルを生成するときに圧縮処理を行わないようにします."
            },
            // DisableWriteTypeTree
            {
                BuildAssetBundleOptions.DisableWriteTypeTree,
                "アセットバンドル内にタイプに関する情報を入れないようにします."
            },
            // DeterministicAssetBundle
            {
                BuildAssetBundleOptions.DeterministicAssetBundle,
                "アセットバンドルに保管されているオブジェクト ID のハッシュを使用して、アセットバンドルを作成します."
            },
            // ForceRebuildAssetBundle
            {
                BuildAssetBundleOptions.ForceRebuildAssetBundle,
                "強制的にアセットバンドルを再ビルドします."
            },
            // IgnoreTypeTreeChanges
            {
                BuildAssetBundleOptions.IgnoreTypeTreeChanges,
                "インクリメンタルビルドチェックを行う場合、タイプツリーの変更を無視します."
            },
            // AppendHashToAssetBundleName
            {
                BuildAssetBundleOptions.AppendHashToAssetBundleName,
                "アセットバンドル名にハッシュを追加します."
            },
            // ChunkBasedCompression
            {
                BuildAssetBundleOptions.ChunkBasedCompression,
                "アセットバンドル作成時にチャンクベースの LZ4 圧縮を使用します。."
            },
            // StrictMode
            {
                BuildAssetBundleOptions.StrictMode,
                "ビルド中にエラーが発生したら、ビルドを中断します."
            },
            // DryRunBuild
            {
                BuildAssetBundleOptions.DryRunBuild,
                //https://docs.unity3d.com/ja/2021.2/ScriptReference/BuildAssetBundleOptions.DryRunBuild.html
                "Do a dry run build.\n実際にはアセットバンドルをビルドせず、manifest のみを作成"
            },

            //https://qiita.com/k7a/items/df6dd8ea66cbc5a1e21d
            // DisableLoadAssetByFileName
            {
                BuildAssetBundleOptions.DisableLoadAssetByFileName,
                "Disables Asset Bundle LoadAsset by file name.\nファイル名でのアクセスを無効化することができる。"
            },
            // DisableLoadAssetByFileNameWithExtension
            {
                BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension,
                "Disables Asset Bundle LoadAsset by file name with extension.\nファイル名＋拡張子でのアクセスを無効化することができる。"
            },
            // AssetBundleStripUnityVersion
            {
                BuildAssetBundleOptions.AssetBundleStripUnityVersion,
                //https://baba-s.hatenablog.com/entry/2022/01/27/120000
                "ビルド中にアーカイブファイルとシリアル化されたファイルのヘッダーにあるUnityバージョン番号を削除します.\nビルドしたアセットバンドルのヘッダにUnity のバージョン情報が含まれなくなる."
            },

            //// Obsolete:廃止
            // CollectDependencies
            {
                BuildAssetBundleOptions.CollectDependencies,
                "すべての依存関係を含みます."
            },
            // CompleteAssets
            {
                BuildAssetBundleOptions.CompleteAssets,
                "アセット全体が強制的に含まれます."
            },
        };

        [MenuItem("AssetBundle/Window")]
        private static void ShowWindow()
        {
            GetWindow<AssetBundleBuildWindow>(true, WndTitle, false);
        }

        private void OnEnable()
        {
            Setup();
        }

        private void OnGUI()
        {
            DrawSchemeArea(@"設定", style, Draw, Color.white, Color.white);
        }

        private void Setup()
        {
            style.normal.textColor = MainColor;
            minSize = new Vector2(350, 200);

            // 現在のビルド設定.
            platform = EditorUserBuildSettings.activeBuildTarget;

            // ReorderableList
            //entoryMapReorderableList = new ReorderableList(assetBundleBuildEntoryMapList, typeof(AssetBundleBuild), true, true, true, true);
            //entoryMapReorderableList.drawElementCallback += DrawReorderableListElement;
        }

        private void Draw()
        {
            DrawBuildTarget();
            DrawBuildOptions();
            DrawOutputPath();
            DrawExclusiveToggle();
            DrawCustomArea();
            DrawBuildButton();
        }

        private void DrawBuildTarget()
        {
            platform = (BuildTarget)EditorGUILayout.EnumPopup(@"BuildTarget", platform);
        }

        private void DrawOutputPath()
        {
            outputPath = EditorGUILayout.TextField(@"出力先", outputPath);
        }

        private void DrawExclusiveToggle()
        {
            isExclusive = EditorGUILayout.Toggle(@"限定ビルド", isExclusive);
        }

        private void DrawCustomArea()
        {
            using (new EditorGUI.DisabledScope(!isExclusive))
            {
                assetBundleName = EditorGUILayout.TextField(@"AssetBundleName", assetBundleName);
                // TODO:variant対応
                // 実装の必要ないので未実装.
                //assetBundleBuild.assetBundleVariant = EditorGUILayout.TextField(@"Variant", assetBundleBuild.assetBundleVariant);
            }
        }

        private void DrawBuildButton()
        {
            if (GUILayout.Button(@"Build"))
            {
                if (isExclusive)
                {
                    var assets = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);

                    if (!assets.Any())
                    {
                        EditorUtility.DisplayDialog(@"ERROR", $"No assets were found from AssetBundleName.{Environment.NewLine}AssetBundleName:\"{assetBundleName}\"", @"OK");
                        return;
                    }

                    var assetBundleBuild = new AssetBundleBuild();
                    assetBundleBuild.assetBundleName = assetBundleName;
                    assetBundleBuild.assetNames = assets.ToArray();

                    // TODO: 現状AddressableAssetSystem未対応.

                    var builds = new AssetBundleBuild[] { assetBundleBuild };
                    AssetBundle.Build(outputPath, builds, options, platform);
                }
                else
                {
                    AssetBundle.Build(outputPath, options, platform);
                }
            }
        }

        private void DrawBuildOptions()
        {
            options = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup(@"BuildOption", options);
            if (!optionsMessageDic.ContainsKey(options)) return;

            EditorGUILayout.HelpBox(optionsMessageDic[options], MessageType.Info);
        }

        private void DrawSchemeArea(string schemeLabel, GUIStyle style, Action drawEvent, Color contentColor, Color backColor)
        {
            var defaultBackColor = GUI.backgroundColor;
            var defaultContentColor = GUI.contentColor;
            GUI.contentColor = contentColor;
            GUI.backgroundColor = backColor;
            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                //  ヘッダー
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    EditorGUILayout.LabelField(schemeLabel, style);
                }
                GUI.backgroundColor = defaultBackColor;
                GUI.contentColor = defaultContentColor;

                //  登録された描画イベント
                drawEvent?.Invoke();
            }
        }

        #region ReorderableList
        //private void DrawReorderableListElement(Rect rect, int index, bool isActive, bool isFocused)
        //{
        //    var element = assetBundleBuildEntoryMapList[index];
        //    using (new EditorGUILayout.VerticalScope())
        //    {
        //        element.assetBundleName = EditorGUI.TextField(rect, @"AssetBundleName", element.assetBundleName);
        //    }
        //}
        #endregion
    }
}