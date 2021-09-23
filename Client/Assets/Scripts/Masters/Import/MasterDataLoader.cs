/*
    �񓯊��ǂݍ��� 
 */
using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System.Xml.Linq;
namespace Masters
{
    public class MasterDataLoader : BehaviourBase
    {
        // Task���g���ĕʃX���b�h�ŗ������߂̋^�����b�N�p.
        // ����L�̂��߃X���b�h���͌Œ�.
        SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private const string DocumentPath = @"Assets/ExternalResources/Masters.xml";
        private const string TagName = @"files";
        private Masters masters { get { return Masters.Instance; } }

        public bool IsLoading { get; } = false;

        protected override void Awake()
        {
#if UNITY_EDITOR
            Execute(DocumentPath);
#endif
        }

        private void PreImport()
        {

        }

        private void Import()
        {

        }

        private void PostImport()
        {

        }

        [ContextMenu("�e�X�g")]
        public void Execute(string path)
        {
            var doc = XDocument.Load(path);
            var elements = doc.Elements(TagName);
            foreach (var element in elements) 
            {
                path = element.Value;
                if (!File.Exists(path)) 
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"<color=yellow>{path}</color> is not exist.");
#endif
                    continue;
                }

                Import(path);
            }
        }

        private void Import(string path)
        {
            semaphore.WaitAsync();
            var ext = Path.GetExtension(path);
            switch (ext)
            {
                case ".csv":
                    ImportCSV(path);
                    break;

                case ".xlsx":
                case ".xlsm":
                    break;

                case ".xml":
                    break;

                case ".yaml":
                case ".yml":
                    break;

                case ".txt":
                default:
                    break;
            }
        }

        private async Task ImportCSV(string path)
        {
        }

        private void ImportXML(string path)
        {

        }

        private void ImportYAML(string path)
        {

        }

        private void ImportXLSX(string path)
        {

        }

        private void ImportTXT(string path)
        {

        }

        #region async

        public void ExecuteAsync(string path)
        {
            masters.Dict.Clear();

            var doc = XDocument.Load(path);
            var elements = doc.Elements(TagName);
            foreach (var element in elements)
            {
                path = element.Value;
                if (!File.Exists(path))
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"<color=yellow>{path}</color> is not exist.");
#endif
                    continue;
                }

                Task.Run(() => ImportAsync(path));
            }

        }

        /// �}�X�^�[
        private async Task ImportAsync(string path)
        {
            await semaphore.WaitAsync();
            var ext = Path.GetExtension(path);
            switch (ext)
            {
                case ".csv":
                    await ImportAsyncCSV(path);
                    break;

                case ".xlsx":
                case ".xlsm":
                    break;

                case ".xml":
                    break;

                case ".yaml":
                case ".yml":
                    break;

                case ".txt":
                default:
                    break;
            }
            semaphore.Release();
        }

        private async Task ImportAsyncCSV(string path)
        {
            var lines = await File.ReadAllLinesAsync(path);

            if (lines.Length < 1) return;

            var masterData = new MasterData();

            var e = lines.GetEnumerator();
            uint row = 0;
            uint columnNum = 0;
            var keys = new List<string>();
            var values = new Dictionary<int, List<object>>();

            while (e.MoveNext())
            {
                var line = lines[row++];

                // �J�����ǂݍ���.
                if (row == 0)
                {
                    var columns = line.Split(",");
                    foreach (var column in columns)
                    {
                        if (string.IsNullOrEmpty(column)) continue;
                        keys.Add(column);
                        columnNum++;
                    }
                }
                // �l�ǂݍ���.
                else
                {
                    var data = line.Split(",");
                    for (int i = 0; i < columnNum; ++i)
                    {
                        if (values[i] == null)
                        {
                            values[i] = new List<object>();
                        }
                        values[i].Add(data[i]);
                        masterData.Data[i][keys[i]].Append(data[i]);
                    }
                }
                row++;
            }

            // IEnumerable�^�ɋl�ߑւ�.
            var eDict = values.GetEnumerator();
            while (eDict.MoveNext())
            {
                var current = eDict.Current;
                var s = current.Value;
            }
        }

        #endregion
    }
}