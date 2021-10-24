using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Expansion;
using IO;
using System.Threading;
using System.Threading.Tasks;
using MasterData;
using Columns = System.Collections.Generic.List<object>;

namespace IO
{
    public class CSVImporter : Importer, IDisposable
    {
        private const char Delimiter = ',';
        private const string Escape = "#";
        private uint row = 0;
        private List<string> columnNames = new List<string>();
        private CSVData csv;

        void IDisposable.Dispose()
        {
            csv = null;
        }

        public async Task<CSVData> Execute(string path, SynchronizationContext context, Action callback = null)
        {
            csv = new CSVData();
            columnNames.Clear();
            row = 0;
            csv.IsComplete = false;
            await ImportAsync(path, ReadLineMethod, context, () =>
            {
                Debug.Log("完了 > " + path);
                csv.IsComplete = true;
                callback?.Invoke();
            });
            return csv;
        }

        private void ReadLineMethod(string line)
        {
            if (line.StartsWith(Escape)) return;

            if (row == 0)
            {
                ImportHeader(line);
            }
            else
            {
                ImportColumns(line);
            }

            row++;
        }

        private void ImportHeader(string line)
        {
            var headers=line.Split(Delimiter);
            foreach(var header in headers)
            {
                if (!columnNames.Contains(header))
                {
                    columnNames.Add(header);
                }
#if UNITY_EDITOR
                else
                {
                    // 同じカラム名の重複登録を警告.
                    Debug.LogWarning($"CSVImportWarning:Duplicate column registration. > {header}");
                }
#endif
            }
        }

        private void ImportColumns(string line)
        {
            var columns = line.Split(Delimiter);

            for(var i = 0; i < columnNames.Count; ++i)
            {
#if DEVELOP_BY_EDITOR
                if (columns.Length <= i)
                {
                    // データがおかしい(先頭のカラム数 < 行で読み込んでいるカラム数).
                    Debug.LogWarning($"CSVImportWarning:A lot of data to read.");
                    continue;
                }
#endif
                var column = columnNames[i];
                var current = (object)(string.IsNullOrEmpty(columns[i]) ? string.Empty : columns[i]);
                if (!csv.Dict.ContainsKey(column))
                {
                    csv.Dict.Add(column, new List<object>(new[] { current }));
                }
                else
                {
                    csv.Dict[column].Add(current);
                }
            }
        }
    }
}