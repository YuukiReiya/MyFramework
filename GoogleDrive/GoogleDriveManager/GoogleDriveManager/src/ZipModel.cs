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

        public const string Extension = ".zip";

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
                Console.WriteLine($"compression success.{Environment.NewLine}source: > {Path.GetFullPath(srcDirPath)}{Environment.NewLine}destination: > {Path.GetFullPath(destArcFileName)}");
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
        public static void Uncompression(string srcArcFilePath, string destPath, bool isOverwrite = true)
        {
            if (!File.Exists(srcArcFilePath))
            {
                Console.WriteLine($"file is not exist! > {srcArcFilePath} ");
                return;
            }

            var ext = Path.GetExtension(srcArcFilePath);
            if (ext != Extension)
            {
                // 拡張子が.zipじゃない.
                Console.WriteLine($"not match zip extension. > {ext}");
            }

            // フォルダがあれば削除しておく.
            if (isOverwrite)
            {
                if (Directory.Exists(destPath))
                {
                    Directory.Delete(destPath, true);
                }
            }

            try
            {
                ZipFile.ExtractToDirectory(srcArcFilePath, destPath);
                Console.WriteLine($"uncompression success.{Environment.NewLine}source: > {Path.GetFullPath(srcArcFilePath)}{Environment.NewLine}destination: > {Path.GetFullPath(destPath)}");
            }
            catch(IOException e) when ((e.HResult & _ExceptionMask) == _ErrorFileExists || (e.HResult & _ExceptionMask) == _ErrorAlreadyExists)
            {
                Console.WriteLine($"uncompression stop. {Environment.NewLine}\"{destPath}\" is already exists.");

                // 結合(差分のみ).
                //var lastIndex = srcArcFilePath.LastIndexOf(Extension);
                //if (lastIndex != -1)
                //{
                //    var tempPath = srcArcFilePath.Substring(0, lastIndex);
                //    ZipFile.ExtractToDirectory(srcArcFilePath, tempPath);
                //    var eEntries = Directory.GetFileSystemEntries(tempPath).GetEnumerator();
                //    while (eEntries.MoveNext())
                //    {
                //        /*
                //            使い道なさそうなので一旦開発中止.
                //         */
                //    }
                //}

                return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"uncompression failed.\n {e.GetType().Name}:{e.Message}\n{e.StackTrace}\n");
                throw e;
            }
        }

        public static void Uncompression(string srcArcFilePath, string destPath, Encoding encoding) => ZipFile.ExtractToDirectory(srcArcFilePath, destPath, encoding);
    }
}
