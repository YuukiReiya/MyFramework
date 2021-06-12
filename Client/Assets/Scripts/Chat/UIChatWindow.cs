using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Server.gRPC;
using System.Linq;
using Model.Chat;

namespace uGUI.Chat
{
    //public class UIChatWindow : MonoBehaviour
    //{
    //    [SerializeField]
    //    private UIChatCell cellPrefab = null;
    //    [SerializeField]
    //    private Transform parentRoot;

    //    // セルを使いまわすオブジェクトプール
    //    private ObjectPoolList<UIChatCell> cellPoolList = null;//Queueで管理しようとすると非同期受信でHashが前後するケースがある模様

    //    void Start()
    //    {
    //        cellPoolList = new ObjectPoolList<UIChatCell>(cellPrefab);
    //        cellPoolList.CreateInstanceMethod = () =>
    //        {
    //            return Instantiate(cellPrefab, parentRoot);
    //        };
    //    }

    //    private void OnEnable()
    //    {
    //        ChatModel.Instance.OnReciveChatMessage += OnReceiveChat;
    //    }

    //    private void OnDisable()
    //    {
    //        ChatModel.Instance.OnReciveChatMessage -= OnReceiveChat;
    //    }
    //    int cnt = 0;
    //    private void Update()
    //    {
    //        //
    //        if (Input.GetKeyDown(KeyCode.M))
    //        {
    //            var cell = cellPoolList.Get();
    //            DuplexChatReceive recv = new DuplexChatReceive { UserID = 0, Hash = (uint)cnt, Message = cnt.ToString() };
    //            cnt++;
    //            cell.Setup(recv);
    //        }
    //    }

    //    public void OnReceiveChat(DuplexChatReceive receive)
    //    {
    //        // 許容数を超えたら古いものからプールに帰し再利用する
    //        if (cellPoolList.Pool.Count(_ => !cellPoolList.IsGetPool(_)/* プール判定条件の偽 */) > ChatModel.Instance.MessageCapacity)
    //        {
    //            var target = cellPoolList.Pool.
    //                Where(_ => !cellPoolList.IsGetPool(_)).//プール対象じゃない ＝ 現在利用中
    //                OrderBy(_ => _.Receive.Hash).             //追加順
    //                FirstOrDefault();                                     //最初の要素

    //            // プールに戻す
    //            RecycleCell(target);
    //        }
    //        var cell = cellPoolList.Get();
    //        cell.Setup(receive);
    //    }

    //    private void RecycleCell(UIChatCell recycleCell)
    //    {
    //        recycleCell.gameObject.SetActive(false);
    //    }
    //}

    public class UIChatWindow : MonoBehaviour
    {
        [SerializeField]
        private UIChatCell cellPrefab = null;
        [SerializeField]
        private UIChatScrollView scrollView;

        void Start()
        {
            scrollView.Setup();
        }

        private void OnEnable()
        {
            ChatModel.Instance.OnReciveChatMessage += OnReceiveChat;
        }

        private void OnDisable()
        {
            ChatModel.Instance.OnReciveChatMessage -= OnReceiveChat;
        }
        int cnt = 0;
        private void Update()
        {
            //
            if (Input.GetKeyDown(KeyCode.M))
            {
                DuplexChatReceive recv = new DuplexChatReceive { UserID = 0, Hash = (uint)cnt, Message = cnt.ToString() };
                ChatModel.Instance.ReceivedMessages.Add(recv);
                cnt++;
                var inst = Instantiate(cellPrefab).GetComponent<UIChatCell>();
                inst.Setup(recv);
                scrollView.Elements.Add(inst);
            }
        }

        public void OnReceiveChat(DuplexChatReceive receive)
        {
            //// 許容数を超えたら古いものからプールに帰し再利用する
            //if (cellPoolList.Pool.Count(_ => !cellPoolList.IsGetPool(_)/* プール判定条件の偽 */) > ChatModel.Instance.MessageCapacity)
            //{
            //    var target = cellPoolList.Pool.
            //        Where(_ => !cellPoolList.IsGetPool(_)).//プール対象じゃない ＝ 現在利用中
            //        OrderBy(_ => _.Receive.Hash).             //追加順
            //        FirstOrDefault();                                     //最初の要素

            //    // プールに戻す
            //    RecycleCell(target);
            //}
            //var cell = cellPoolList.Get();
            //cell.Setup(receive);
        }

        private void RecycleCell(UIChatCell recycleCell)
        {
            recycleCell.gameObject.SetActive(false);
        }
    }
}