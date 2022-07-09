//Drive v3:https://developers.google.com/resources/api-libraries/documentation/drive/v3/csharp/latest/classGoogle_1_1Apis_1_1Drive_1_1v3_1_1FilesResource_1_1ListRequest.html#ab8206c847810cd19497e32fbff56378c
/* Quick Startのサンプル改良しただけ。
 * 参考URL:
 * https://developers.google.com/drive/api/v3/quickstart/dotnet
 * https://qiita.com/nori0__/items/dd5bbbf0b09ad58e40be
 */
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GDriveManager.src;

namespace DriveQuickstart
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "GDrive Management";
        static DriveService Service = null;

        /// <summary>
        /// 読み込んだデータ(ファイル/フォルダ)のIDと名前を紐づける
        /// </summary>
        static Dictionary<string/* FILE_ID */, string/* FILE_NAME */> StorageDataDic = new Dictionary<string, string>();

        /// <summary>
        /// リソースが入っているディレクトリ
        /// </summary>
        const string ResourcesDirectory = "res/";

        /// <summary>
        /// 読み込みファイル
        /// </summary>
        const string CredentialsFilePath = ResourcesDirectory + "credentials.json";

        /// <summary>
        /// 設定ファイル
        /// </summary>
        const string IniFilePath = ResourcesDirectory + "system.ini";

        /// <summary>
        /// セクション:表示
        /// </summary>
        const string ShowSection = "SHOW";

        /// <summary>
        /// セクション:ダウンロード
        /// </summary>
        const string DownloadSection = "DOWNLOAD";

        /// <summary>
        /// セクション:アップロード
        /// </summary>
        const string UploadSection = "UPLOAD";

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream(CredentialsFilePath, FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            //GoogleDriveサービス上の全フォルダ/ファイルを取得.
            FilesResource.ListRequest listRequest = Service.Files.List();

            //取得する際に設定項目でフィルタを掛ける
            listRequest.PageSize = 10;//ページサイズ
            listRequest.Fields = "nextPageToken, files(id, name)";

            #region 特定フォルダ
            string folderName = "ProjectSetting";
            //memo.Qはクエリのことっぽい。
            //listRequest.Q = $"(name = '{folderName}') ";

            #endregion

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            Console.WriteLine("Files:");
            if (files.Any())
            {
                StorageDataDic.Clear();
                StorageDataDic = files.ToDictionary(file => file.Id, file => file.Name);
            }
            else
            {
                Console.WriteLine("No files found.");
                Console.Read();
                return;
            }

            /*
             *　iniファイル読み込み
             */
            var ini = new IniReader(IniFilePath);
            
            //表示フラグ
            string showFlag = "0";
            if (ini.DataList.Any(e => e.Item1 == ShowSection))
            {
                showFlag = ini.DataList.FindLast(e => e.Item1 == ShowSection).Item2;
                if (showFlag != "0")
                {
                    foreach (var data in StorageDataDic)
                    {
                        Show(data.Key, data.Value);
                    }
                }
            }

            //ダウンロード
            if (ini.DataList.Any(e => e.Item1 == DownloadSection))
            {
                foreach (var data in ini.DataList.Where(e => e.Item1 == DownloadSection))
                {
                    if (!StorageDataDic.Any(e => e.Value == data.Item2)) continue;
                    // ファイル名がiniの[DOWNLOAD]セクションに入っているものか
                    var file = StorageDataDic.First(e => e.Value == data.Item2);

                    if (showFlag != "0")
                    {
                        Console.WriteLine($"{Environment.NewLine}DOWNLOAD_DICTIONARY:{data.Item3}\nFULL_PATH:{Path.GetFullPath(data.Item3)}{Environment.NewLine}");
                    }
                    //ダウンロード
                    Download(file.Key, data.Item3);
                }
            }
        }

        /// <summary>
        /// 表示
        /// </summary>
        /// <format>"<FILE_ID>:<FILE_NAME>"</format>
        /// <param name="id"></param>
        /// <param name="fileName"></param>
        private static void Show(string id, string fileName)
        {
            Console.WriteLine($"{id}:{fileName}");
        }

        /// <summary>
        /// ダウンロード
        /// memo.保存名はファイルと同じ
        /// </summary>
        /// <param name="id">ファイルID</param>
        /// <param name="downloadDir">ファイルの保存先ディレクトリ</param>
        private static void Download(string id,string downloadDir)
        {
            // ディレクトリが無ければ作る
            if (!string.IsNullOrEmpty(downloadDir) && !Directory.Exists(downloadDir))
            {
                Directory.CreateDirectory(downloadDir);
            }
            var request = Service.Files.Get(id);
            var stream = new FileStream(
                Path.Combine(downloadDir, StorageDataDic[id]),
                FileMode.Create,
                FileAccess.Write);
            request.Download(stream);
        }

        /// <summary>
        /// アップロード
        /// memo.現状必要ないので未実装
        /// </summary>
        /// <param name="id"></param>
        private static void Upload(string id)
        {

        }
    }
}