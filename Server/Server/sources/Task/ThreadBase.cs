using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Framework
{
    abstract class ThreadBase
    {
        public Task task { get; private set; }
        public CancellationTokenSource tokenSource = new CancellationTokenSource();
        public uint TargetFrameRate { get; }
        
        private double TargetFrameMilliSec { get { return 1000f / TargetFrameRate; } }
        private double prevTick, nowTick;
        private double elapsedTime;
        public Stopwatch DeltaTimeWatch { get; private set; } = new Stopwatch();

        protected SynchronizationContext context;

#if DEBUG || _DEBUG
        /// <summary>
        /// スレッドIDのCLI上出力フラグ
        /// </summary>
        protected bool IsDisplayThreadID { get; set; } = false;
#endif
        /// <summary>
        /// ループ処理の中断フラグ
        /// </summary>
        public bool IsSuspensionProcess = false;

        /// <summary>
        /// FPS制限無しフラグ
        /// memo.trueだとFPS同期をしない
        /// </summary>
        public bool IsUnlimitedFPS { get; set; } = false;

        public bool IsStopThread
        {
            set 
            {
                if (value)
                {
                    tokenSource.Cancel();
                }
            }
        }

        public ThreadBase(uint fps)
        {
            TargetFrameRate = fps == 0 ? 1 : fps;//0除算回避
            elapsedTime = 0;
        }

        ~ThreadBase()
        {
            tokenSource.Cancel();
            Console.WriteLine("スレッド停止");
        }

        protected virtual void Setup()
        {
            prevTick = Environment.TickCount;
            IsSuspensionProcess = false;
            context = SynchronizationContext.Current;
        }

        protected void Loop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                //  現在の時間
                nowTick = Environment.TickCount;

                DeltaTimeWatch.Reset();
                DeltaTimeWatch.Start();

                //  中断フラグが立ってなければ処理する
                if (!IsSuspensionProcess)
                {
#if DEBUG || _DEBUG
                    if (IsDisplayThreadID) DisplayThreadID();
#endif

                    //  処理
                    Process();
                }

                //  フレーム待機
                if (!IsUnlimitedFPS) Sleep();

                //  カウント更新
                prevTick = Environment.TickCount;
            }
        }

        /// <summary>
        /// スレッドを起動
        /// </summary>
        public virtual void Start()
        {
            task = Task.Run(() =>
            {
                Setup();
                Loop(tokenSource.Token);
            },tokenSource.Token);
        }

        /// <summary>
        /// スレッド内のループ処理で行う実処理
        /// </summary>
        public abstract void Process();

        /// <summary>
        /// フレームに合わせ待機
        /// </summary>
        private void Sleep()
        {
            //  経過時間
            elapsedTime = (nowTick - prevTick) / TargetFrameRate;
            bool isSleep = elapsedTime < TargetFrameMilliSec;
            if (isSleep)
            {
                var sleepTime = (int)(TargetFrameMilliSec - elapsedTime);
                Thread.Sleep(sleepTime);
            }
        }

#if DEBUG||_DEBUG
        private void DisplayThreadID()
        {
            Console.WriteLine($"class:{this.GetType()} ThreadID:{Thread.CurrentThread.ManagedThreadId}");
        }
#endif
    }
}
