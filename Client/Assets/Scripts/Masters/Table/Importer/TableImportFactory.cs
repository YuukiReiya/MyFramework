using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;
using Masters;

namespace Masters
{
    public static class TableImportFactory
    {
        public static TableImporter CreateTableImporter(string pathWithExtension, CancellationTokenSource cancellationTokenSource)
        {
            var ext = Path.GetExtension(pathWithExtension);
            return ext switch
            {
                // .csv
                CSVTable.Extension => new CSVTableImporter(cancellationTokenSource),

                // default
                _ => throw new System.Exception($"ImportException: Invalid extension. > { pathWithExtension }"),
            };
        }
    }
}