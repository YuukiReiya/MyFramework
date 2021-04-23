using System;
using Grpc.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// 排他制御用セマフォ
    /// Task(async,await)で使うため'SemaphoreSlim'を起用している
    /// /// </summary>
    private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    /// <summary>
    /// 双方向チャット通信
    /// </summary>
    /// <param name="requestStream"></param>
    /// <param name="responseStream"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task DuplexChat(IAsyncStreamReader<DuplexChatSend> requestStream, IServerStreamWriter<DuplexChatReceive> responseStream, ServerCallContext context)
    {
        // ロック取れるまで待つ
        await semaphore.WaitAsync();

        try
        {
            while (await requestStream.MoveNext())
            {
                var current = requestStream.Current;

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
        finally
        {
            // ロックの解除
            semaphore.Release();
        }
    }
}
