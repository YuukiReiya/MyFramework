using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Server.gRPC;

namespace uGUI.Chat
{
    public class UIChatCell : UIBehaviour, IScrollElement<DuplexChatReceive>
    {
        [SerializeField]
        private Text userName;
        [SerializeField]
        private Text message;

        public DuplexChatReceive Receive { get; private set; } = null;
        private RectTransform rectTransform = null;

        public void Setup(DuplexChatReceive receive)
        {
            this.Receive = receive;
            this.userName.text = receive.UserID.ToString();
            this.message.text = $":{receive.Hash}:" + receive.Message;
            if (rectTransform == null)
            {
                TryGetComponent(out rectTransform);
            }
            rectTransform.sizeDelta = new Vector2(100, 100);
        }
    }
}