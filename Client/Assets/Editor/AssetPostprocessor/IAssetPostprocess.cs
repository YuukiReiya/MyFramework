﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.Expansion
{
    /// <summary>
    /// アセットのPost処理.
    /// <br>下記4パターン対応のための専用インターフェース.</br>
    /// <br>(アセット個別に行う処理を定義)</br>
    /// <br>* インポートアセット</br>
    /// <br>* デリートアセット</br>
    /// <br>* 移動先アセット</br>
    /// <br>* 移動元アセット</br>
    /// </summary>
    public interface IAssetPostprocess
    {
        void Execute(string path);
    }

    /// <summary>
    /// アセットのPost処理.
    /// </summary>
    public interface IAssetAllPostprocess
    {
        void Execute();
    }
}