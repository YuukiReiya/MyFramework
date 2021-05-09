﻿using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Editor.Expansion
{
    /*
     * Unity2020系から(?)新規に生成するScriptの文字コードがおかしくなるみたいなので無理やりUTF-8に保存しなおす。
     *  https://debuglog.tumblr.com/post/26550984743/%E3%82%B9%E3%82%AF%E3%83%AA%E3%83%97%E3%83%88%E3%83%95%E3%82%A1%E3%82%A4%E3%83%AB%E3%81%AEutf-8%E5%A4%89%E6%8F%9B
     * 記事によるとテンプレートファイル(C:\Program Files (x86)\Unity\Editor\Data\Resources\ScriptTemplates)をUTF-8(BOM)に変更してもUS-ASCIIに戻るらしい。
     */

    /// <summary>
    /// スクリプト作成時にエンコード形式をUTF-8に変換する
    /// </summary>
    public class ScriptEncoder : IAssetPostprocess
    {
        /// <summary>
        /// スクリプトの拡張子
        /// </summary>
        readonly static string[] Extensions = {
            ".cs",
        };


        /// <summary>
        /// BOM付きか判定.
        /// <para>参考:https://uxmilk.jp/48923</para>
        /// BOM付きなら先頭3バイトが順に" 0xEF 0xBB 0xBF "になってるらしい。
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private bool HaveBOM(byte[] bytes)
        {
            if (bytes.Length < 3) return false;
            return bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
        }

        void IAssetPostprocess.Execute(string path)
        {
            // Assets以下のフォルダ以外は対象外.
            if (!AssetPostprocessorImpl.IsValidRootDirectory(path)) return;

            // ファイル判定
            if (!File.Exists(path)) return;

            // 拡張子判定
            var ext = Path.GetExtension(path);
            if (!Extensions.Contains(ext)) return;

            // UTF-8に変換して保存しなおす
            ConvertEncodingUTF8(path);
        }

        /// <summary>
        /// エンコードをUTF-8形式に変換.
        /// </summary>
        /// <param name="binary"></param>
        private void ConvertEncodingUTF8(string assetPath)
        {
            var bin = GetBinary(assetPath);
            var enc = GetEncoding(bin);

            // エンコード形式特定できた.
            if (enc != null)
            {
                // BOM付きUTF-8なら変換は不要なのでリターンする
                if (enc != Encoding.UTF8 && HaveBOM(bin)) return;
            }
            // エンコード形式がわからなかった.
            else
            {
                // ダメ元でBOM判定できるか試す
                enc = GetEncodingByBOM(bin);
                // エンコード形式が特定できなかったので変換出来ない.
                if (enc == null)
                {
                    Debug.LogWarning($"Encoding conversion failed.\nAsset:{assetPath}\nEncoding is null.");
                    return;
                }
            }

            // 念のためOS対応
            // 改行コードCR+LF → LF に変換
            var contents = enc.GetString(bin).Replace("\r\n", "\n");
            //var contents = enc.GetString(bin);

            // UTF-8で上書き保存
            File.WriteAllText(assetPath, contents, Encoding.UTF8);
            Debug.Log($"<color=green>Script</color>:{assetPath} is converted to UTF-8.");
        }

        /// <summary>
        /// ファイルのバイナリデータを取得.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private byte[] GetBinary(string path)
        {
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var binary = new byte[stream.Length];
            stream.Read(binary, 0, binary.Length);
            stream.Close();
            return binary;
        }

        /// <summary>
        /// ファイルのエンコード方式を返す.
        /// <para>コピペ: https://dobon.net/vb/dotnet/string/detectcode.html </para>
        /// </summary>
        /// <returns></returns>
        private Encoding GetEncoding(byte[] bytes)
        {
            const byte bEscape = 0x1B;
            const byte bAt = 0x40;
            const byte bDollar = 0x24;
            const byte bAnd = 0x26;
            const byte bOpen = 0x28;    //'('
            const byte bB = 0x42;
            const byte bD = 0x44;
            const byte bJ = 0x4A;
            const byte bI = 0x49;

            int len = bytes.Length;
            byte b1, b2, b3, b4;

            //Encode::is_utf8 は無視

            bool isBinary = false;
            for (int i = 0; i < len; i++)
            {
                b1 = bytes[i];
                if (b1 <= 0x06 || b1 == 0x7F || b1 == 0xFF)
                {
                    //'binary'
                    isBinary = true;
                    if (b1 == 0x00 && i < len - 1 && bytes[i + 1] <= 0x7F)
                    {
                        //smells like raw unicode
                        return System.Text.Encoding.Unicode;
                    }
                }
            }
            if (isBinary)
            {
                return null;
            }

            //not Japanese
            bool notJapanese = true;
            for (int i = 0; i < len; i++)
            {
                b1 = bytes[i];
                if (b1 == bEscape || 0x80 <= b1)
                {
                    notJapanese = false;
                    break;
                }
            }
            if (notJapanese)
            {
                return System.Text.Encoding.ASCII;
            }

            for (int i = 0; i < len - 2; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                b3 = bytes[i + 2];

                if (b1 == bEscape)
                {
                    if (b2 == bDollar && b3 == bAt)
                    {
                        //JIS_0208 1978
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    }
                    else if (b2 == bDollar && b3 == bB)
                    {
                        //JIS_0208 1983
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    }
                    else if (b2 == bOpen && (b3 == bB || b3 == bJ))
                    {
                        //JIS_ASC
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    }
                    else if (b2 == bOpen && b3 == bI)
                    {
                        //JIS_KANA
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    }
                    if (i < len - 3)
                    {
                        b4 = bytes[i + 3];
                        if (b2 == bDollar && b3 == bOpen && b4 == bD)
                        {
                            //JIS_0212
                            //JIS
                            return System.Text.Encoding.GetEncoding(50220);
                        }
                        if (i < len - 5 &&
                            b2 == bAnd && b3 == bAt && b4 == bEscape &&
                            bytes[i + 4] == bDollar && bytes[i + 5] == bB)
                        {
                            //JIS_0208 1990
                            //JIS
                            return System.Text.Encoding.GetEncoding(50220);
                        }
                    }
                }
            }

            //should be euc|sjis|utf8
            //use of (?:) by Hiroki Ohzaki <ohzaki@iod.ricoh.co.jp>
            int sjis = 0;
            int euc = 0;
            int utf8 = 0;
            for (int i = 0; i < len - 1; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if (((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC)) &&
                    ((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC)))
                {
                    //SJIS_C
                    sjis += 2;
                    i++;
                }
            }
            for (int i = 0; i < len - 1; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if (((0xA1 <= b1 && b1 <= 0xFE) && (0xA1 <= b2 && b2 <= 0xFE)) ||
                    (b1 == 0x8E && (0xA1 <= b2 && b2 <= 0xDF)))
                {
                    //EUC_C
                    //EUC_KANA
                    euc += 2;
                    i++;
                }
                else if (i < len - 2)
                {
                    b3 = bytes[i + 2];
                    if (b1 == 0x8F && (0xA1 <= b2 && b2 <= 0xFE) &&
                        (0xA1 <= b3 && b3 <= 0xFE))
                    {
                        //EUC_0212
                        euc += 3;
                        i += 2;
                    }
                }
            }
            for (int i = 0; i < len - 1; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if ((0xC0 <= b1 && b1 <= 0xDF) && (0x80 <= b2 && b2 <= 0xBF))
                {
                    //UTF8
                    utf8 += 2;
                    i++;
                }
                else if (i < len - 2)
                {
                    b3 = bytes[i + 2];
                    if ((0xE0 <= b1 && b1 <= 0xEF) && (0x80 <= b2 && b2 <= 0xBF) &&
                        (0x80 <= b3 && b3 <= 0xBF))
                    {
                        //UTF8
                        utf8 += 3;
                        i += 2;
                    }
                }
            }
            //M. Takahashi's suggestion
            //utf8 += utf8 / 2;

            //Debug.Log(string.Format("sjis = {0}, euc = {1}, utf8 = {2}", sjis, euc, utf8));
            if (euc > sjis && euc > utf8)
            {
                //EUC
                return System.Text.Encoding.GetEncoding(51932);
            }
            else if (sjis > euc && sjis > utf8)
            {
                //SJIS
                return System.Text.Encoding.GetEncoding(932);
            }
            else if (utf8 > euc && utf8 > sjis)
            {
                //UTF8
                return System.Text.Encoding.UTF8;
            }

            return null;
        }

        /// <summary>
        /// BOMを調べて文字コードを判別.
        /// <para>コピペ: https://dobon.net/vb/dotnet/string/detectcode.html </para>
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>見つからなければnull.</returns>
        private Encoding GetEncodingByBOM(byte[] bytes)
        {
            if (bytes.Length < 2)
            {
                return null;
            }
            if ((bytes[0] == 0xfe) && (bytes[1] == 0xff))
            {
                //UTF-16 BE
                return new System.Text.UnicodeEncoding(true, true);
            }
            if ((bytes[0] == 0xff) && (bytes[1] == 0xfe))
            {
                if ((4 <= bytes.Length) &&
                    (bytes[2] == 0x00) && (bytes[3] == 0x00))
                {
                    //UTF-32 LE
                    return new System.Text.UTF32Encoding(false, true);
                }
                //UTF-16 LE
                return new System.Text.UnicodeEncoding(false, true);
            }
            if (bytes.Length < 3)
            {
                return null;
            }
            if ((bytes[0] == 0xef) && (bytes[1] == 0xbb) && (bytes[2] == 0xbf))
            {
                //UTF-8
                return new System.Text.UTF8Encoding(true, true);
            }
            if (bytes.Length < 4)
            {
                return null;
            }
            if ((bytes[0] == 0x00) && (bytes[1] == 0x00) &&
                (bytes[2] == 0xfe) && (bytes[3] == 0xff))
            {
                //UTF-32 BE
                return new System.Text.UTF32Encoding(true, true);
            }

            return null;
        }
    }
}