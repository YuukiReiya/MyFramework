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
                // 
                //ini.DataList.Where()
                var dataCollection = ini.DataList.Where(sec => sec.Item1 == _Section);

                if (model.IsExist(""))
                {

                }
                Console.Read();

                return;
                model.Upload(new[] { "test" });

                Console.WriteLine("Complete upload!.");
                Console.Read();
            }
        }
    }
}
