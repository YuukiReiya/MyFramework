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
                //�t�@�C�����Ȃ�.
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
            // �֐����ݒ肳��Ă���̂ɃR���e�L�X�g���ݒ肳��ĂȂ������ŌĂяo���Ȃ�.
            if (postCallback != null && context == null)
            {
                Debug.LogWarning($"ImportWarning: Invalid callback call. > context is null.");
            }
#endif

            // �X���b�h��߂��ď������R�[���o�b�N.
            if (context != null)
            {
                context.Post(_ => postCallback?.Invoke(), null);
            }
        }
    }
}