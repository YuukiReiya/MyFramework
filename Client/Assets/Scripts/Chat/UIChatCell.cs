using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace uGUI.Chat
{
    public class UIChatCell : MonoBehaviour
    {
        [SerializeField]
        private Text userName;
        [SerializeField]
        private Text message;

        RectTransform rectTransform = null;

        public void Setup(string userName,string message)
        {
            this.userName.text = userName;
            this.message.text = message;
            if (rectTransform == null)
            {
                TryGetComponent(out rectTransform);
            }
            rectTransform.sizeDelta = new Vector2(100, 100);
        }
    }
}