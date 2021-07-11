#define TeX

using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;


namespace Model
{
    public class GoogleServiceModel
    {
        public enum Result : byte
        {
            Success = 0,
            Fail = 1,
        }

        //https://www.kyoto-su.ac.jp/ccinfo/use_web/mine_contenttype/
        //↑より良さそうな参考:https://webbibouroku.com/Blog/Article/asp-mimetype
        //.Net4.5以降なら拡張子からMIMEの取得できるみたい。
        //        private readonly List<(string/* 拡張子 */, string/* MIME Content-Type */)> mimeContentTypes = new List<(string, string)>
        //        {
        //            // テキスト/ドキュメント
        //            /* HTML */
        //            ("html", "text/html"),
        //            ("htm", "text/html"),
        //            /* LaTeX */
        //#if !TeX
        //            ("tex","application/x-latex"),
        //#endif
        //            ("latex","application/x-latex"),
        //            ("ltx","application/x-latex"),
        //            /* PDF */
        //            ("pdf","application/pdf"),
        //            /* Postscript */
        //            ("ps","application/postscript"),
        //            /* Rich Text Format */
        //            ("rtf","application/rtf"),
        //            /* SGML */
        //            ("sgm","text/sgml"),
        //            ("sgml","text/sgml"),
        //            /* TSV／タブ区切り形式 */
        //            ("tab","text/tab-separated-values"),
        //            ("tsv","text/tab-separated-values"),
        //#if TeX
        //            /* TeX */
        //            ("tex","application/x-tex"),
        //#endif
        //            /* Text */
        //            ("txt","text/plain"),
        //            /* XML */
        //            ("xml","text/xml"),

        //            // 圧縮
        //            /* JAR */
        //            ("jar","application/java-archiver"),
        //            /* Compact Pro */
        //            ("cpt","	application/mac-compactpro"),
        //            /*  */
        //            ("",""),
        //            /*  */
        //            ("",""),
        //            /*  */
        //            ("",""),
        //            /*  */
        //            ("",""),
        //            /*  */
        //            ("",""),
        //            /*  */
        //            ("",""),
        //            ("",""),
        //        };

        public static GoogleServiceModel Instance { get; } = new GoogleServiceModel();

        static readonly string[] _Scopes = 
        {
            DriveService.Scope.DriveReadonly,
            DriveService.Scope.Drive,
            DriveService.Scope.DriveAppdata,
            DriveService.Scope.DriveFile,
            DriveService.Scope.DriveMetadataReadonly,
            DriveService.Scope.DriveScripts,
        };
        static readonly string _ApplicationName = "My Google Drive";
        static DriveService _Service = null;
        const int _PageSize = 10;

        public Result Setup(string credentialFilePath)
        {
            UserCredential credential;

            if (!System.IO.File.Exists(credentialFilePath))
            {
                Console.WriteLine($"{credentialFilePath}\nFile is not found.");
                return Result.Fail;
            }

            try
            {
                using (var stream =
                    new FileStream(credentialFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        _Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)
                        ).Result;
                }
                _Service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = _ApplicationName,
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Credential:{credentialFilePath}");
                Console.WriteLine($"{e.GetType().Name}{Environment.NewLine}{e.Message}{Environment.NewLine}{Environment.NewLine}{e.StackTrace}");
                return Result.Fail;
            }
            return Result.Success;
        }

        public Result Download(IEnumerable<string> targetFilesName)
        {
            if (_Service == null) return Result.Fail;

            var filesListRequest = _Service.Files.List();
            filesListRequest.PageSize = _PageSize;
            // name:拡張子まで
            filesListRequest.Fields = "nextPageToken, files(id, name)";
            
            var files = filesListRequest.Execute().Files;

            if (!files.Any()) return Result.Fail;

            foreach(var name in targetFilesName)
            {
                var target = files.FirstOrDefault(file => file.Name == name);
                if (target != null)
                {
                    Download(target.Id, target.Name, "download");
                }
                else
                {
                    Console.WriteLine($"file is not find.{name}");
                }
            }

            return Result.Success;
        }

