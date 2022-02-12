using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resources
{
    public static partial class ResourceManager
    {
        public static T GetBuiltinResource<T>(string resourceNameWithExtension) where T : UnityEngine.Object
        {
            return
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.GetBuiltinExtraResource<T>(resourceNameWithExtension)
#else
                UnityEngine.Resources.GetBuiltinResource<T>(resourceNameWithExtension)
#endif
            ;
        }
    }
}