using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.Base;
using System.Diagnostics;
using Helper;
using input = UnityEngine.Input;
using Debug = UnityEngine.Debug;

namespace Model.Input
{
    public partial class InputModel
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        readonly static Dictionary<string, KeyCode> BindingKeyDic = new Dictionary<string, KeyCode>();
        public InputModel() : base()
        {
            SetupBindingKeyDictionary();
        }
        [Conditional("ENABLE_LEGACY_INPUT_MANAGER")]
        private void SetupBindingKeyDictionary()
        {
            // KeyCodeを全マッピング
            foreach (var member in Enum.GetNames(typeof(KeyCode)))
            {
                BindingKeyDic.Add(member, (KeyCode)Enum.Parse(typeof(KeyCode), member));
            }
        }
        private bool IsContainsKey(string keyName)
        {
            if (BindingKeyDic.Keys.Contains(keyName)) return true;
            Debug.LogError($"{keyName} is not found.");
            return false;
        }

        public bool GetButton(string bindingKeyName)
        {
            if (IsContainsKey(bindingKeyName)) return input.GetKey(BindingKeyDic[bindingKeyName]);
            return false;
        }

        public bool GetButtonDown(string bindingKeyName)
        {
            if (IsContainsKey(bindingKeyName)) return input.GetKeyDown(BindingKeyDic[bindingKeyName]);
            return false;
        }

        public bool GetButtonUp(string bindingKeyName)
        {
            if (IsContainsKey(bindingKeyName)) return input.GetKeyUp(BindingKeyDic[bindingKeyName]);
            return false;
        }
#endif
    }
}