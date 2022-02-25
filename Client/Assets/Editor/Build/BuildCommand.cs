using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEditor.Build.Reporting;


public class BuildCommand
{
    private const string BuildConfigFilePath = @"Assets\Editor\Build\BuildConfig.xml";
    //https://blog.applibot.co.jp/2018/08/31/buildplayer-unity-201801/
    [MenuItem("Build/BuildFromXML")]
    public static void BuildFromXML()
    {
        if (!File.Exists(BuildConfigFilePath))
        {
            throw new FileNotFoundException($"{Path.GetFullPath(BuildConfigFilePath)}", BuildConfigFilePath);
        }
        var doc = XDocument.Load(BuildConfigFilePath);
        const string RootTag = @"Root";
        var root = doc.Element(RootTag);
        const string ScenesTag = @"Scenes";
        var scenes = root.Element(ScenesTag).Elements().Select(x => x.Value).ToArray();

        const string configsTag = @"Configs";
        const string platformTag = @"platform";
        const string outputTag = @"output";
        const string optionTag = @"options";
        bool resultContains = false;
        foreach (var config in root.Elements(configsTag))
        {
            var outputDir = config.Element(outputTag).Value;
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var platform = config.Element(platformTag).Value switch
            {
                @"Windows" => BuildTarget.StandaloneWindows64,
                @"Windows x86" => BuildTarget.StandaloneWindows,
                @"OSX" => BuildTarget.StandaloneOSX,
                @"iOS" => BuildTarget.iOS,
                @"Android" => BuildTarget.Android,
                // 現在の設定.
                _ => EditorUserBuildSettings.activeBuildTarget,
            };

            int value;
            if (!int.TryParse(config.Element(optionTag).Value, out value))
            {
                value = (int)BuildOptions.None;
            }
            var option = (BuildOptions)value;
            var report = BuildPipeline.BuildPlayer(
               scenes,
               outputDir,
               platform,
               option);

            var msg = report.summary.result switch
            {
                BuildResult.Succeeded => $"[BuildResult]Success.{Environment.NewLine}出力先:{outputDir}{Environment.NewLine}Platform:{platform}{Environment.NewLine}Options:{option}",
                BuildResult.Cancelled => $"[BuildResult]Warning.{Environment.NewLine}{string.Join($"{Environment.NewLine}", report.steps.Select(x => x.messages))}",
                BuildResult.Failed => $"[BuildResult]Failed.{Environment.NewLine}{string.Join($"{Environment.NewLine}", report.steps.Select(x => x.messages))}",
                BuildResult.Unknown => $"[BuildResult]Unknown.{Environment.NewLine}{string.Join($"{Environment.NewLine}", report.steps.Select(x => x.messages))}",
                _ => $"[BuildResult]default.{Environment.NewLine}{string.Join($"{Environment.NewLine}", report.steps.Select(x => x.messages))}",
            };

            switch (report.summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log(msg);
                    resultContains = true;
                    break;
                case BuildResult.Cancelled:
                    Debug.LogWarning(msg);
                    break;
                case BuildResult.Unknown:
                case BuildResult.Failed:
                default:
                    Debug.LogError(msg);
                    break;
            };
        }

        EditorApplication.Exit(resultContains ? 0 : 1);
    }
}
