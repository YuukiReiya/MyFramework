﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
namespace Masters
{
    public abstract class TableImporter
    {
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private CancellationTokenSource tokenSource;
        public TableImporter(CancellationTokenSource cancellationTokenSource)
        {
            tokenSource = cancellationTokenSource;
        }
        public abstract Task Execute(TableBase table, SynchronizationContext context = null, Action postCallback = null);
        protected virtual async Task ImportToTableAsync(TableBase table, Action<string> readLineMethod, SynchronizationContext context = null, Action postCallback = null)
        {
            if (table == null || !File.Exists(table.PathWithExtension))
            {
                return;
            }

            try
            {
                await semaphore.WaitAsync();
                Profiler.BeginThreadProfiling($"Async Task", $"Thread:{Thread.CurrentThread.ManagedThreadId}");
                using (var io = new StreamReader(table.PathWithExtension))
                {
                    var line = string.Empty;
                    while ((line = await io.ReadLineAsync()) != null)
                    {
                        if (tokenSource.IsCancellationRequested)
                        {
                            Debug.LogWarning($"<color=yellow>TaskWarning</color>:Asynchronous import of \"{table.PathWithExtension}\" {this.GetType()} is canceld.");
                            break;
                        }
                        readLineMethod?.Invoke(line);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.GetType()}:{e.Message}\n{e.StackTrace}");
                throw;
            }
            finally
            {
                semaphore.Release();
                Profiler.EndThreadProfiling();
            }

            // スレッドを戻して処理をコールバック.
            if (context != null)
            {
                context.Post(_ => postCallback?.Invoke(), null);
            }
        }
    }
}