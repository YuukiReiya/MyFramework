using System;
using Grpc.Core;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.gRPC;

namespace Server.Impl
{
    class UnaryImpl : Unary.UnaryBase
    {
        S2C_Ping_Response pingReponse = new S2C_Ping_Response();

        public override Task<S2C_Ping_Response> Ping(C2S_Ping_Request request, ServerCallContext context)
        {
            double ping = 0;
            pingReponse.Ping = ping;
            Console.WriteLine($"ping:{ping}");
            return Task.FromResult(pingReponse);
        }
    }
}