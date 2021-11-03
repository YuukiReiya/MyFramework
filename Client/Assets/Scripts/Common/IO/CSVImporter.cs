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
        private TableBase csv;

        void IDisposable.Dispose()
        {
            csv = null;
        }

        public async Task<CSVTable> Execute(string path, SynchronizationContext context, Action callback = null)
        {
            var table = new CSVTable(path);
            columnNames.Clear();
            row = 0;
            table.IsComplete = false;
            await ImportAsync(path, ReadLineMethod, context, callback);
            return table;
        }

        public async Task Execute(TableBase table, SynchronizationContext context, Action callback = null)
        {
            csv = table;
            columnNames.Clear();
            row = 0;
            table.IsComplete = false;
            await ImportToTableAsync(table, ReadLineMethod, context, callback);
            table.IsComplete = true;
            return;
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
                if (!csv.Table.ContainsKey(column))
                {
                    csv.Table.Add(column, new List<object>(new[] { current }));
                }
                else
                {
                    csv.Table[column].Add(current);
                }
            }
        }
    }
}