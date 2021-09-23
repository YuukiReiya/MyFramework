using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using 
public class CSVImporter : IImporter
{
    string IImporter.Extension { get { return $".csv"; } }
    bool IImporter.Import(string s)
    {
        return false;
    }
}
