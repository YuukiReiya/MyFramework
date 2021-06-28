using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Expansion;

namespace Helper
{
    public static class ScriptableHelper
    {
        public const BindingFlags c_InstanceMethodBindingFlag = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags c_StaticMethodBindingFlag = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags c_InstanceFieldBindingFlag = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags c_StaticFieldBindingFlag = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags c_InstancePropertyBindingFlag = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags c_StaticPropertyBindingFlag = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags c_EnumBindingFlag = BindingFlags.DeclaredOnly | BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        private static readonly BindingFlags c_AllMask = (BindingFlags)((1 << Enum.GetNames(typeof(BindingFlags)).Length) - 1);

        public static void CallMethod<T>(T instance, string methodName, BindingFlags bindFlag, params object[] arguments)
        {
            var method = typeof(T).GetMethod(methodName, bindFlag,
                null,
                arguments.Select(_ => _.GetType()).ToArray(),
                null);
            if (method == null)
            {
                Debug.LogError($"Not found method. : \"{typeof(T)}.{methodName}\"({string.Join(",", arguments)})");
            }
            else
            {
#if UNITY_EDITOR
                try
                {
                    method.Invoke(instance, arguments);
                }
                catch (AmbiguousMatchException ame)
                {
                    // AmbiguousMatchException: Ambiguous match found.
                    // 定義された関数が複数(オーバーロード)ありどれなのか判断できない
                    Debug.LogError($"<color=red>[try-catch]</color>{ame.GetType().Name}:{methodName} is defined more than once.");
                }
                catch (Exception e)
                {
                    // InnerExceptionが無い場合もあるので面倒なので未考慮.
                    Debug.LogError($"<color=red>[try-catch]</color>{e.GetType().Name}:{e.Message}\n{e.StackTrace}\n");//\n<color=orange>InnerException</color>:{e.InnerException.Message}\n{e.InnerException.StackTrace}");
                    throw e;
                }
#else
                method.Invoke(instance, arguments);
#endif
            }
        }

        public static void CallMethod<T>(T instance, string methodName, params object[] arguments) => CallMethod(instance, methodName, c_InstanceMethodBindingFlag, arguments);

        public static void CallStaticMethod<T>(string methodName, params object[] arguments)
        {
            var method = typeof(T).GetMethod(methodName, c_StaticMethodBindingFlag, null, arguments.Select(_ => _.GetType()).ToArray(), null);
            if (method == null)
            {
                Debug.LogError($"Not found method. : \"{typeof(T)}.{methodName}\"({string.Join(",", arguments)})");
            }
            else
            {
#if UNITY_EDITOR
                try
                {
                    method.Invoke(null, arguments);
                }
                catch (AmbiguousMatchException ame)
                {
                    // AmbiguousMatchException: Ambiguous match found.
                    // 定義された関数が複数(オーバーロード)ありどれなのか判断できない
                    Debug.LogError($"<color=red>[try-catch]</color>{ame.GetType().Name}:{methodName} is defined more than once.");
                }
                catch (Exception e)
                {
                    // InnerExceptionが無い場合もあるので面倒なので未考慮.
                    Debug.LogError($"<color=red>[try-catch]</color>{e.GetType().Name}:{e.Message}\n{e.StackTrace}\n");//\n<color=orange>InnerException</color>:{e.InnerException.Message}\n{e.InnerException.StackTrace}");
                    throw e;
                }
#else
                method.Invoke(null, arguments);
#endif
            }
        }

        // 関数.
        // memo.返り値が欲しい場合などこっちで関数取ってInvokeで呼びだした方が使い勝手よさそう.
        public static MethodInfo GetMethod<T>(string methodName, BindingFlags bindFlag, params Type[] argumentsTypes)
        {
            var method = typeof(T).GetMethod(methodName, bindFlag, null, argumentsTypes, null);
            if (method == null)
            {
                Debug.LogError($"Not found method. : \"{typeof(T)}.{methodName}");
            }
            return method;
        }

        public static MethodInfo GetMethod<T>(string methodName, params Type[] argumentsTypes) => GetMethod<T>(methodName, c_InstanceMethodBindingFlag, argumentsTypes);

        public static MethodInfo GetStaticMethod<T>(string methodName, params Type[] argumentsTypes) => GetMethod<T>(methodName, c_StaticMethodBindingFlag, argumentsTypes);

        // 変数.
        public static FieldInfo GetField<T>(string fieldName, BindingFlags bindFlag)
        {
            var field = typeof(T).GetField(fieldName, bindFlag);
            if (field == null)
            {
                Debug.LogError($"Not found field. : \"{typeof(T)}.{fieldName}");
            }
            return field;
        }

        public static FieldInfo GetField<T>(string fieldName) => GetField<T>(fieldName, c_InstanceFieldBindingFlag);

        public static FieldInfo GetStaticField<T>(string fieldName) => GetField<T>(fieldName, c_StaticFieldBindingFlag);

        // プロパティ.
        public static PropertyInfo GetProperty<T>(string propertyName, BindingFlags bindFlag)
        {
            var property = typeof(T).GetProperty(propertyName, bindFlag);
            if (property == null)
            {
                Debug.LogError($"Not found property. : \"{typeof(T)}.{propertyName}");
            }
            return property;
        }

        public static PropertyInfo GetProperty<T>(string propertyName) => GetProperty<T>(propertyName, c_InstancePropertyBindingFlag);

        public static PropertyInfo GetStaticProperty<T>(string propertyName) => GetProperty<T>(propertyName, c_StaticPropertyBindingFlag);

        // 列挙体.
        // HACK.Enum.GetNamesで取得した値に対してEnum.Parseすればいいことに気づいた。いらないかも。
        public static IEnumerable<MemberInfo> GetEnumMembers<T>(BindingFlags bindFlag)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                Debug.LogError($"{type.Name} is not enum.");
                return null;
            }
            var members = GetMembers<T>(bindFlag);
            if (members == null)
            {
                return null;
            }
            return members.Where(m => Enum.GetNames(typeof(T)).Contains(m.Name)).DefaultIfEmpty().ToArray();
        }

        public static IEnumerable<MemberInfo> GetEnumMembers<T>() => GetEnumMembers<T>(c_EnumBindingFlag);

        public static IEnumerable<MemberInfo> GetMembers<T>(BindingFlags bindFlag)
        {
            var members = typeof(T).GetMembers(bindFlag);
            if (members == null)
            {
                Debug.LogError($"{typeof(T).Name} is not found members.");
            }
            return members;
        }
        public static IEnumerable<MemberInfo> GetMembers<T>()
        {
            var members = typeof(T).GetMembers();
            if (members == null)
            {
                Debug.LogError($"{typeof(T).Name} is not found members.");
            }
            return members;
        }
    }
}