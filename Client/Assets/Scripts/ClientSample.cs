using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Grpc.Core;
using Server.gRPC;
using Grpc;
using System.Threading.Tasks;

/*
 * Taskで実行した場合終了時にshutdownしないとEditor上でも動き続ける
 */

namespace Sample
{
    public class ClientSample : MonoBehaviour
    {
        Action S2C_Recive = null;
        Channel channel;
        Unary.UnaryClient unaryClient;
        BidirectionalStreaming.BidirectionalStreamingClient bidirectionalStreamingClient;

        Task duplexChatReciveTask = null;
        uint cnt = 0;
        bool isRun = false;
        public string IPAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 1122;

        // Start is called before the first frame update
        void Start()
        {
            Connect();
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log($"state:{(channel != null ? channel.State.ToString() : "null")}");
            S2C_Recive?.Invoke();
            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log("「B」");
                cnt++;
                DuplexChatSend request = new DuplexChatSend { UserID = 0, Message = $"Count={cnt}" };
                ChatRequests.Add(request);
            }
        }

        //IEnumerable<DuplexChatSend> ChatRequests = new List<DuplexChatSend>();
        List<DuplexChatSend> ChatRequests = new List<DuplexChatSend>();
        /// <summary>
        /// 
        /// </summary>
        void OnUpdate()
        {
            //OnUpdateChat().Wait();
            OnUpdateChat().Wait(5);
            //OnUpdatePing();

        }

        void OnUpdatePing()
        {
            if (unaryClient != null && channel.State != ChannelState.TransientFailure)
            {
                try
                {
                    var ping = unaryClient.Ping(new C2S_Ping_Request());
                    if (ping != null)
                    {
                        Debug.Log($"ping:{ping.Ping }");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"<color=red>Ping</color>:{e.GetType()}\n{e.Message}\n{e.StackTrace}");
                    throw;
                }
            }
        }
        
        async Task OnUpdateChat()
        {
            if (bidirectionalStreamingClient == null || channel.State == ChannelState.TransientFailure)
            {
                return;
            }
            try
            {
                using (var call = bidirectionalStreamingClient.DuplexChat())
                {
                    //if (duplexChatReciveTask != null && duplexChatReciveTask.Status == TaskStatus.Running) return;
                    //if (duplexChatReciveTask != null && duplexChatReciveTask.Status == TaskStatus.Running) return;

                    //受信
                    duplexChatReciveTask = Task.Run(async () =>
                    {
                        while (await call.ResponseStream.MoveNext())
                        {
                            var current = call.ResponseStream.Current;
                            Debug.Log($"Recive\n{current.UserID}:{current.Message}");
                        }
                        await call.ResponseHeadersAsync;
                        //call.ResponseHeadersAsync.IsCompleted
                    });
                    //if (isRun) return;
                    //送信
                    int i = 0;
                    while (i < ChatRequests.Count)
                    {
                        var request = ChatRequests[i];
                        Debug.Log($"Send\n{request.UserID}:{request.Message}");

                        await call.RequestStream.WriteAsync(request);
                        i++;
                    }
                    ChatRequests.Clear();
                    await call.RequestStream.CompleteAsync();
                    await duplexChatReciveTask;
                    //receiveTask.Dispose();
                    
                    Debug.Log("チャット終了");
                }

            }
            catch (Exception e)
            {
                Debug.LogError($"<color=red>Chat</color>:{e.GetType()}\n{e.Message}\n{e.StackTrace}");
                duplexChatReciveTask.Dispose();
                Disconnect();
                throw;
            }
        }

        private void Connect()
        {
            try
            {
                channel = new Channel(IPAddress, Port, ChannelCredentials.Insecure
                    //, new List<ChannelOption>
                    //{
                    //    new ChannelOption(ChannelOptions.MaxReceiveMessageLength, 8388608/2), 
                    //}
                    );
                bidirectionalStreamingClient = new BidirectionalStreaming.BidirectionalStreamingClient(channel);
                unaryClient = new Unary.UnaryClient(channel);
                S2C_Recive = OnUpdate;
            }
            catch (Exception e)
            {
                Debug.LogError($"<color=red>Connect</color>:{e.GetType()}\n{e.Message}\n{e.StackTrace}");
                throw;
            }

        }

        private void Disconnect()
        {
            Debug.Log("<color=green>Disconnect</color>");
            channel.ShutdownAsync().Dispose();
            duplexChatReciveTask.Dispose();
        }
    }
}