using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Grpc.Core;
using Server.gRPC;
using Grpc;
namespace Sample
{
    public class ClientSample : MonoBehaviour
    {
        Action S2C_Recive = null;
        Channel channel;
        Server.gRPC.Ping.PingClient client;

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
        }

        /// <summary>
        /// 
        /// </summary>
        void OnUpdate()
        {
            if (client != null && channel.State != ChannelState.TransientFailure)
            {
                try
                {
                    var ping = client.Reply(new C2S_Ping_Request());
                    if (ping != null)
                    {
                        Debug.Log($"ping:{ping.Ping }");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"<color=red>例外</color>{e.Message}");
                    throw;
                }
            }
        }

        private void Connect()
        {
            try
            {
                channel = new Channel(IPAddress, Port, ChannelCredentials.Insecure);
                client = new Server.gRPC.Ping.PingClient(channel);
                S2C_Recive = OnUpdate;
            }
            catch (Exception e)
            {
                Debug.LogError($"<color=red>例外</color>{e.Message}");
                throw;
            }

        }
    }
}