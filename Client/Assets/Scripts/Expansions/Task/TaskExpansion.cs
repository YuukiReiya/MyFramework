using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace Expansion
{
    public static class TaskExpansion
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        // タスクの実行命令.
        public static async Task Execute(this Task task, Action postProcess = null)
        {
            try
            {
                await semaphore.WaitAsync();
                Debug.Log($"<color=green>ID:{Thread.CurrentThread.ManagedThreadId}</color>");
                await task;
                postProcess?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.GetType()}:{e.Message}\n{e.StackTrace}");
                throw;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}