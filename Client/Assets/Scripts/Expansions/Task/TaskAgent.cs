using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Expansion
{
//    public class TaskAgent : IDisposable, IAsyncDisposable
//    {
//        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
//        private CancellationTokenSource cts = null;
//        public bool ThrowIfCancellationRequested { get; set; }
//        void IDisposable.Dispose()
//        {
//            cts.Cancel();
//        }

//        async ValueTask IAsyncDisposable.DisposeAsync()
//        {

//        }


//        public TaskAgent(bool throwIfCancellationRequested = false)
//        {
//            cts = new CancellationTokenSource();
//            ThrowIfCancellationRequested = throwIfCancellationRequested;
//        }

//        public async Task Execute(Action action)
//        {
//            await ExecuteImpl(action);
//        }

//        private async Task ExecuteImpl(Action action)
//        {
//            await semaphore.WaitAsync();
//            try
//            {
//                if (cts.IsCancellationRequested)
//                {
//                    if (ThrowIfCancellationRequested) cts.Token.ThrowIfCancellationRequested();
//                }
//                action?.Invoke();
//            }
//            catch (Exception e)
//            {
//                Debug.LogError($"{e.GetType()}:{e.Message}\n{e.StackTrace}");
//                throw;
//            }
//            finally
//            {
//                semaphore.Release();
//            }
//        }

//        //public async Task ExecuteE(Task task)
//        //{
//        //}

//        //public Task Execute(Task task)
//        //{
//        //    return Task.Run(() => { return task; }, cts.Token);
//        //}

//        //public Task<TResult> Execute<TResult>(Func<TResult> task)
//        //{
//        //    return Task.Factory.StartNew((TResult) => { return task.Invoke(); }, cts.Token);
//        //}
//    }
}