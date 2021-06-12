using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public static class SerializedPropertyExpansion
{
    /// <summary>
    /// MonoBehaviour継承クラスか.
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public static bool IsMonoBehaviour(this SerializedProperty property)
    {
        return property.serializedObject.targetObject.GetType().IsSubclassOf(typeof(UnityEngine.MonoBehaviour));
    }

}
