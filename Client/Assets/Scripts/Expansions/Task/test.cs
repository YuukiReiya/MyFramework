using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    CancellationTokenSource cts = new CancellationTokenSource();
    // Start is called before the first frame update
    Task task;
    void Start()
    {
        //task = Task.Run(process, cts.Token);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async Task process()
    {
        await Task.Delay(5 * 1000);
        Debug.Log("5秒");
        await Task.Delay(5 * 1000);
        Debug.Log("10秒");
        await Task.Delay(10 * 1000);
        Debug.Log("20秒");
        Debug.Log("End");
    }

    private void OnApplicationQuit()
    {
        //Debug.Log($"{cts}.cancel命令."); 
        //cts.Cancel();
        //cts.Dispose();
        //task.Wait();
        //Debug.Log("停止したよ");
    }
}
