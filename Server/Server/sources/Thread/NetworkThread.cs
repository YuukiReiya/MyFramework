using System;
using Grpc.Core;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Framework;
using Server.gRPC;

class NetworkThread : AbstractThread
{
    public NetworkThread(uint fps) : base(fps) { instance = this; }
    ~NetworkThread() { }

    private const string IPAddress = "127.0.0.1";
    private const int Port = 1122;

    private static NetworkThread instance { get; set; }

    #region protocol
    class PingImpl : Ping.PingBase
    {
        public override Task<S2C_Ping_Response> Reply(C2S_Ping_Request request, ServerCallContext context)
        {
            var ping = instance != null ? instance.DeltaTimeWatch.Elapsed.TotalMilliseconds : -1;
            Console.WriteLine($"ping:{ping}");
            return Task.FromResult(new S2C_Ping_Response
            {
                Ping = ping
            });
        }
    }
    #endregion

    public override void StartThread()
    {
        thread = new Thread(new ThreadStart(() => { base.StartThread(); }));
        Setup();
        try
        {
            Grpc.Core.Server server = new Grpc.Core.Server
            {
                Services = { Ping.BindService(new PingImpl()) },
                Ports = { new ServerPort(IPAddress, Port, ServerCredentials.Insecure) }
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