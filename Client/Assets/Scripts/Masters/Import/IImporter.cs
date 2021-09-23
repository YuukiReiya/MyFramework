using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IImporter
{
    string Extension { get; }
    bool Import(string path);
}
