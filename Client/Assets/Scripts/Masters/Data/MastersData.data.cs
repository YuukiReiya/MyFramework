using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


namespace Masters
{
    using Column = Dictionary<string, IEnumerable<object>>;
    public class MasterData : BehaviourBase
    {
        public List<Column> Data;
    }
}