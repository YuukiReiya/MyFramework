using System.Collections;
using System.Collections.Generic;
using Columns = System.Collections.Generic.List<object>;

namespace MasterData
{
    public class CSVData
    {
        public Dictionary<string/* カラム名 */, Columns> Dict;

        #region constructor
        public CSVData() { Dict = new Dictionary<string, Columns>(); }
        #endregion

        /// インポート完了フラグ.
        public bool IsComplete { get; set; } = false;
    }
}