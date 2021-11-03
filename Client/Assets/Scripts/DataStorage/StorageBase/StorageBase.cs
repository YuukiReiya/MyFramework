using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Columns = System.Collections.Generic.List<object>;

namespace MasterData
{
    public abstract class StorageBase : IDisposable
    {
        public string Path { get; protected set; } = string.Empty;
        public abstract string Extension { get; }
        public Dictionary<string, Columns> Storage { get; protected set; } = null;
        public bool IsComplete { get; set; } = false;

        public StorageBase(string path)
        {
            Path = path;
            Storage = new Dictionary<string, Columns>();
            IsComplete = false;
        }

        void IDisposable.Dispose() 
        {
            Path = null;
            Storage.Clear();
            Storage = null;
        }
    }
}