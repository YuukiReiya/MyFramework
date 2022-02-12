using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//WebClientを使ってFTPサーバーからファイルをダウンロード、アップロードする
//https://dobon.net/vb/dotnet/internet/ftpwebclient.html
//FTPのレスポンスコード一覧
//https://atmarkit.itmedia.co.jp/fnetwork/rensai/netpro10/ftp-responsecode.html
namespace Resources
{
    public static partial class ResourceManager
    {
        public static IEnumerator GetFileFromFTP(string uri,string filePath)
        {
            using (var client = new WebClient())
            {
                try
                {
                    client.Credentials = new NetworkCredential(
                        /* ユーザー名 */
                        "root",
                        /* パスワード */
                        "admin");

                    client.DownloadFile(uri, filePath);
                }
                catch (Exception e)
                {
                    Debug.LogError($"<color=red>[try-catch]</color>{e.GetType().Name}:{e.Message}{Environment.NewLine}Failed to load asset bundle. > {uri}{Environment.NewLine}{e.StackTrace}{Environment.NewLine}");
                    throw;
                }
            }
            yield break;
        }

    }
}