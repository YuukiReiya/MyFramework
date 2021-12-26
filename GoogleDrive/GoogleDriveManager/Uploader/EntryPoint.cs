using System;
using System.IO;
using System.Linq;
using Model;
using System.Xml.Linq;
using Resources.XML;

namespace Uploader
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
                Console.ReadKey();
                return;
            }

            // 表示数.
            GoogleServiceModel.PageSize = 15;

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

            var targets = root.Element(Config._UploadTag).Elements(Config._DataTag);
            var e = targets.GetEnumerator();
            if (!targets.Any())
            {
                // アップロード対象のデータが設定されていない（アップロードの必要がない）
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
            
            // 圧縮.
            while (e.MoveNext())
            {
                var current = e.Current;
                var source = current.Element(Config._SourceTag).Value;
                var destination = current.Element(Config._DestinationTag).Value;

                // 書き出し先
                var destArcPath = temp + Path.GetFileName(source);

                // ディレクトリを用意しとく.
                var info = new DirectoryInfo(destArcPath);
                if (info.Parent != null && !info.Parent.Exists)
                {
                    Directory.CreateDirectory(info.Parent.FullName);
                }

                // 圧縮前にアーカイブが書き出されていたら削除.
                var destArcPathWithExt = Path.ChangeExtension(Path.GetFullPath(destArcPath), ZipModel.Extension);
                if (File.Exists(Path.GetFullPath(destArcPathWithExt)))
                {
                    File.Delete(Path.GetFullPath(destArcPathWithExt));
                    Console.WriteLine($"Archive file is already exists. > delete:{Path.GetFullPath(destArcPathWithExt)}");
                }

                // 該当ファイルを圧縮し書き出す.
                ZipModel.Compression(source, destArcPath);

                // 既にフォルダがあれば削除してから挙げなおす.
                if (model.IsExist(destination))
                {
                    model.Delete(destination);
                }

                model.Upload(destArcPathWithExt, destination);
            }
            Console.ReadKey();
        }
    }
}
