using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ScrollView - Cellの指定はしたくない（充てるプレハブ変えても動作する程度）
 * Cell - 専用で用意してもいい（）
 */
namespace uGUI
{
    public interface IScrollElement<T>
    {
        void Setup(T data);
    }
}