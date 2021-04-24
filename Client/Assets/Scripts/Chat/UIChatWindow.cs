using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Server.gRPC;

namespace uGUI.Chat
{
    public class UIChatWindow : MonoBehaviour
    {
        [SerializeField]
        private UIChatCell cellPrefab = null;
        [SerializeField]
        private Transform parentRoot;
        
        // セルを使いまわすオブジェクトプール
        private ObjectPoolList<UIChatCell> cellPoolList = null;

        void Start()
        {
            cellPoolList = new ObjectPoolList<UIChatCell>(cellPrefab);
            cellPoolList.CreateInstanceMethod = () =>
            {
                return Instantiate(cellPrefab, parentRoot);
            };
        }

        public void AddMessage(DuplexChatReceive receive)
        {
            var cell = cellPoolList.Get();
            cell.Setup(receive.UserID.ToString(), receive.Message);
        }
    }
}