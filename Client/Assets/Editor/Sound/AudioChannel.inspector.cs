using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sound;

namespace Editor
{
    [CustomEditor(typeof(Sound.AudioChannel))]
    public class AudioChannel : UnityEditor.Editor
    {
        SerializedProperty indexProperty;

        public override void OnInspectorGUI()
        {
            DrawIndex();
        }

        void DrawIndex()
        {
            if (indexProperty == null)
            {
                indexProperty = serializedObject.FindProperty(@"Index");
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Index");
            EditorGUILayout.TextField($"{(target as Sound.AudioChannel).Index}");
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }
    }
}