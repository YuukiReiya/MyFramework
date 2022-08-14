using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Editor
{
    using Editor = UnityEditor.Editor;
    [CustomEditor(typeof(Sound.AudioManager))]
    public class AudioManager : Editor
    {
        Sound.AudioManager instance = null;
        Inspector.List list = null;
        bool isExpand = false;
        bool isAttachedListener;

        #region const
        const string ChildChannelNamePrefix = @"channel_{0}";
        #endregion

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            instance = target as Sound.AudioManager;

            Draw();
  
            serializedObject.ApplyModifiedProperties();
        }

        void Draw()
        {
            DrawChannelConfig();
            DrawChannelList();
        }

        void DrawChannelConfig()
        {
            EditorGUILayout.LabelField($"チャンネル設定");
            {
                var property = serializedObject.FindProperty(@"channelNum");
                if (property != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"数");
                    property.intValue = EditorGUILayout.IntSlider(property.intValue, 1, 16);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginVertical();
                    if (GUILayout.Button($"チャンネル数の設定"))
                    {
                        SetupChannels();
                    }
                    EditorGUILayout.HelpBox($"指定のチャンネル数になるように数を調整します。{Environment.NewLine}※足りない場合はこのオブジェクトに子オブジェクト作り不足分アタッチします。", MessageType.Info);
                    EditorGUILayout.EndVertical();

                    // Button
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button($"チャンネルの登録"))
                    {
                        ResistChannels();
                    }

                    if (GUILayout.Button($"チャンネル設定をクリア"))
                    {
                        ClearChannels();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            {
                var property = serializedObject.FindProperty(@"isAttachedListener");
                if (property != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"リスナーをこのオブジェクトにアタッチ");
                    property.boolValue = EditorGUILayout.Toggle(property.boolValue);
                    EditorGUILayout.EndHorizontal();

                    if (GUILayout.Button($"リスナー設定"))
                    {
                        SetListener();
                    }
                }
            }
        }

        void DrawChannelList()
        {
            if (list == null)
            {
                var property = serializedObject.FindProperty("channels");
                list = new Inspector.List(property);

                list.onSwapElementUpward += SwapChannelIndex;
                list.onSwapElementDownward += SwapChannelIndex;
            }
            list.Draw();
        }

        #region event
        void SetListener()
        {
            var instance = target as Sound.AudioManager;

            Helper.ScriptableHelper.CallMethod(instance, @"SetListener");
            Debug.Log($"SetListener");
            return;

            #region TODO
            var listeners = GameObject.FindObjectsOfType<AudioListener>();

            if (listeners == null)
            {
                return;
            }

            if (isAttachedListener)
            {


            }
            else
            {

            }
            #endregion
        }

        void SetupChannels()
        {
            var property = serializedObject.FindProperty(@"channelNum");
            if (property == null) 
            {
                Debug.LogError($"Not find serialized property. \"channelNum\"");
                return; 
            }

            var count = property.intValue;

            // チャンネル削除.
            if (count < instance.ChannelList.Count)
            {
                var index = Math.Max(0, count - 1);
                var length = Math.Max(0, instance.ChannelList.Count - index);

                if (EditorUtility.DisplayDialog(@"確認", $"オブジェクトにアタッチされているコンポーネントを削除しますか？", @"Yes", @"No"))
                {
                    for (int i = 1; i < length; ++i)
                    {
                        var channel = instance.ChannelList[Math.Min(index + i, Math.Max(0, instance.ChannelList.Count - 1))];
                        // RequireComponentでAudioSourceアタッチしてるので併せてDestroyする。
                        AudioSource source = channel.gameObject.GetComponent<AudioSource>();
                        DestroyImmediate(channel);
                        if (source != null)
                        {
                            DestroyImmediate(source);
                        }
                    }
                }

                instance.ChannelList.RemoveRange(index, length);
            }
            // チャンネル増やす.
            else if (instance.ChannelList.Count < count)
            {
                var size = instance.ChannelList.Count;
                var loop = count - size;

                const string ChildChannelRoot = @"channels";
                var children = instance.transform.Find(ChildChannelRoot);
                if (children == null)
                {
                    var childObj = new GameObject(ChildChannelRoot);
                    childObj.transform.SetParent(instance.transform);
                    children = childObj.transform;
                }

                //for(int i=0; i< children.transform.childCount; ++i)
                //{
                //    var child = children.GetChild(i);
                //    var component = child.GetComponent<Sound.AudioChannel>();
                //    if (!child.name.StartsWith(ChildChannelNamePrefix) || component == null) continue;

                    
                //}

                for (var i = 0; i < loop; ++i)
                {
                    var name = string.Format(ChildChannelNamePrefix, i);

                    var child = children.Find(name);
                    if (child == null)
                    {
                        var childObj = new GameObject(name);
                        child = childObj.transform;
                    }
                        
                    child.SetParent(children);

                    if (child.gameObject.GetComponent<Sound.AudioChannel>() != null) continue;

                    var channel = child.gameObject.AddComponent<Sound.AudioChannel>();
                    SetChannelIndex(channel, size + i);
                }
            }
        }

        void ResistChannels()
        {
            var channels = FindObjectsOfType<Sound.AudioChannel>();
            if (channels == null) return;

            // 分かりやすく、インスペクターの上から順に並びなおす.
            channels = channels.OrderBy(x => x.gameObject.transform.GetSiblingIndex()).ToArray();

            instance.ChannelList.Clear();

            for(int i = 0; i < channels.Length; ++i)
            {
                var channel = channels[i];
                SetChannelIndex(channel, i);
                instance.ChannelList.Add(channel);
            }
        }

        void ClearChannels()
        {
            if (instance == null) return;

            instance.ChannelList.Clear();
        }
        #endregion

        void SetChannelIndex(Sound.AudioChannel channel, int index)
        {
            var accessory = Helper.ScriptableHelper.GetProperty<Sound.AudioChannel>(@"Index");
            accessory.SetValue(channel, index);
        }

        void SwapChannelIndex(int src,int dest)
        {
            if (instance == null || !instance.ChannelList.Any()) return;

            var swap = instance.ChannelList[Math.Max(0, Math.Min(src, instance.ChannelList.Count))].Index;
            SetChannelIndex(instance.ChannelList[Math.Max(0, Math.Min(src, instance.ChannelList.Count))],
                instance.ChannelList[Math.Max(0, Math.Min(dest, instance.ChannelList.Count))].Index);
            SetChannelIndex(instance.ChannelList[Math.Max(0, Math.Min(dest, instance.ChannelList.Count))], swap);
        }

    }
}