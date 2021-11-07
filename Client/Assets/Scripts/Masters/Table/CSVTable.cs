using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masters
{
    public class CSVTable:TableBase
    {
        public const string Extension = ".csv";

        public CSVTable(string path) : base(path) { }
    }
}
