using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Assertions;
#endif

public class TweenTransform : MonoBehaviour
{
    [SerializeField]
    AnimationCurve m_curve = null;
    [SerializeField]
    float m_animationTime = 0;
    [SerializeField]
    Vector3 m_fromPosition = Vector3.zero;
    [SerializeField]
    Vector3 m_toPosition = Vector3.zero;
    [SerializeField]
    Vector3 m_fromRotation = Vector3.zero;
    [SerializeField]
    Vector3 m_toRotation = Vector3.zero;
    [SerializeField]
    Vector3 m_fromScale = Vector3.one;
    [SerializeField]
    Vector3 m_toScale = Vector3.one;

    float m_elapsedTime = 0;
    float AnimationTime
    {
        get
        {
            var ret = m_curve.postWrapMode switch
            {
                // PingPongは 0→1→0...とループさせるため2倍の時間にする
                WrapMode.PingPong=>m_animationTime*2,
                _=> m_animationTime
            };
            return ret;
        }
    }
    bool IsClampAnimationCurve
    {
        get
        {
            var isClamp = m_curve.postWrapMode switch
            {
                WrapMode.Clamp => true,
                WrapMode.ClampForever => true,
                _ => false,
            };
            return isClamp;
        }
    }
    IEnumerator routine = null;

#if UNITY_EDITOR
    private void Start()
    {
        // アニメーションカーブが未設定.
        Debug.Assert(m_curve != null && m_curve.length != 0, $"アニメーションカーブが設定されていません", this.gameObject);
        // アニメーションの時間が0以下
        Debug.Assert(m_animationTime > 0, $"遷移時間が0以下", this.gameObject);
    }
#endif

    private void OnEnable()
    {
        if (m_animationTime > 0)
        {
            routine = Routine();
            StartCoroutine(routine);
        }
    }

    private void OnDisable()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    IEnumerator Routine()
    {
        while (true)
        {
#if UNITY_EDITOR
            if (m_animationTime <= 0)
            {
                Debug.LogError($"アニメーションの遷移時間に0以下が設定されました:{m_animationTime}", this.gameObject);
                yield break;
            }
#endif
            var value = m_elapsedTime / m_animationTime;
            var evalueate = m_curve.Evaluate(value);

            if (m_elapsedTime >= AnimationTime)
            {
                if (IsClampAnimationCurve)
                {
                    evalueate = 1f;
                }
                else
                {
                    m_elapsedTime = 0;
                }
            }

            var pos = GetTweenVector3(m_fromPosition, m_toPosition, evalueate);
            var rotate = GetTweenVector3(m_fromRotation, m_toRotation, evalueate);
            var scale = GetTweenVector3(m_fromScale, m_toScale, evalueate);

            // ローカル系
            this.gameObject.transform.localPosition = pos;
            this.gameObject.transform.localRotation = Quaternion.Euler(rotate);
            this.gameObject.transform.localScale = scale;
            yield return null;
            m_elapsedTime += Time.deltaTime;
        }
    }

    Vector3 GetTweenVector3(Vector3 from, Vector3 to, float evalueate)
    {
        // Tweenしないなら計算不要.
        if (from == to) return from;

        var x = GetTweenValue(from.x, to.x, evalueate);
        var y = GetTweenValue(from.y, to.y, evalueate);
        var z = GetTweenValue(from.z, to.z, evalueate);
        return new Vector3(x, y, z);
    }

    float GetTweenValue(float from, float to, float rate)
    {
        var formula = to - from;
        if (formula == 0) return from;
        
        rate = Mathf.Clamp01(rate);
        var abs = Mathf.Abs(formula);
        if (formula > 0)
        {
            return from + abs * rate;
        }
        else if (formula < 0)
        {
            return from - abs * rate;
        }

#if UNITY_EDITOR
        Debug.LogWarning($"invalid tween value.\nfrom:{from}\nto:{to}\nrate:{rate}");
#endif
        return from;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TweenTransform))]
