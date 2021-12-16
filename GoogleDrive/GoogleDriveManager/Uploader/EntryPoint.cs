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
        const string _TokenFolderPath = @"./res/token.json";
        const string _IniFilePath = @"./res/system.ini";
        const string _TempArchiveDirectory = @"./temp/";
        const string _UnityClientSection = "COMMON";
        const string _UnityClientKey = "UNITY_PROJECT_PATH";
#if false
        // Upload方法は3種類あるけど、ぶっちゃけResumableだけでいい。
        const string _SectionHeader = "UPLOAD";
        readonly string[] _SectionTails = new string[] { "SIMPLE", "", "" };
#else
        const string _UploadSection = "UPLOAD";
        const string _CompressionSection = "COMPRESSION";
#endif
        static void Main(string[] resourcesDirectory)
        {
            var model = GoogleServiceModel.Instance;

            if(resourcesDirectory.Length < 1)
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

            var result = model.Setup(Path.GetFullPath(credentialPath), Path.GetFullPath(tokenFolderPath));
            if (result != GoogleServiceModel.Result.Success)
            {
                Console.WriteLine("Setup Failed.");
                return;
            }

            // 表示数.
            GoogleServiceModel.PageSize = 15;

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
            var iniPath = resDir + _IniFilePath;
            if (!File.Exists(iniPath))
            {
                Console.WriteLine($"ini file is not exist. > {iniPath}");
                Console.ReadKey();
                return;
            }
            // 指定したiniから読み込む.
            var ini = new IniReader(Path.GetFullPath(iniPath),
#if DEBUG
                    true
#else
                    false
#endif
                    );
            var clientAssetsPath = ini.DataList.Find(data => data.Item1 == _UnityClientSection && data.Item2 == _UnityClientKey).Item3;
            var e = ini.DataList.Where(sec => sec.Item1 == _CompressionSection).GetEnumerator();

            // 圧縮.
            while (e.MoveNext())
            {
                var current = e.Current;

                // 書き出し先
                var destArcPath = _TempArchiveDirectory + Path.GetFileName(current.Item3);

                // ディレクトリを用意しとく.
                var info = new DirectoryInfo(destArcPath);
                if (info.Parent != null && !info.Parent.Exists)
                {
                    Directory.CreateDirectory(info.Parent.FullName);
                }

                // 該当ファイルを圧縮し書き出す.
                ZipModel.Compression(current.Item3, destArcPath);
                var cloudPath = string.Empty;
                var target = ini.DataList.FirstOrDefault(data => data.Item1 == _UploadSection && data.Item2 == current.Item2);
                if(target != default)
                {
                    cloudPath = target.Item3;
                }

                // 既にフォルダがあれば削除してから挙げなおす.
                if (model.IsExist(cloudPath))
                {
                    model.Delete(cloudPath);
                }
                model.Upload(destArcPath + ZipModel.Extension, cloudPath);
            }
            Console.ReadKey();
        }
    }
}
