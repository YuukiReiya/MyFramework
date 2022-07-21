using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Expansion
{
    public static class WindowsExpansion
    {
        /// <summary>
        /// %USERNAME%
        /// </summary>
        public static string USERNAME { get { return Environment.UserName; } }

        private static readonly Dictionary<string, string> environmentVariableDict = new Dictionary<string, string>()
        {
            {"%USERNAME%",Environment.UserName},
            {"%USERPROFILE%",Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) },
        };

        public static string Convert(string pathWithExtension)
        {
            var splits = pathWithExtension.Split(Path.DirectorySeparatorChar);
            if (splits == null)
            {
                return string.Empty;
            }

            var conv = string.Join(Path.DirectorySeparatorChar, splits.Select(str =>
            {
                if (environmentVariableDict.ContainsKey(str))
                {
                    return environmentVariableDict[str];
                }
                return str;
            }));
            return conv;
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Test/Log/WindowsEnvironmentList")]
        public static void Log()
        {
            foreach(var v in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                Debug.Log($"{(int)v}:<color=green>{v}</color> {Environment.GetFolderPath((Environment.SpecialFolder)v)}");
            }
        }
#endif
    }
}