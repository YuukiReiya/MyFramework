//参考：https://qiita.com/iwatuki/items/37dcd2e853b192cbce25
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

// エラーハンドル.
//https://www.milk-island.net/translate/ggd/drive/api/v3/handle-errors.html

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
        public static List<Google.Apis.Drive.v3.Data.File> DriveFiles = new List<Google.Apis.Drive.v3.Data.File>();
        // Driveのファイルの表示数.
        // ※この数を基準に情報を取得しているので、GetFileなどのメソッドも存在するのに取得できないことがある。
        public static int PageSize { get; set; } = _DefaultPageSize;

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
        const int _DefaultPageSize = 10;
        //クエリに仕込めるプロパティ群
        //参考:https://developers.google.com/resources/api-libraries/documentation/drive/v3/csharp/latest/classGoogle_1_1Apis_1_1Drive_1_1v3_1_1Data_1_1File.html
        const string _PropertyFields =
        // プロパティ全群
        //"nextPageToken, files(AppProperties, Capabilities, ContentHints, CopyRequiresWriterPermission, CreatedTime, Description," +
        //" DriveId, ExplicitlyTrashed, ExportLinks, FileExtension, FolderColorRgb, FullFileExtension, HasAugmentedPermissions, HasThumbnail," +
        //" HeadRevisionId, IconLink, Id, ImageMediaMetadata, IsAppAuthorized, Kind, LastModifyingUser, Md5Checksum, MimeType, ModifiedByMe," +
        //" ModifiedByMeTimeRaw, ModifiedByMeTime, ModifiedTimeRaw, ModifiedTime, Name, OriginalFilename, OwnedByMe, Owners, Parents, PermissionIds," +
        //" Permissions, Properties, QuotaBytesUsed, Shared, SharedWithMeTimeRaw, SharedWithMeTime, SharingUser, ShortcutDetails, Size, Spaces, Starred," +
        //" TeamDriveId, ThumbnailLink, ThumbnailVersion, Trashed, TrashedTimeRaw, TrashedTime, TrashingUser, Version, VideoMediaMetadata, ViewedByMe," +
        //" ViewedByMeTimeRaw, ViewedByMeTime, ViewersCanCopyContent, WebContentLink, WebViewLink, WritersCanShare, ETag)";
        // ローワーキャメルで登録出来たやつのみ抜粋↓
        "nextPageToken, files(appProperties, capabilities, contentHints, copyRequiresWriterPermission, createdTime, description," +
        "driveId, explicitlyTrashed, exportLinks, fileExtension, folderColorRgb, fullFileExtension, hasAugmentedPermissions, hasThumbnail," +
        "headRevisionId, iconLink, id, imageMediaMetadata, isAppAuthorized, kind, lastModifyingUser, md5Checksum, mimeType, modifiedByMe," +
        "modifiedByMeTime, modifiedTime, name, originalFilename, ownedByMe, owners, parents, permissionIds," +
        "permissions, properties, quotaBytesUsed, shared, sharedWithMeTime, sharingUser, shortcutDetails, size, spaces, starred," +
        "teamDriveId, thumbnailLink, thumbnailVersion, trashed, trashedTime, trashingUser, version, videoMediaMetadata, viewedByMe," +
        "viewedByMeTime, viewersCanCopyContent, webContentLink, webViewLink, writersCanShare)";

        public Result Setup(string credentialFilePath, string tokenFolderPath)
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
                    string credPath = tokenFolderPath;
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

        /// キャッシュ情報の更新
        public Result UpdateFilesCache()
        {
            if (_Service == null)
            {
                Console.WriteLine($"not initialized.");
                return Result.Fail;
            }

            var requestList = _Service.Files.List();
            requestList.PageSize = PageSize;
            requestList.Fields = _PropertyFields;
            DriveFiles = requestList.Execute().Files.ToList();
            return Result.Success;
        }

        public void ShowUploadedFiles()
        {
            if (_Service == null)
            {
                Console.WriteLine($"not initialized.");
                return;
            }
            foreach (var file in DriveFiles)
            {
                var path = GetFileFullName(file.Id);
                Console.WriteLine($"{path} : {file.Id}"); ;
            }
        }

        public void Download(string drivePath,string downloadPath)
        {
            var fileID = GetFileID(drivePath);
            if (!string.IsNullOrEmpty(fileID))
            {
                DownloadByFileID(fileID, downloadPath);
            }
        }

        private void DownloadByFileID(string id, string downloadPath)
        {
            if (_Service == null)
            {
                Console.WriteLine($"Not initialized.");
                return;
            }

            try
            {
                var request = _Service.Files.Get(id);
                var stream = new FileStream(
                    downloadPath,
                    FileMode.Create,
                    FileAccess.ReadWrite);
                request.Download(stream);
                stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType().Name}{Environment.NewLine}{e.Message}{Environment.NewLine}{Environment.NewLine}{e.StackTrace}");
                throw;
            }
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


        public string GetFileID(string filePath)
        {
            var targets = DriveFiles.Where(file => GetFileFullName(file.Id) == filePath);

            var count = targets.Count();
            if (count == 1)
            {
                try
                {
                    var result = targets.First().Id;
                    return result;
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine($"{e.GetType()}:{Environment.NewLine}{e.Message}{Environment.NewLine}LinQ:First not found target. > {filePath}");
                    return string.Empty;
                }
            }
            else if (count > 1)
            {
                // 同じパスのファイルが複数存在してしまっている.
                Console.WriteLine($"Multiple path with the same path. > {filePath}");
            }
            else
            {
                Console.WriteLine($"Not found target. > {filePath}");
            }
            return string.Empty;
        }

        public string GetFileFullName(string fileId)
        {
            var file = GetFile(fileId);
            if (file != null)
            {
                var parentId = file.Parents != null ? string.Join("", file.Parents.SelectMany(p => p)) : string.Empty;
                var filePath = file.Name;
                while (!string.IsNullOrEmpty(parentId))
                {
                    var parentFile = DriveFiles.Where(x => x != null).FirstOrDefault(y => y.Id == parentId);
                    if (parentFile == null) break;
                    filePath = string.Concat(parentFile.Name + "/", filePath);
                    parentId = parentFile.Parents != null ? string.Join("", parentFile.Parents.SelectMany(p => p)) : string.Empty;
                }
                return filePath;
            }
            return string.Empty;
        }

        public Google.Apis.Drive.v3.Data.File GetFile(string id)
        {
            var count = DriveFiles.Count(file => file.Id == id);
            if (count == 1)
            {
                try
                {
                    var target = DriveFiles.First(file => file.Id == id);
                    return target;
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine($"{e.GetType()}:{Environment.NewLine}{e.Message}{Environment.NewLine}LinQ:First not found target. > {id}");
                    return null;
                }
            }
            else if (count == 0)
            {
                Console.WriteLine($"Not found target. > {id}");
            }
            else if (count > 1)
            {
                Console.WriteLine($"Multiple IDs with the same id. > {id}");
            }

            return null;
        }

        //Cacheを見るので必要に応じて呼び出し前にキャッシュを更新する必要がある.
        // ※ゴミ箱の中身にあっても判定.
        public bool IsExist(string driveFilePath)
        {
            if (_Service == null)
            {
                Console.WriteLine($"Not initialized.");
                return false;
            }

            return DriveFiles != null && DriveFiles.Any(file => GetFileFullName(file.Id) == driveFilePath);
        }

        public void Delete(string driveFilePath)
        {
            var fileID = GetFileID(driveFilePath);
            if (!string.IsNullOrEmpty(fileID))
            {
                DeleteForFileID(fileID);
            }
        }

        // 完全削除.
        // (ゴミ箱には行かない)
        private void DeleteForFileID(string fileID)
        {
            if (_Service == null)
            {
                Console.WriteLine($"Not initialized.");
                return;
            }
            try
            {
                var request = _Service.Files.Delete(fileID);
                request.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType().Name}{Environment.NewLine}{e.Message}{Environment.NewLine}{Environment.NewLine}{e.StackTrace}");
                throw;
            }
        }

        public void ShowUploadedList()
        {
            if (_Service == null)
            {
                Console.WriteLine($"Not initialized.");
                return ;
            }
            var filesListRequest = _Service.Files.List();
            filesListRequest.PageSize = PageSize;
            // name:拡張子まで
            //filesListRequest.Fields = "nextPageToken, files(id, name, createdTime, mimeType)";
            filesListRequest.Fields = "files(id, name, parents)";

            var files = filesListRequest.Execute().Files;

            if (files != null && files.Any())
            {
                try
                {
                    List<string> list = new List<string>();
                    foreach (var f in files)
                    {
                        var parentName = f.Parents != null ? string.Join("", f.Parents.SelectMany(p => p)) : "null";

                        // 出力情報をパスに変換する (parentにはidが入るので変換)
                        var parentId = f.Parents != null ? string.Join("", f.Parents.SelectMany(p => p)) : string.Empty;
                        var path = f.Name;

                        while (!string.IsNullOrEmpty(parentId))
                        {
                            var parentFile = files.Where(x => x != null).FirstOrDefault(y => y.Id == parentId);
                            if (parentFile == null) break;
                            path = string.Concat(parentFile.Name + "/", path);
                            parentId = parentFile.Parents != null ? string.Join("", parentFile.Parents.SelectMany(p => p)) : string.Empty;
                        }
                        list.Add(path);
                    }
                    foreach(var path in list.OrderBy(p => p)) Console.WriteLine($"{path}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.GetType().Name}:{Environment.NewLine}{e.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// 参考:https://stackoverflow.com/questions/46515805/file-upload-or-folder-creation-using-google-drive-api-v3-c-sharp
        /// memo.
        /// 同じ名前のファイルも複数挙げられてしまうみたいなので重複名でアップロードされないよう呼び出し先で管理推奨.
        /// </summary>
        /// <param name="path">ローカル(アップロード元)ファイルのパス</param>
        /// <param name="drivePath">クラウド(アップロード先)ファイルのパス</param>
        public void Upload(string path, string drivePath = "")
        {
            var io = new Google.Apis.Drive.v3.Data.File();
            //「System.Webは、プラットフォーム固有のAPIに依存しすぎるため、.NetCoreに移行しません。 Microsoftのリファレンスソースをご覧ください。」ってことらしい。
            // MIMEはレジストリから読み取る手法を取る。
            //var mime = System.Web.MimeMapping.GetMimeMapping(path);
            var mime = GetMime(path);
            if (mime == null)
            {
                Console.WriteLine($"Failed to upload. > {path} not get mime type.");
                return;
            }

            // アップロード元ファイルが存在するかチェック.
            if (!System.IO.File.Exists(path))
            {
                Console.WriteLine($"Failed to upload. > {path} not found source file.");
                return;
            }

            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(path, FileMode.Open))
            {
                request = _Service.Files.Create(io, stream, mime);

                // ファイル名.
                io.Name = Path.GetFileName(drivePath);
                // 親のフォルダ情報を登録する.
                //※既にあるフォルダなら親に出来る
                //　無ければルート.
                var index = string.IsNullOrEmpty(drivePath) ? -1 : drivePath.Replace("\\", "/").LastIndexOf("/");
                if (index != -1)
                {
                    var directory = drivePath.Substring(0, index);
                    var id = GetFileID(directory);
                    var parent = GetFile(id);
                    if (parent != null)
                    {
                        io.Parents = new string[] { parent.Id };
                    }
                }
                request.Fields = _PropertyFields;
                Google.Apis.Upload.IUploadProgress progress = null;
                
                try
                {
                    progress = request.Upload();
                    do { } while (progress.Status == Google.Apis.Upload.UploadStatus.Uploading);
                    Console.WriteLine($"upload state:{Google.Apis.Upload.UploadStatus.Completed}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.GetType()}:{Environment.NewLine}{e.Message}{Environment.NewLine}upload state:{progress.Status}");
                    return;
                }
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
            Console.WriteLine($"Failed to get mime. > {fileName}");
            return null;
        }
    }
}
