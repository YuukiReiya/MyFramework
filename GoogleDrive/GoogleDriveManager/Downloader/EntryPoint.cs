using System;
using System.IO;
using System.Linq;
using Model;
using Ini;

namespace Downloader
{
    class EntryPoint
    {
        const string _CredentialsFilePath = @"./res/credentials.json";
        const string _TokenFolderPath = @"./res/token.json";
        const string _IniFilePath = @"./res/system.ini";
        const string _TempArchiveDirectory = @"./temp/";
        const string _DownloadSection = "DOWNLOAD";
        const string _UncompressionSection = "UNCOMPRESSION";

        static void Main(string[] resourcesDirectory)
        {
            var model = GoogleServiceModel.Instance;

            if (resourcesDirectory.Length < 1)
            {
                //早期リターン.
                //Console.WriteLine($"invalid application argments. [0]:resource root directory.");
                //return;

                //NOTE: bin\$(Configuration)\net5.0\*.exe
                resourcesDirectory = new[] { $"../../../" };
            }

            var resDir = resourcesDirectory[0];
            var credentialPath = resDir + _CredentialsFilePath;

            if (!File.Exists(credentialPath))
            {
                Console.WriteLine($"credentials file is not exist. > {credentialPath}{Environment.NewLine}[FullPath]:{Path.GetFullPath(credentialPath)}");
                Console.ReadKey();
                return;
            }

            var tokenFolderPath = resDir + _TokenFolderPath;

#if DEBUG//RELEASEビルドなら無い場合でもバイナリ直下に作る.
            if (!Directory.Exists(tokenFolderPath))
            {
                Console.WriteLine($"token.json folder is not exist. > {tokenFolderPath}{Environment.NewLine}[FullPath]:{Path.GetFullPath(tokenFolderPath)}");
                Console.ReadKey();
                return;
            }
#endif

            var iniPath = resDir + _IniFilePath;
            if (!File.Exists(iniPath))
            {
                Console.WriteLine($"ini file is not exist. > {iniPath}");
                Console.ReadKey();
                return;
            }

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

            // 引数無し実行なら.iniから読み込む
            var ini = new IniReader(Path.GetFullPath(iniPath),
#if DEBUG
                    true
#else
                    false
#endif
                    );
            var e = ini.DataList.Where(sec => sec.Item1 == _DownloadSection).GetEnumerator();

            // ダウンロード(一時保存)
            while (e.MoveNext())
            {
                var current = e.Current;
                var drivePath = current.Item3;

                // ダウンロード先のファイルがない.
                if (!model.IsExist(drivePath))
                {
                    Console.WriteLine($"Download failed.{Environment.NewLine}Not found file! > {drivePath}");
                    continue;
                }

                var downloadPath = _TempArchiveDirectory + drivePath;

                // 保存先のディレクトリが無ければ作る.
                var info = new DirectoryInfo(downloadPath);
                if (info.Parent != null && !info.Parent.Exists)
                {
                    Directory.CreateDirectory(info.Parent.FullName);
                }
                model.Download(drivePath, downloadPath);

                // 解凍.
                var target = ini.DataList.FirstOrDefault(data => data.Item1 == _UncompressionSection && data.Item2 == current.Item2);
                if (target != default)
                {
                    ZipModel.Uncompression(downloadPath, target.Item3);
                }
            }
            Console.Read();
        }
    }
}