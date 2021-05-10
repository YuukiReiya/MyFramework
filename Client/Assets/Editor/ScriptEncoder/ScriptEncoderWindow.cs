﻿using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.Expansion
{
    public class ScriptEncoderWindow : EditorWindow
    {
        private const string WndTitle = "スクリプトエンコーダー";

        private GUIStyle style;
        private string scriptName;

        private static readonly string[] Extensions =
        {
            ".cs",
        };

        private enum EncodeType
        {
            UTF8,
        }
        EncodeType encodeType = EncodeType.UTF8;

        [MenuItem("Tool/ScriptEncode")]
        private static void ShowWindow()
        {
            EditorWindow.GetWindow<ScriptEncoderWindow>(true, WndTitle, false);
        }
        private void OnEnable()
        {
            Setup();
        }
        private void OnGUI()
        {
            DrawSchemeArea("設定", style, DrawSetting, Color.white, Color.white);
        }

        private void Setup()
        {
            style = new GUIStyle();
            style.normal.textColor = Color.white;
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

        private void DrawSetting()
        {
            // エンコード方式の選択ポップアップ
            using(new GUILayout.HorizontalScope(GUI.skin.box))
            {
                encodeType = (EncodeType)EditorGUILayout.EnumPopup("エンコード", encodeType);
            }

            // スクリプトのパス入力フォーム
            // D&D対応
            using (new GUILayout.HorizontalScope(GUI.skin.box))
            {
                GUILayout.Label("変換するスクリプト");
                scriptName = GUILayout.TextField(scriptName);
                var rect = GUILayoutUtility.GetLastRect();
                var dropScriptPath = GetScriptPath(rect);
                if (!string.IsNullOrEmpty(dropScriptPath))
                {
                    scriptName = dropScriptPath;
                }
            }

            // 実行ボタン
            DrawButton();
        }

        private void DrawButton()
        {
            if (!GUILayout.Button("実行")) return;

            IAssetPostprocess encoder = new ScriptEncoder();

            // パスが有効か判定
            if (!IsValidPath(scriptName)) 
            {
                Debug.LogWarning($"{scriptName}:入力されたパスが有効ではありません。");
                return; 
            }
            encoder.Execute(scriptName);

            // アセットの更新
            AssetDatabase.Refresh();
        }

        private bool IsValidPath(string path)
        {
            if (!File.Exists(path)) return false;

            var ext = Path.GetExtension(path);
            if (!Extensions.Contains(ext)) return false;

            return true;
        }

        private string GetScriptPath(Rect dropAreaRect)
        {
            var dropObject = GetDragAndDropObject(dropAreaRect);
            if (dropObject == null) return null;

            // 拡張子を見てスクリプト以外は弾く.
            var path = AssetDatabase.GetAssetPath(dropObject);
            var ext = Path.GetExtension(path);
            if (!Extensions.Contains(ext)) return null;

            return path;
        }

        private UnityEngine.Object GetDragAndDropObject(Rect dropAreaRect)
        {
            // ドラッグ対象のレクト外
            if (!dropAreaRect.Contains(Event.current.mousePosition)) return null;

            var ev = Event.current.type;
            // ドラッグ中もしくはドラッグ実行していなければ処理しない
            if (ev != EventType.DragUpdated && ev != EventType.DragPerform) return null;
            DragAndDrop.AcceptDrag();
            Event.current.Use();
            return DragAndDrop.objectReferences.FirstOrDefault();
        }
    }
}