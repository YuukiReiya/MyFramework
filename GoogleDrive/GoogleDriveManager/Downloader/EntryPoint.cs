using System;
using System.IO;
using System.Linq;
using Model;
using System.Xml.Linq;
using Resources.XML;
using PE = PathExpansion;

namespace Downloader
{
    class EntryPoint
    {

        static void Main(string[] args)
        {
            var model = GoogleServiceModel.Instance;
            var documentXMLPath = args.Length > 0 ? args[0] : string.Empty;
            var isOverwrite = args.Length > 1 ? Convert.ToBoolean(args[1]) : false;
            if (args.Length > 1)
            {
                var arg = args[1];
            }
            if (string.IsNullOrEmpty(documentXMLPath))
            {
                //documentXMLPath = PE.Convert(Path.Combine(PE.GoogleDrive, $"res/config.xml"));
                documentXMLPath = PE.Convert(Path.Combine(PE.GoogleDrive, $"res/project_setting.xml"));
            }
            documentXMLPath = PE.Convert(documentXMLPath);
            if (!File.Exists(documentXMLPath))
            {
                Console.WriteLine($"Not found xml file. > {Path.GetFullPath(documentXMLPath)}");
                Console.ReadKey();
                return;
            }

            var doc = XDocument.Load(documentXMLPath);
            var root = doc.Element(Config._RootTag);
            var credentialPath = PE.Convert(root.Element(Config._CredentialsTag).Value);

            if (!File.Exists(credentialPath))
            {
                Console.WriteLine($"credentials file is not exist. > {credentialPath}{Environment.NewLine}[FullPath]:{Path.GetFullPath(credentialPath)}");
                Console.ReadKey();
                return;
            }

            var tokenFolderPath = PE.Convert(root.Element(Config._TokenTag).Value);

#if DEBUG//RELEASEビルドなら無い場合でもバイナリ直下に作る.
            if (!Directory.Exists(tokenFolderPath))
            {
                Console.WriteLine($"token.json folder is not exist. > {tokenFolderPath}{Environment.NewLine}[FullPath]:{Path.GetFullPath(tokenFolderPath)}");
                Console.ReadKey();
                return;
            }
#endif

            var result = model.Setup(Path.GetFullPath(credentialPath), Path.GetFullPath(tokenFolderPath));
            if (result != GoogleServiceModel.Result.Success)
            {
                Console.WriteLine("Setup Failed.");
                Console.ReadKey();
                return;
            }

            int pageSize;
            if (int.TryParse(root.Element(Config._PageSizeTag).Value, out pageSize))
            {
                GoogleServiceModel.PageSize = pageSize;
            }

            result = model.UpdateFilesCache();
            if (result != GoogleServiceModel.Result.Success)
            {
                Console.WriteLine($"Update Failed.");
                Console.ReadKey();
                return;
            }

#if DEBUG
            // Drive上にあるファイルをディレクトリ込みでパスのように表示する.
            model.ShowUploadedFiles();
#endif

            var targets = root.Element(Config._DownloadTag).Elements(Config._DataTag);
            var e = targets.GetEnumerator();
            if (!targets.Any())
            {
                // ダウンロード対象のデータが設定されていない（ダウンロードの必要がない）
                Console.WriteLine($"targets file is not exist.");
                Console.ReadKey();
            }

            var temp = PE.Convert(root.Element(Config._TempTag).Value);
            if (!Path.EndsInDirectorySeparator(temp))
            {
                // Unix(Linux)が'/'だった気がするので合わせる.
                //temp += Path.DirectorySeparatorChar;// 「\」
                temp += Path.AltDirectorySeparatorChar;// 「/」
            }

            // ダウンロード(一時保存)
            while (e.MoveNext())
            {
                var current = e.Current;
                var source = PE.Convert(current.Element(Config._SourceTag).Value);
                var destination = PE.Convert(current.Element(Config._DestinationTag).Value);

                // ダウンロード先のファイルがない.
                if (!model.IsExist(source))
                {
                    Console.WriteLine($"Download failed.{Environment.NewLine}Not found file! > {source}");
                    continue;
                }

                var downloadPath = Path.Combine(temp, Path.GetFileName(source));

                // 保存先のディレクトリが無ければ作る.
                var info = new DirectoryInfo(downloadPath);
                if (info.Parent != null && !info.Parent.Exists)
                {
                    Directory.CreateDirectory(info.Parent.FullName);
                }
                model.Download(source, downloadPath);

                // 解凍.
                ZipModel.Uncompression(downloadPath, destination, isOverwrite);
            }

            // 一時保存に使ったフォルダを削除.
            if (Directory.Exists(temp))
            {
                Directory.Delete(temp, true);
            }

            Console.Read();
        }
    }
}