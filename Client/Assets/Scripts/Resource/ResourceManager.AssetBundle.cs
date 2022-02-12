using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Resources
{
    public static partial class ResourceManager
    {
        public static Dictionary<string, AssetBundle> CacheDict { get; private set; } = new Dictionary<string, AssetBundle>();
        
        public static IEnumerator LoadAssetBundle(string url)
        {
            using (var request = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                yield return request.SendWebRequest();

                while (!request.isDone) yield return null;

               try
                {
                    switch (request.result)
                    {
                        // サーバーとの通信に失敗.
                        // Ex)通信失敗、セキュリティ保護
                        case UnityWebRequest.Result.ConnectionError:
                            throw new Exception($"Failed to communicate with the server.");

                        // サーバーのエラー応答を受信.
                        // リクエストは成功したがSV側でエラー.
                        case UnityWebRequest.Result.ProtocolError:
                            throw new Exception($"The server returned an error response.");

                        // データの処理中にエラーが発生.
                        //Ex) データの破損、正しい形式ではない.
                        case UnityWebRequest.Result.DataProcessingError:
                            throw new Exception("Error processing data.");
                        default:
                            break;
                    }
                    
                    AssetBundle asset = DownloadHandlerAssetBundle.GetContent(request);

                    var key = asset.name;
                    if (!CacheDict.ContainsKey(key))
                    {
                        CacheDict.Add(key, asset);
                    }
                    else
                    {
                        Debug.LogError($"\"{key}\" is already exists. from {url}");
                    }
                }
                catch (Exception e)
                {
                    request.Abort();
                    Debug.LogError($"<color=red>[try-catch]</color>{e.GetType().Name}:{e.Message}{Environment.NewLine}Failed to load asset bundle. > {url}{Environment.NewLine}{e.StackTrace}{Environment.NewLine}");
                    throw;
                }
            }

            yield break;
        }
    }
}