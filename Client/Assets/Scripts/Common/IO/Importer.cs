using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Expansion;

namespace IO
{
    public abstract class Importer
    {
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        public bool IsComplete { get; protected set; } = false;
        protected virtual async Task ImportAsync(string path, Action<string> readLineMethod, SynchronizationContext context = null, Action postCallback = null)
        {
            if (!File.Exists(path))
            {
                //ファイルがない.
                IsComplete = true;
                return;
            }

            IsComplete = false;
            try
            {
                await semaphore.WaitAsync();
                using (var io = new StreamReader(path, Encoding.GetEncoding("Shift_JIS")))
                {
                    var line = string.Empty;
                    while ((line = await io.ReadLineAsync()) != null)
                    {
                        Debug.Log($"<color=yellow>PATH:{path}</color>");
                        //Debug.Log($"<color=yellow>ID:{Thread.CurrentThread.ManagedThreadId}</color>");
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
                IsComplete = true;
                semaphore.Release();
            }
            Debug.Log($"<color=orange>ID:{Thread.CurrentThread.ManagedThreadId}</color>");

#if UNITY_EDITOR
            // 関数が設定されているのにコンテキストが設定されてないせいで呼び出せない.
            if (postCallback != null && context == null)
            {
                Debug.LogWarning($"ImportWarning: Invalid callback call. > context is null.");
            }
#endif

            // スレッドを戻して処理をコールバック.
            if (context != null)
            {
                context.Post(_ => postCallback?.Invoke(), null);
            }
        }
    }
}