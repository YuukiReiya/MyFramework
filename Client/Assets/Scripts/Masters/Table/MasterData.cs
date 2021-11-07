using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Masters;
using IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Expansion;

namespace Masters
{
    public class MasterData : SingletonBehaviour<MasterData>
    {
        #region 変数
        public static SynchronizationContext Context { get; private set; } = null;

        private Task importTask = null;
        private Dictionary<string, TableImporter> tableImporterCacheDic = new Dictionary<string, TableImporter>();
        public Dictionary<uint, TableBase> TableDic { get; private set; } = new Dictionary<uint, TableBase>();
        #endregion

        #region 列挙体
        #endregion

        #region readonly
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
            Register();
            Import();
        }

#if UNITY_EDITOR
        [ContextMenu("Import Test")]
        private void TestImport()
        {
            Register();
            Import();
        }
#endif

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
                TableImporter importer = null;

                var extension = Path.GetExtension(current.PathWithExtension);
                if (!tableImporterCacheDic.TryGetValue(extension, out importer))
                {
                    importer = TableImportFactory.CreateTableImporter(current.PathWithExtension);
                    tableImporterCacheDic.Add(extension, importer);
                }
                Debug.Log($"{current.PathWithExtension} is import start.");
                await importer.Execute(current, Context);
                Debug.Log($"{current.PathWithExtension} is import complete.");
            }
            Debug.Log($"Completed import of registered table.");
            RemoveImporters();
        }

        private void SetupImporters()
        {
            tableImporterCacheDic = new Dictionary<string, TableImporter>();
        }

        private void RemoveImporters()
        {
            tableImporterCacheDic.Clear();
            tableImporterCacheDic = null;
        }
    }
}