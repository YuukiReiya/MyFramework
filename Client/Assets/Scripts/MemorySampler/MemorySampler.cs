using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Profiling.Memory;
using UnityEngine.UI;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Sampler
{
    public static class MemoryUnit
    {
        [Serializable]
        public enum BytesType
        {
            Byte = 0,
            KByte,
            MByte,
            GByte,
        }
        public const uint UnitBasicValue = 1024u;

        #region byteTo
        public static double ByteToKByte(this ulong bytes)
        {
            return (double)bytes / GetCoeffcient(BytesType.KByte);
        }
        public static double ByteToMByte(this ulong bytes)
        {
            return (double)bytes / GetCoeffcient(BytesType.MByte);
        }
        public static double ByteToGByte(this ulong bytes)
        {
            return (double)bytes / GetCoeffcient(BytesType.GByte);
        }
        #endregion

        #region kbyteTo
        public static double KByteToByte(this double kbytes)
        {
            return kbytes * GetCoeffcient(BytesType.KByte);
        }
        public static double KByteToMByte(this double kbytes)
        {
            return kbytes / UnitBasicValue;
        }
        public static double KByteToGByte(this double kbytes)
        {
            return kbytes / Math.Pow(UnitBasicValue, 2u);
        }
        #endregion

        #region mbyteTo
        public static double MbyteToByte(double mbytes)
        {
            return mbytes * GetCoeffcient(BytesType.MByte);
        }
        public static double MbyteToKByte(double mbytes)
        {
            return mbytes * UnitBasicValue;
        }
        public static double MbyteToGByte(double mbytes)
        {
            return mbytes / UnitBasicValue;
        }
        #endregion

        #region gbyteTo
        public static double GbyteToByte(double gbytes)
        {
            return gbytes * GetCoeffcient(BytesType.GByte);
        }
        public static double GbyteToKByte(double gbytes)
        {
            return gbytes * Math.Pow(UnitBasicValue, 2);
        }
        public static double GbyteToMByte(double gbytes)
        {
            return gbytes * UnitBasicValue;
        }
        #endregion

        public static ulong GetCoeffcient(BytesType type)
        {
            return (ulong)Math.Pow(UnitBasicValue, (int)type);
        }

        public static string GetUnitString(BytesType type)
        {
            return type switch
            {
                BytesType.Byte => "B",
                BytesType.KByte => "KB",
                BytesType.MByte => "MB",
                BytesType.GByte => "GB",
                _ => throw new NotImplementedException(),
            };
        }

        public static double GetMemorySizeFromByte(BytesType type, ulong bytes, uint effectiveDigits = 3)
        {
            var ret = type switch
            {
                BytesType.Byte => bytes,
                BytesType.KByte => bytes.ByteToKByte(),
                BytesType.MByte => bytes.ByteToMByte(),
                BytesType.GByte => bytes.ByteToGByte(),
                _ => 0,
            };

            ret = Math.Floor(ret * Math.Pow(10, effectiveDigits)) / Math.Pow(10, effectiveDigits);
            return ret;
        }
    }

    public class MemorySampler : MonoBehaviour
    {
        #region behaviour
#if true
        [Header("UI")]
        [SerializeField] private Text text;
        [Header("Update")]
        [SerializeField] private bool isAlwaysUpdate = true;
        [SerializeField] private float updateTime = 1f;

        [Header("Manual")]
        [SerializeField] private MemoryUnit.BytesType showByteType = MemoryUnit.BytesType.KByte;
        [SerializeField] private uint effectiveDigits = 3;

        private static MemorySampler Instance = null;
        private float elapsedTime = 0f;
        private string format = string.Empty;



        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(text.text))
            {
                format = $"";
            }
            else
            {
                format = text.text;
            }
        }


        private void Update()
        {

            if (!isAlwaysUpdate) return;

            elapsedTime += Time.deltaTime; 
            if (elapsedTime >= updateTime)
            {
                UpdateMemorySize();
                elapsedTime = 0;
            }
        }

        [Conditional("UNITY_EDITOR")]
        private void OnPostRender()
        {
            GUILayout.BeginVertical();
            var useMemory = MemoryUnit.GetMemorySizeFromByte(showByteType, (ulong)Profiler.GetTotalReservedMemoryLong(), effectiveDigits);
            var unuseMemory = MemoryUnit.GetMemorySizeFromByte(showByteType, (ulong)Profiler.GetTotalUnusedReservedMemoryLong(), effectiveDigits);
            var allocateMemory = MemoryUnit.GetMemorySizeFromByte(showByteType, (ulong)Profiler.GetTotalAllocatedMemoryLong(), effectiveDigits);
            var byteType = MemoryUnit.GetUnitString(showByteType);
            GUILayout.Label($"UseMemory:{useMemory}{byteType}");
            GUILayout.Label($"unuseMemory:{unuseMemory}{byteType}");
            GUILayout.Label($"allocateMemory:{ allocateMemory}{byteType}");
            GUILayout.EndVertical();
        }

        private void UpdateMemorySize()
        {
            var useMemory = MemoryUnit.GetMemorySizeFromByte(showByteType, (ulong)Profiler.GetTotalReservedMemoryLong(), effectiveDigits);
            var unuseMemory = MemoryUnit.GetMemorySizeFromByte(showByteType, (ulong)Profiler.GetTotalUnusedReservedMemoryLong(), effectiveDigits);
            var allocateMemory = MemoryUnit.GetMemorySizeFromByte(showByteType, (ulong)Profiler.GetTotalAllocatedMemoryLong(), effectiveDigits);
            var byteType = MemoryUnit.GetUnitString(showByteType);

            if (text != null)
            {
                text.text = string.Format(format,
                    useMemory, byteType,
                    unuseMemory, byteType,
                    allocateMemory, byteType);
            }
        }

        #region EDITOR_ONLY
