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
        const string _Section = "DOWNLOAD";

        static void Main(string[] targetFileNames)
        {
            var model = GoogleServiceModel.Instance;

            var result = model.Setup(Path.GetFullPath(_CredentialsFilePath));
            if (result != GoogleServiceModel.Result.Success)
            {
                Console.WriteLine("Setup Failed.");
                return;
            }

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
                var dataCollection = ini.DataList.Where(sec => sec.Item1 == _Section);
                targetFileNames = dataCollection
                    .Select(key => key.Item2)
                    .ToArray();
            }

            if (model.Download(targetFileNames) != GoogleServiceModel.Result.Success)
            {
                Console.WriteLine("Target files download failed.");
                foreach (var fileName in targetFileNames) Console.WriteLine(fileName);
            }

            if (targetFileNames.All(name => string.IsNullOrEmpty(name)))
            {
                Console.WriteLine("Target files none.");
            }

            Console.Read();
        }
    }
}