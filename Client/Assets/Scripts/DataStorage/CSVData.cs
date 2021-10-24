using System.Collections;
using System.Collections.Generic;
using Columns = System.Collections.Generic.List<object>;

namespace MasterData
{
    public class CSVData
    {
        public Dictionary<string/* �J������ */, Columns> Dict;

        #region constructor
        public CSVData() { Dict = new Dictionary<string, Columns>(); }
        #endregion

        /// �C���|�[�g�����t���O.
        public bool IsComplete { get; set; } = false;
    }
}