public class TweenTransformInspecter : Editor
{
    SerializedProperty animationCurveProp => serializedObject.FindProperty("m_curve");
    SerializedProperty animationTimeProp => serializedObject.FindProperty("m_animationTime");
    SerializedProperty fromPositionProp => serializedObject.FindProperty("m_fromPosition");
    SerializedProperty toPositionProp => serializedObject.FindProperty("m_toPosition");
    SerializedProperty fromRotationProp => serializedObject.FindProperty("m_fromRotation");
    SerializedProperty toRotationProp => serializedObject.FindProperty("m_toRotation");
    SerializedProperty fromScaleProp => serializedObject.FindProperty("m_fromScale");
    SerializedProperty toScaleProp => serializedObject.FindProperty("m_toScale");

    // memo.
    // Editor拡張用クラスも実行時にフラグが入ってしまう。
    // 宣言にtrue厳禁。
    bool isSyncLocalTransform = false;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawAnimationCurve();
        DrawSyncCurrentTransform();
        DrawVector3Property();
        DrawCurrentTransformButton();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnValidate()
    {
        
    }

    void DrawAnimationCurve()
    {
        using (new GUILayout.VerticalScope(GUI.skin.box))
        {
            EditorGUILayout.LabelField($"アニメーションカーブ");
            animationCurveProp.animationCurveValue = EditorGUILayout.CurveField($"", animationCurveProp.animationCurveValue);
            animationTimeProp.floatValue = EditorGUILayout.FloatField($"アニメーションループ時間", animationTimeProp.floatValue);
        }
    }

    void DrawVector3Property()
    {
        GUILayout.Label($"座標");
        using (new GUILayout.VerticalScope(GUI.skin.box))
        {
            EditorGUILayout.LabelField($"移動元");
            fromPositionProp.vector3Value = EditorGUILayout.Vector3Field($"", fromPositionProp.vector3Value);
            EditorGUILayout.LabelField($"移動先");
            toPositionProp.vector3Value = EditorGUILayout.Vector3Field($"", toPositionProp.vector3Value);
        }
        GUILayout.Label($"回転");
        using (new GUILayout.VerticalScope(GUI.skin.box))
        {
            EditorGUILayout.LabelField($"回転元");
            fromRotationProp.vector3Value = EditorGUILayout.Vector3Field($"", fromRotationProp.vector3Value);
            EditorGUILayout.LabelField($"回転先");
            toRotationProp.vector3Value = EditorGUILayout.Vector3Field($"", toRotationProp.vector3Value);
        }
        GUILayout.Label($"拡大縮小");
        using (new GUILayout.VerticalScope(GUI.skin.box))
        {
            EditorGUILayout.LabelField($"拡縮元");
            fromScaleProp.vector3Value = EditorGUILayout.Vector3Field($"", fromScaleProp.vector3Value);
            EditorGUILayout.LabelField($"拡縮先");
            toScaleProp.vector3Value = EditorGUILayout.Vector3Field($"", toScaleProp.vector3Value);
        }
    }

    void DrawSyncCurrentTransform()
    {
        EditorGUILayout.Space(20);
        using (new GUILayout.VerticalScope(GUI.skin.box))
        {
            GUILayout.Label($"オブジェクトのLocalTransformを遷移元に同期させる");
            isSyncLocalTransform = GUILayout.Toggle(isSyncLocalTransform, $"");
            if (isSyncLocalTransform)
            {
                Debug.Log("同期");
                SyncLocalTransformForCurrentLocalTransform();
            }
        }
        EditorGUILayout.Space(10);
    }

    void DrawCurrentTransformButton()
    {  
        if (GUILayout.Button($"遷移元を現在のローカル系に設定する."))
        {
            SyncLocalTransformForCurrentLocalTransform();
        }
    }

    void SyncLocalTransformForCurrentLocalTransform()
    {
        var obj = (target as TweenTransform).gameObject;
        fromPositionProp.vector3Value = obj.transform.localPosition;
        fromRotationProp.vector3Value = obj.transform.localRotation.eulerAngles;
        fromScaleProp.vector3Value = obj.transform.localScale;
    }
}
#endif