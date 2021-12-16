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
        const string _IniFilePath = @"./res/system.ini";
        const string _TempArchiveDirectory = @"./temp/";
        const string _DownloadSection = "DOWNLOAD";
        const string _UncompressionSection = "UNCOMPRESSION";

        static void Main(string[] targetFileNames)
        {
            var model = GoogleServiceModel.Instance;

            var result = model.Setup(Path.GetFullPath(_CredentialsFilePath),"");
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

            var downloadFiles = targetFileNames;
            // 引数無し実行なら.iniから読み込む
            if (!downloadFiles.Any())
            {
                var ini = new IniReader(Path.GetFullPath(_IniFilePath),
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
            }
            Console.Read();
        }
    }
}