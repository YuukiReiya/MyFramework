using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Columns = System.Collections.Generic.List<object>;

namespace MasterData
{
    public class Schedule
    {
        public DateTime Begin;
        public DateTime End;

        public bool IsInvalid 
        {
            get 
            {
                return Begin == default(DateTime) || End == default(DateTime);
            }
        }
    }
    public class TableBase : IDisposable
    {
        public string PathWithExtension { get; protected set; } = string.Empty;
        public Dictionary<string, Columns> Table { get; protected set; } = null;
        public bool IsComplete { get; set; } = false;
        public Schedule Schedule { get; protected set; } = null;

        public TableBase(string path)
        {
            PathWithExtension = path;
            Table = new Dictionary<string, Columns>();
            IsComplete = false;
        }

        void IDisposable.Dispose()
        {
            PathWithExtension = null;
            Table.Clear();
            Table = null;
        }
    }
}
