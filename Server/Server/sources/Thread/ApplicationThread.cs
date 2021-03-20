using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Framework;

/// <summary>
/// ゲーム(アプリケーション)の処理を担当するスレッド
/// </summary>
class ApplicationThread : AbstractThread
{
    public ApplicationThread(uint fps) : base(fps) { }


    public override void StartThread()
    {
        //Appスレッドはメインスレッドで稼働させる
        Console.WriteLine("メインスレッド");
        thread = Thread.CurrentThread;
        base.StartThread();
    }

    public override void Process()
    {
    }
}