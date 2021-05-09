﻿//Githubに挙げる際に後々必要になりそうな名前のフォルダを空で用意しておきたい。
//参考:https://qiita.com/tsujihaneta/items/0096ed9f8f0621d1ccbd
//※多分コピペしただけだと問題ありそうなのでリファクタリングは必須。
//　そのうえで必要があれば改良する。
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;


namespace Editor.Expansion
{
    public class FolderKeep : IAssetAllPostprocess
    {
        /// <summary>
        /// 空フォルダのために作るファイル
        /// NOTE:
        /// 「.gitkeep」という名前のダミーファイルが一般的らしい。
        /// </summary>
        private const string DummyFileName = ".gitkeep";
        private const string RootDirectoryName = "Assets";

        void IAssetAllPostprocess.Execute()
        {
            Create(RootDirectoryName);
        }

        /// <summary>
        /// memo.
        /// https://qiita.com/tsujihaneta/items/0096ed9f8f0621d1ccbd
        /// コピペなので必要に応じてリファクタリング。
        /// </summary>
        /// <param name="directoryPath"></param>
        private void Create(string directoryPath)
        {
            //ディレクトリパスの配列
            string[] directories = Directory.GetDirectories(directoryPath);
            //ファイルパスの配列
            string[] files = Directory.GetFiles(directoryPath);
            //キーパーの配列
            string[] keepers = Directory.GetFiles(directoryPath, DummyFileName);

            //ディレクトリがあるかどうか
            bool isDirectoryExist = 0 < directories.Length;
            //(キーパー以外の)ファイルがあるかどうか
            bool isFileExist = 0 < (files.Length - keepers.Length);
            //キーパーがあるかどうか
            bool isKeeperExist = 0 < keepers.Length;

            //ディレクトリもファイルもなかったら
            if (!isDirectoryExist && !isFileExist)
            {
                //キーパーがなかったら
                if (!isKeeperExist)
                {
                    //キーパーを作成
                    File.Create(Path.Combine(directoryPath, DummyFileName)).Close();
                    UnityEngine.Debug.Log("Create dummy file for empty folder. : " + directoryPath);
                }
                return;
            }
            //ディレクトリかファイルがあったら
            else
            {
                //キーパーがあったら
                if (isKeeperExist)
                {
                    //キーパーを削除
                    File.Delete(Path.Combine(directoryPath, DummyFileName));
                    UnityEngine.Debug.Log("Delete dummy file for empty folder. : " + directoryPath);
                }
            }

            //再帰
            foreach (var directory in directories)
            {
                Create(directory);
            }
        }
    }
}