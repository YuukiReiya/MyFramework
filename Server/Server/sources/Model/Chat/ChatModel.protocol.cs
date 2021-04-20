using System;
using Grpc.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.gRPC;
using Chat;

namespace Chat
{
    partial class ChatModel
    {

    }
}

partial class BidirectionalStreamingImpl : BidirectionalStreaming.BidirectionalStreamingBase
{
    /// <summary>
    /// 双方向チャット通信
    /// </summary>
    /// <param name="requestStream"></param>
    /// <param name="responseStream"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task DuplexChat(IAsyncStreamReader<DuplexChatSend> requestStream, IServerStreamWriter<DuplexChatReceive> responseStream, ServerCallContext context)
    {
        try
        {
            while (await requestStream.MoveNext())
            {
                var current = requestStream.Current;

                // 許容メッセージ数を超過したら古いものから消していく。
                //if(ChatModel.Instance.Messages.Count>=ChatModel.SaveMessagesCount)
                //{
                //    ChatModel.Instance.Messages.Remove(ChatModel.Instance.Messages.First());
                //}
                //ChatModel.Instance.Messages.Add(new ChatModel.Info(current.UserID, current.Message));
                //for (int i = 0; i < ChatModel.Instance.Messages.Count; ++i)
                //{
                //    //var message = ChatModel.Instance.Messages[i];
                //    var message = ChatModel.Instance.Messages.ElementAt(i);
                //    Console.WriteLine($"{message.UserID}:{message.Message}");
                //    await responseStream.WriteAsync(new DuplexChatReceive { UserID = message.UserID, Message = message.Message });
                //}

                /*
                例外:System.InvalidOperationException
                Already finished.
                at Grpc.Core.Internal.AsyncCallServer`2.CheckSendAllowedOrEarlyResult()
                at Grpc.Core.Internal.AsyncCallBase`2.SendMessageInternalAsync(TWrite msg, WriteFlags writeFlags)
                at BidirectionalStreamingImpl.DuplexChat(IAsyncStreamReader`1 requestStream, IServerStreamWriter`1 responseStream, ServerCallContext context) 
                in ChatModel.protocol.cs:line 60

                例外:System.IO.IOException
                Error sending from server.
                at BidirectionalStreamingImpl.DuplexChat(IAsyncStreamReader`1 requestStream, IServerStreamWriter`1 responseStream, ServerCallContext context)
                in ChatModel.protocol.cs:line 60
                */

                // 許容メッセージ数を超過したら古いものから消していく。
                if (ChatModel.Instance.Messages.Count >= ChatModel.MessageCapacity)
                {
                    ChatModel.Instance.Messages.Dequeue();
                }
                ChatModel.Instance.Messages.Enqueue(new ChatModel.Info(current.UserID, current.Message));
                var e = ChatModel.Instance.Messages.GetEnumerator();
                while (e.MoveNext())
                {
                    var message = e.Current;
                    Console.WriteLine($"{message.UserID}:{message.Message}");
                    await responseStream.WriteAsync(new DuplexChatReceive { UserID = message.UserID, Message = message.Message });
                }
                Console.WriteLine("===============End Chat===============");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"例外:{e.GetType()}\n{e.Message}\n{e.StackTrace}");
            throw;
        }
    }
}
