using System;
using System.IO;
using System.Linq;
using Model;
using System.Xml.Linq;
using Resources.XML;

namespace Downloader
{
    class EntryPoint
    {

        static void Main(string[] args)
        {
            var model = GoogleServiceModel.Instance;
            var documentXMLPath = args.Length > 0 ? args[0] : string.Empty;
            if (string.IsNullOrEmpty(documentXMLPath))
            {
                documentXMLPath = "../../../res/config.xml";
            }

            if (!File.Exists(documentXMLPath))
            {
                Console.WriteLine($"Not found xml file. > {Path.GetFullPath(documentXMLPath)}");
                Console.ReadKey();
                return;
            }

            var doc = XDocument.Load(documentXMLPath);
            var root = doc.Element(Config._RootTag);
            var credentialPath = root.Element(Config._CredentialsTag).Value;

            if (!File.Exists(credentialPath))
            {
                Console.WriteLine($"credentials file is not exist. > {credentialPath}{Environment.NewLine}[FullPath]:{Path.GetFullPath(credentialPath)}");
                Console.ReadKey();
                return;
            }

            var tokenFolderPath = root.Element(Config._TokenTag).Value;

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
                return;
            }

            result = model.UpdateFilesCache();
            if (result != GoogleServiceModel.Result.Success)
            {
                Console.WriteLine($"Update Failed.");
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

            var temp = root.Element(Config._TempTag).Value;
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
                var source = current.Element(Config._SourceTag).Value;
                var destination = current.Element(Config._DestinationTag).Value;

                // ダウンロード先のファイルがない.
                if (!model.IsExist(source))
                {
                    Console.WriteLine($"Download failed.{Environment.NewLine}Not found file! > {source}");
                    continue;
                }

                var downloadPath = temp + Path.GetFileName(source);

                // 保存先のディレクトリが無ければ作る.
                var info = new DirectoryInfo(downloadPath);
                if (info.Parent != null && !info.Parent.Exists)
                {
                    Directory.CreateDirectory(info.Parent.FullName);
                }
                model.Download(source, downloadPath);

                // 解凍.
                ZipModel.Uncompression(downloadPath, destination);
            }

            // 一時保存に使ったフォルダを削除.
            if (Directory.Exists(temp))
            {
                Directory.Delete(temp, true);
                return;
            }

            Console.Read();
        }
    }
}