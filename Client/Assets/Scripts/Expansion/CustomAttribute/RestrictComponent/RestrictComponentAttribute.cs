using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if false
[Conditional("UNITY_EDITOR"), AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class RestrictComponentAttribute : PropertyAttribute
{
    private Behaviour m_source;
    private Type[] m_components;

    private RestrictComponentAttribute() { }
    public RestrictComponentAttribute(Type[] components)
    {
        //m_source = self;
        //m_components = components;
        //EditorApplicationExpansion.OnValidate += OnValidate;
    }
    public RestrictComponentAttribute(Type component)
    {
        UnityEngine.Debug.Log("通ってます");
        if (!component.IsSubclassOf(typeof(Component)))
        {
            UnityEngine.Debug.LogWarning($"<color=yellow>Warning</color>:{component} is not inherited component.");
        }
        EditorUtility.DisplayDialog("テスト", $"追加出来ないよ", "OK");
        UnityEngine.Debug.Assert(true, "消せ");
    }

    private void OnValidate()
    {
        var behaviour = EditorUtility.InstanceIDToObject(m_source.GetInstanceID()) as Behaviour;
        foreach (var component in m_components)
        {
            if (!component.IsSubclassOf(typeof(Component)))
            {
                UnityEngine.Debug.LogWarning($"<color=yellow>Warning</color>:{component} is not inherited component.");
                continue;
            }
            var c = behaviour.GetComponent(component);
            if (c != null)
            {
                EditorUtility.DisplayDialog("テスト", $"追加出来ないよ", "OK");
                GameObject.DestroyImmediate(c);
            }
        }
    }

    private void Alert()
    {

    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RestrictComponentAttribute))]
class RestrictComponentDrawer : UnityEditor.Editor
{
    private RestrictComponentAttribute m_restrictComponent;

    private void Awake()
    {
        var obj = EditorUtility.InstanceIDToObject(target.GetInstanceID()) as Component;
        var gameObject = obj.GetComponent<GameObject>();
        if (gameObject != null)
        {
            
        }
    }

    private void OnValidate()
    {
        //m_restrictComponent =target as 
    }

    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //{
    //    base.OnGUI(position, property, label);
    //    if (property.IsMonoBehaviour())
    //    {
    //        
    //    }
    //}

    private void DeletesComponents()
    {

    }
}
#endif
#endif
