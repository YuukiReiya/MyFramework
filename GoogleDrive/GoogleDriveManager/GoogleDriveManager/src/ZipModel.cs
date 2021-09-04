using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace Model
{
    public class ZipModel
    {
        const int _ExceptionMask = 0x0000FFFF;
        const int _ErrorFileExists = 80;
        const int _ErrorAlreadyExists = 183;

        /// <summary>
        /// 圧縮
        /// </summary>
        /// <param name="srcDirPath">アーカイブするファイル</param>
        /// <param name="destArcFileName">生成先</param>
        /// <param name="isSrcFileDelete">アーカイブ元のファイルを削除するか</param>
        public static void Compression(string srcDirPath, string destArcFileName)
        {
            if (!Directory.Exists(srcDirPath))
            {
                Console.WriteLine($"directory is not exist! > {srcDirPath} {Environment.NewLine}=> FullPath:{Path.GetFullPath(srcDirPath)}");
                return;
            }
            try
            {
                // 拡張子がついてなければ付ける
                ZipFile.CreateFromDirectory(srcDirPath, Path.GetExtension(destArcFileName) != string.Empty ? destArcFileName : destArcFileName + ".zip");
                Console.WriteLine($"compression success.");
            }
            catch (IOException e) when ((e.HResult & _ExceptionMask) == _ErrorFileExists || (e.HResult & _ExceptionMask) == _ErrorAlreadyExists)
            {
                Console.WriteLine($"compression stop. {Environment.NewLine}\"{Path.GetFullPath(srcDirPath)}\" is already exists.");
                return;
            }
            catch (DirectoryNotFoundException)
            {
                // Archiveの保存先のディレクトリが無いかも.
                var destArcPath = Path.GetExtension(destArcFileName) != string.Empty ? destArcFileName : destArcFileName + ".zip";
                Console.WriteLine($"compression stop. {Environment.NewLine}\"{destArcPath}\" is not found destination directory. {Environment.NewLine}{destArcFileName}=>FullPath:{Path.GetFullPath(destArcPath)}");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"compression failed.\n {e.GetType().Name}:{e.Message}\n{e.StackTrace}\n");
                throw e;
            }
        }

        public static void Compression(string srcDirPath, string destArcFileName, CompressionLevel compressionLevel, bool includeBaseDirectory) => ZipFile.CreateFromDirectory(srcDirPath, destArcFileName, compressionLevel, includeBaseDirectory);

        /// 解凍
        public static void Unzip()
        {

        }
    }
}
