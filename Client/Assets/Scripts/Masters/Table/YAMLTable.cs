using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Masters
{
    public class YAMLTable : TableBase
    {
        public const string Extension = ".yaml";
        public const string Extension2 = ".yml";

        public YAMLTable(string path) : base(path) { }
    }
}