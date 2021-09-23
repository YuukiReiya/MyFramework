using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Masters
{
    public partial class Masters : SingletonBase<Masters>
    {
        public const string Delimiter = @".";

        public Dictionary<string, MasterData> Dict { get; } = new Dictionary<string, MasterData>();
    }
}