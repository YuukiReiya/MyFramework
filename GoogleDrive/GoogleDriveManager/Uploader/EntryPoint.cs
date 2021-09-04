using System;
using System.IO;
using System.Linq;
using Model;
using Ini;

namespace Uploader
{
    class EntryPoint
    {
        const string _CredentialsFilePath = @"./res/credentials.json";
        const string _IniFilePath = @"./res/system.ini";
        const string _TempArchiveDirectory = @"./temp/";
        const string _UnityClientSection = "COMMON";
        const string _UnityClientKey = "UNITY_PROJECT_PATH";
#if false
        // Upload方法は3種類あるけど、ぶっちゃけResumableだけでいい。
        const string _SectionHeader = "UPLOAD";
        readonly string[] _SectionTails = new string[] { "SIMPLE", "", "" };
#else
        const string _Section = "UPLOAD";
#endif
        static void Main(string[] targetNames)
        {
            var model = GoogleServiceModel.Instance;

            var result = model.Setup(Path.GetFullPath(_CredentialsFilePath));
            if (result != GoogleServiceModel.Result.Success)
            {
                Console.WriteLine("Setup Failed.");
                return;
            }

            var uploadFiles = targetNames;

            if (!uploadFiles.Any())
            {
                var ini = new IniReader(Path.GetFullPath(_IniFilePath),
#if DEBUG
                    true
#else
                    false
#endif
                    );
                var clientAssetsPath = ini.DataList.Find(data => data.Item1 == _UnityClientSection && data.Item2 == _UnityClientKey).Item3;
                var dataCollection = ini.DataList.Where(sec => sec.Item1 == _Section);

                foreach(var data in dataCollection)
                {
                    var tempDir = _TempArchiveDirectory + Path.GetFileName(data.Item2);

                    // 生成先の親ディレクトリが無ければ作る
                    DirectoryInfo directory = new DirectoryInfo(tempDir);
                    if (directory.Parent != null && !directory.Parent.Exists)
                    {
                        Directory.CreateDirectory(directory.Parent.FullName);
                    }

                    ZipModel.Compression(data.Item2, tempDir);
                }

                // GDrive上にArchiveが挙がっているのか確認用に表示する.
                Console.WriteLine($"====================================================================");
                Console.WriteLine($"GoogleDrive Uploaded Files.");
                Console.WriteLine($"====================================================================");
                model.ShowUploadedList();
                Console.WriteLine($"====================================================================");

                var archivedFiles = Directory.GetFiles(_TempArchiveDirectory).Select(path => Path.GetFileName(path));
                foreach (var arcFileName in archivedFiles)
                {
                    Console.WriteLine($"arcedName:{arcFileName}");
                    if (model.IsExist(arcFileName))
                    {
                        Console.WriteLine($"\"{arcFileName}\" is already exist. skip this file.");
                        continue;
                    }
                    model.Upload($"{_TempArchiveDirectory}{arcFileName}", "");
                }
                Console.Read();

                // zip化したアーカイブファイル先
                var arcFileNames = Directory.GetFiles(_TempArchiveDirectory);

#if DEBUG
                foreach(var name in arcFileNames)
                {
                    Console.Write($"zip:{name} => {Path.GetFullPath(name)}");
                }
#endif
                return;
            }
        }
    }
}
