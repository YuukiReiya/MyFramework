//参考:https://baba-s.hatenablog.com/entry/2017/12/04/090000#Unity-%E3%82%A8%E3%83%87%E3%82%A3%E3%82%BF%E4%B8%8A%E3%81%A7%E4%BD%95%E3%81%8B%E6%93%8D%E4%BD%9C%E3%81%95%E3%82%8C%E3%81%9F%E6%99%82
//InitializeOnLoad:https://docs.unity3d.com/ja/2019.4/Manual/RunningEditorCodeOnLaunch.html
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoad]
public static class EditorApplicationExpansion
{
    private const BindingFlags c_BindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;
    private static readonly FieldInfo m_info = null;

    static EditorApplicationExpansion()
    {
        UnityEngine.Debug.Log("<color=green>InitializeOnLoad</color>:EditorApplicationExpansion");
        m_info = typeof(EditorApplication).GetField("OnValidate", c_BindingFlags);
    }

    public static EditorApplication.CallbackFunction OnValidate
    {
        get => m_info.GetValue(null) as EditorApplication.CallbackFunction;
        set
        {
            var callback = m_info.GetValue(null) as EditorApplication.CallbackFunction;
            callback += value;
            m_info.SetValue(null, callback);
        }
    }

    public static EditorApplication.CallbackFunction OnUpdate
    {
        get => EditorApplication.update;
        set => EditorApplication.update += value;
    }
}
#endif