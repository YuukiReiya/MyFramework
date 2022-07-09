using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GDriveManager.src
{
    /// <summary>
    /// .iniの読み込み用
    /// </summary>
    class IniReader
    {
        public IniReader(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            EncodingType = Encoding.GetEncoding("Shift_JIS");
            Read(path);
            
            //作ったリスト セクション:キー:値 を表示する
            //Log();
        }
        /// <summary>
        /// iniのエンコードタイプ
        /// memo.windowsはデフォルトでSHIFT_JIS
        /// </summary>
        private readonly Encoding EncodingType = null;

        public Stack<string> SectionStack { get; private set; } = new Stack<string>();
        public List<(string/* セクション */, string /* キー */, string/* 値 */)> DataList { get; private set; } = new List<(string, string, string)>();
        public List<string> Keys
        {
            get
            {
                return DataList.Select(elem => elem.Item2).ToList();
            }
        }
        public List<string> Values
        {
            get
            {
                return DataList.Select(elem => elem.Item3).ToList();
            }
        }

        private void Read(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"LOAD_ERROR{Environment.NewLine}path:{path} is not found!");
                return;
            }
            using (var sr = new StreamReader(path, EncodingType))
            {
                var contents = sr.ReadToEnd().Split($"{Environment.NewLine}");
                foreach (var element in contents)
                {
                    var text = element;
                    var dem = text.IndexOf(";");
                    //「;」以降はコメントアウトなので不要
                    if (0 <= dem)
                    {
                        text = text.Substring(0, dem);
                    }

                    // 全部コメントアウトされてしまっていたら次。
                    if (string.IsNullOrEmpty(text)) continue;

                    // セクション判定
                    if (text.StartsWith("[") && text.EndsWith("]"))
                    {
                        SectionStack.Push(text.Substring(1, text.Length - 2));
                    }
                    // メンバ
                    else
                    {
                        //　登録セクション
                        var section = SectionStack.Any() ? SectionStack.Peek() : string.Empty;

                        // 「キー ＝ データ」 の構成か
                        dem = text.IndexOf("=");

                        // データがある
                        if (0 <= dem)
                        {
                            var key = text.Substring(0, dem);
                            var value = text.Substring(dem + 1);
                            DataList.Add((section, key, value));
                        }
                        // キーのみのパターン
                        else
                        {
                            var key = text;
                            DataList.Add((section, key, string.Empty));
                        }
                    }
                }
            }
        }

        private void Log()
        {
            foreach (var data in DataList)
            {
                Console.WriteLine($"セクション:{data.Item1} キー:{data.Item2} 値:{data.Item3}");
            }
        }
    }
}
