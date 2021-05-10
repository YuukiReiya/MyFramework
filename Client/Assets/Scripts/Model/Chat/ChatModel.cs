using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Server.gRPC;
using Model.Base;
using Grpc.Core;

namespace Model.Chat
{
    public class ChatModel : ModelBase<ChatModel>
    {
        /// <summary>
        /// 送信するチャット情報の詰まったリクエストキュー
        /// </summary>
        private Queue<DuplexChatSend> RequestSendMessageQueue = new Queue<DuplexChatSend>();

        /// <summary>
        /// 受け取ったチャット情報
        /// </summary>
        public List<DuplexChatReceive> ReceivedMessages = new List<DuplexChatReceive>();

        /// <summary>
        /// メッセージ表示の許容数
        /// </summary>
        public uint MessageCapacity { get; private set; } = 5;

        /// <summary>
        /// チャットメッセージのリクエスト時に行うコールバック
        /// ※送信は別スレッドで行われ、UnityAPIが使えないのでコールバックのタイミングはリクエスト時にしてる。
        /// </summary>
        public event Action<DuplexChatSend> OnRequestChatMessage = null;

        /// <summary>
        /// チャット受信時に行うコールバック
        /// </summary>
        public event Action<DuplexChatReceive> OnReciveChatMessage = null;

        /// <summary>
        /// 双方向ストリーミング
        /// CL → SVチャットメッセージ送信処理
        /// >>> S2C_Receive_Duplex_Chat
        /// </summary>
        /// <param name="receive"></param>
        //public void C2S_Send_Duplex_Chat(System.Threading.Tasks.Task ,DuplexChatSend send)
        public async Task C2S_Send_Duplex_Chat(AsyncDuplexStreamingCall<DuplexChatSend, DuplexChatReceive> call, Task receiveTask)
        {
            try
            {
                // リクエストの書き込み
                foreach (var request in RequestSendMessageQueue)
                {
                    await call.RequestStream.WriteAsync(request);
                }
                RequestSendMessageQueue.Clear();

                // 書き込み終わるまで待つ
                await call.RequestStream.CompleteAsync();

                // 受信処理が終わるまで待つ
                await receiveTask;
            }
            catch (Exception e)
            {
                Sample.ClientSample.context.Post(_ =>
                {
                    Debug.LogError($"<color=red>C2S_Send_Chat</color>:{e.GetType()}\n{e.Message}\n{e.StackTrace}");
                }, null);
                throw;
            }
        }

        /// <summary>
        /// 双方向ストリーミング
        /// SV → CLチャットメッセージ受信処理
        /// >>> C2S_Send_Duplex_Chat
        /// </summary>
        /// <param name="receive"></param>
        public void S2C_Receive_Duplex_Chat(DuplexChatReceive receive)
        {
            // 同じデータが重複して登録されることを防ぐために選別
            if (!ReceivedMessages.Any(_ => _.Hash == receive.Hash))
            {
                // 受け取り処理
                OnReciveChatMessage?.Invoke(receive);
                ReceivedMessages.Add(receive);
            }
        }

        /// <summary>
        /// 送信メッセージのリクエスト
        /// </summary>
        /// <param name="request"></param>
        public void AddRequestSendMessage(DuplexChatSend request)
        {
            // リクエスト処理
            OnRequestChatMessage?.Invoke(request);
            RequestSendMessageQueue.Enqueue(request);
        }
    }
}