        private void Download(string id, string fileName, string downloadDir)
        {
            if (!string.IsNullOrEmpty(downloadDir) && !Directory.Exists(downloadDir))
            {
                Directory.CreateDirectory(downloadDir);
            }
            var request = _Service.Files.Get(id);
            var stream = new FileStream(
                Path.Combine(downloadDir, fileName),
                FileMode.Create,
                FileAccess.Write);
            request.Download(stream);
        }


        public Result Upload(IEnumerable<string> targets)
        {
            foreach(var path in targets)
            {
                Upload(path,"");
            }
            return Result.Success;
        }

        public bool IsExist(string driveFilePath)
        {
            if (_Service == null)
            {
                Console.WriteLine($"Not initialized.");
                return false;
            }
            var filesListRequest = _Service.Files.List();
            filesListRequest.PageSize = _PageSize;
            // name:拡張子まで
            //filesListRequest.Fields = "nextPageToken, files(id, name, createdTime, mimeType)";
            filesListRequest.Fields = "files(id, name)";

            

            var files = filesListRequest.Execute().Files;

            if (files != null && files.Any())
            {
                try
                {
                    foreach (var f in files)
                    {
                        var ps = f.Parents != null ? string.Join(",", f.Parents) : "null";
                        Console.WriteLine($"{f.Name} : {ps}");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"エラー:{e.Message}");
                    Console.WriteLine("failed.");
                  
                    throw;
                }


                return files.Any(file => file.Name == driveFilePath);
            }
            Console.WriteLine("failed.");
            return false;
        }

        /// <summary>
        /// 参考:https://stackoverflow.com/questions/46515805/file-upload-or-folder-creation-using-google-drive-api-v3-c-sharp
        /// </summary>
        private void Upload(string path,string parentFolderName)
        {
            var io = new Google.Apis.Drive.v3.Data.File();

            //「System.Webは、プラットフォーム固有のAPIに依存しすぎるため、.NetCoreに移行しません。 Microsoftのリファレンスソースをご覧ください。」ってことらしい。
            // MIMEはレジストリから読み取る手法を取る。
            //var mime = System.Web.MimeMapping.GetMimeMapping(path);
            var mime = GetMime(path);
            io.MimeType = mime;

            var filesListRequest = _Service.Files.List();
            filesListRequest.PageSize = _PageSize;
            // name:拡張子まで
            filesListRequest.Fields = "nextPageToken, files(id, name, createdTime, mimeType)";

            

            if (!System.IO.File.Exists(path))
            {
                Console.WriteLine($"file is not found! {path}");
                return;
            }

            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(path, FileMode.Open))
            {
                request = _Service.Files.Create(io, stream, mime);

                // ファイル名.
                io.Name = Path.GetFileName(path);
                
                //クエリに仕込めるプロパティ群
                //参考:https://developers.google.com/resources/api-libraries/documentation/drive/v3/csharp/latest/classGoogle_1_1Apis_1_1Drive_1_1v3_1_1Data_1_1File.html
                request.Fields = "id,name";
                var progress= request.Upload();
                do { } while (progress.Status == Google.Apis.Upload.UploadStatus.Uploading);

                Console.WriteLine($"upload state:{progress.Status}");

                // 中身の確認
                var data = request.ResponseBody;
                Console.WriteLine($"{data.Id},{data.Name},{data.FileExtension}");
            }
        }

        private string GetMime(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);
            var mimeType = key.GetValue("Content Type");
            if (mimeType != null)
            {
                return mimeType.ToString();
            }
            return null;
        }

        const char _Separator = '/';
        private IEnumerable<string> GetParent(string path)
        {
            var convPath = path.Replace('\\', _Separator);
            var parents = convPath.Split(_Separator);
            
            if (parents.Count() == 1) return parents;

            return parents;
        }
    }
}
