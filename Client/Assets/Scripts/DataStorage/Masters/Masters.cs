using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using MasterData;
using IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MasterData
{
    public class Masters : SingletonBehaviour<Masters>
    {
        #region 変数
        public static SynchronizationContext Context { get; private set; } = null;

        public Dictionary<CSVIndex, CSVData> CSV { get; private set; }
        private CSVImporter csvImporter = null;
        private Task importTask = null;

        public Dictionary<uint, TableBase> TableDic { get; private set; } = new Dictionary<uint, TableBase>();
        #endregion

        #region 列挙体
        public enum CSVIndex : int
        {
            Sample = 0,         // 読み込みサンプル
            Sample2,             // 読み込みサンプル
            HeavySample,     // 読み込みサンプル (重い)
        }
        #endregion

        #region readonly
        private static readonly Dictionary<CSVIndex, string/* ファイルパス */> CSVDic = new Dictionary<CSVIndex, string>
        {
            {CSVIndex.Sample,"C:\\Users\\yuuki\\Documents\\develop\\Template-OnlineProject\\Masters\\sample.csv" },
            {CSVIndex.Sample2,"C:\\Users\\yuuki\\Documents\\develop\\Template-OnlineProject\\Masters\\sample2.csv" },
            {CSVIndex.HeavySample,"C:\\Users\\yuuki\\Documents\\develop\\Template-OnlineProject\\Masters\\sample_heavy.csv" },
        };

        private const string DocumentPath = @"Assets/ExternalResources/Masters.xml";
        private const string RootTag = @"files";
        private const string ElementTag = @"element";
        private const string IndexTag = @"index";
        private const string PathTag = @"file";
        #endregion

        #region inner class
        #endregion

        #region property
        public bool IsComplete
        {
            get
            {
                return TableDic.All(data => data.Value.IsComplete);
            }
        }
        #endregion

        protected override void Awake()
        {
        }

        [ContextMenu("sample import")]
        private void Test()
        {
            Register();
            Import();
        }

        /// 読み込むテーブルをメモリに確保.
        public void Register()
        {
            TableDic.Clear();
            var doc = XDocument.Load(DocumentPath);
            var root = doc.Elements(RootTag);
            var elements = root.Elements(ElementTag);

            foreach(var element in elements)
            {
                var indexData = element.Element(IndexTag);
                var path = $"Assets/ExternalResources/{element.Element(PathTag).Value}";
                if (!File.Exists(path))
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"ImportWarning:Registration failed. > {path}");
#endif
                    continue;
                }

                uint index = 0;
                if(!uint.TryParse(indexData.Value,out index))
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"ImportWarning:Index parse failed. > {index}");
#endif
                    continue;
                }

#if UNITY_EDITOR
                if (TableDic.Keys.Contains(index))
                {
                    Debug.LogWarning($"ImportWarning:Duplicate key registration. > {index}");
                    continue;
                }
#endif
                var table = new TableBase(path);
                TableDic.Add(index, table);
            }
        }

        public void Unregister()
        {
            TableDic.Clear();
        }

        public void Import()
        {
            SetupImporters();
            importTask = Task.Run(ImportTask);
        }
        private async Task ImportTask()
        {
            foreach (var table in TableDic)
            {
                var current = table.Value;
                await ImportToTableByExtension(current.PathWithExtension, current);
            }
            RemoveImporters();
        }

        private void SetupImporters()
        {
            if (csvImporter == null)
            {
                csvImporter = new CSVImporter();
            }
        }

        private void RemoveImporters()
        {
            if (csvImporter != null)
            {
                csvImporter = null;
            }
        }

        private async Task ImportToTableByExtension(string pathWithExtension, TableBase table)
        {
            var ext = Path.GetExtension(pathWithExtension);
            switch (ext)
            {
                case CSVTable.Extension:
                    {
                        //await いらない？
                        await ImportByCSV(pathWithExtension, table);
                    }
                    break;

                default:
                    Debug.LogWarning($"MasterImportWarning:Invalid extension. > {pathWithExtension}");
                    break;
            }
        }

        private async Task ImportByCSV(string pathWithExtension, TableBase csv)
        {
            await csvImporter.Execute(csv, Context) ;
        }
    }
}