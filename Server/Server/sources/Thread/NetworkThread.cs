using System;
using System.Collections;
using System.Collections.Generic;
using Grpc.Core;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Framework;
using Server.gRPC;
using Server.Impl;

class NetworkThread : AbstractThread
{
    public NetworkThread(uint fps) : base(fps) { }
    ~NetworkThread() { }

    private const string IPAddress = "127.0.0.1";
    private const int Port = 1122;

    public override void StartThread()
    {
        thread = new Thread(new ThreadStart(() => { base.StartThread(); }));
        Setup();
        try
        {
            Grpc.Core.Server server = new Grpc.Core.Server(
                new List<ChannelOption>
                {
                    new ChannelOption(ChannelOptions.MaxReceiveMessageLength,8388608),
                })
            {
                Services =
                {
                    Unary.BindService(new UnaryImpl()),
                    ServerStreaming.BindService(new ServerStreamingImpl()),
                    ClientStreaming.BindService(new ClientStreamingImpl()),
                    BidirectionalStreaming.BindService(new BidirectionalStreamingImpl()),
                },
                Ports =
                {
                    new ServerPort(IPAddress, Port, ServerCredentials.Insecure)
                }

            };
            server.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine($"エラー\n{e.Message}");
        }
        thread.Start();
    }

    public override void Process()
    {
        //Console.WriteLine($"{instance.DeltaTimeWatch.Elapsed.TotalMilliseconds}");
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Setup()
    {

    }
}