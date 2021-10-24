using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using MasterData;
using IO;
using System.Threading;
using System.Threading.Tasks;

namespace DataStorage
{
    public class Masters : SingletonBehaviour<Masters>
    {
        #region 変数
        public static SynchronizationContext Context { get; private set; } = null;

        public Dictionary<CSVIndex, CSVData> CSV;
        private Task importTask = null;
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
        #endregion

        #region inner class
        #endregion

        #region property
        public bool IsComplete
        {
            get
            {
                return CSV.All(data => data.Value != null && data.Value.IsComplete);
            }
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            Context = SynchronizationContext.Current;
            importTask = Task.Run(ImportCSV);
        }

        private async Task ImportCSV()
        {
            CSV = new Dictionary<CSVIndex, CSVData>();
            var csvImporter = new CSVImporter();

            foreach (CSVIndex index in Enum.GetValues(typeof(CSVIndex)))
            {
                var path = CSVDic[index];
                CSV.Add(index, await csvImporter.Execute(path, Context));
            }
        }
    }
}