#if UNITY_EDITOR
        public GameObject sampleTargetGameObject;
        [Conditional("UNITY_EDITOR"), ContextMenu("GetUseRuntimeMemorySize")]
        public void RuntimeMemorySample()
        {
            if (sampleTargetGameObject != null)
            {
                Debug.Log($"GetUseRuntimeMemorySize:{GetUseRuntimeMemorySize(sampleTargetGameObject)}{MemoryUnit.GetUnitString(showByteType)}", sampleTargetGameObject);
            }
            else
            {
                Debug.LogError($"Invalid sample target object.", this);
            }
        }
#endif
        #endregion

        public static double GetUseRuntimeMemorySize(GameObject obj)
        {
            var components = obj.GetComponentsInChildren<Component>();
            var totalMemory = (ulong)Profiler.GetRuntimeMemorySizeLong(obj);
            foreach (var component in components)
            {
                var memory = (ulong)Profiler.GetRuntimeMemorySizeLong(component);
                totalMemory += memory;
            }
            return MemoryUnit.GetMemorySizeFromByte(Instance.showByteType, totalMemory, Instance.effectiveDigits);
        }

        // GameObjectのRuntimeMemory
        public static double GetRuntimeObjectMemorySize(GameObject obj)
        {
            return MemoryUnit.GetMemorySizeFromByte(Instance.showByteType, (ulong)Profiler.GetRuntimeMemorySizeLong(obj), Instance.effectiveDigits);
        }
        // TransformのRuntimeMemory
        public static double GetRuntimeTransformMemorySize(Transform trans)
        {
            return MemoryUnit.GetMemorySizeFromByte(Instance.showByteType, (ulong)Profiler.GetRuntimeMemorySizeLong(trans), Instance.effectiveDigits);
        }

        public static double GetRuntimeBehavioursMemorySize(GameObject obj)
        {
            var behaviours = obj.GetComponentsInChildren<MonoBehaviour>();
            var totalMemory = (ulong)0d;
            foreach (var behaviour in behaviours)
            {
                var memory = (ulong)Profiler.GetRuntimeMemorySizeLong(behaviour);
                totalMemory += memory;
            }
            return MemoryUnit.GetMemorySizeFromByte(Instance.showByteType, totalMemory, Instance.effectiveDigits);
        }

        public static double GetRuntimeComponentsMemorySize(GameObject obj)
        {
            var components = obj.GetComponentsInChildren<Component>();
            var totalMemory = 0ul;
            foreach (var component in components)
            {
                var memory = (ulong)Profiler.GetRuntimeMemorySizeLong(component);
                totalMemory += memory;
            }
            return MemoryUnit.GetMemorySizeFromByte(Instance.showByteType, totalMemory, Instance.effectiveDigits);
        }

        public static double GetRuntimeMemorySize<T>(GameObject obj) where T : Component
        {
            var components = obj.GetComponentsInChildren<T>();
            var totalMemory = 0ul;
            foreach (var component in components)
            {
                var memory = (ulong)Profiler.GetRuntimeMemorySizeLong(component);
                totalMemory += memory;
            }
            return MemoryUnit.GetMemorySizeFromByte(Instance.showByteType, totalMemory, Instance.effectiveDigits);
        }
#endif
        #endregion
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class MemorySamplerEditor
    {
        static bool isDrawMemory = true;
        static MemoryUnit.BytesType showByteType = MemoryUnit.BytesType.GByte;
        static uint effectiveDigits = 3;

        static MemorySamplerEditor()
        {
            SceneView.duringSceneGui += (sceneView) =>
            {
                Handles.BeginGUI();
                DrawMemory();
                Handles.EndGUI();
            };
        }

        private static void DrawMemory()
        {
            using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Width(100)))
            {
                using (new GUILayout.HorizontalScope())
                {
                    isDrawMemory = GUILayout.Toggle(isDrawMemory, $"IsDrawMemory");
                    if (isDrawMemory) showByteType = (MemoryUnit.BytesType)EditorGUILayout.EnumPopup(showByteType);
                }
                if (isDrawMemory)
                {
                    var useMemory = MemoryUnit.GetMemorySizeFromByte(showByteType, (ulong)Profiler.GetTotalReservedMemoryLong(), effectiveDigits);
                    var unuseMemory = MemoryUnit.GetMemorySizeFromByte(showByteType, (ulong)Profiler.GetTotalUnusedReservedMemoryLong(), effectiveDigits);
                    var allocateMemory = MemoryUnit.GetMemorySizeFromByte(showByteType, (ulong)Profiler.GetTotalAllocatedMemoryLong(), effectiveDigits);
                    var byteType = MemoryUnit.GetUnitString(showByteType);
                    EditorGUILayout.LabelField($"UseMemory:{useMemory}{byteType}");
                    EditorGUILayout.LabelField($"unuseMemory:{unuseMemory}{byteType}");
                    EditorGUILayout.LabelField($"allocateMemory:{ allocateMemory}{byteType}");
                }
            }
        }
    }
#endif
}