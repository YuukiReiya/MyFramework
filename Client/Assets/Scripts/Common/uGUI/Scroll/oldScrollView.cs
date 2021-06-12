//MaskよりRectMask2Dの方が高速:http://tsubakit1.hateblo.jp/entry/2015/11/08/212202﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿
//2Dの描画領域(ViewBounds)の取得:https://hacchi-man.hatenablog.com/entry/2020/05/09/220000

//RectTransformUtility.CalculateRelativeRectTransformBounds を利用することで Bounds が取得できますが、
//これは子供の RectTransfrom も対象にしている

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#if false
namespace uGUI
{
    public abstract class ScrollView<Contents> : UIBehaviour
    {
        /// <summary>
        /// セルの間隔
        /// </summary>
        [SerializeField, Range(1e-2f, 1f)]
        float m_spacing = 1e-2f;

        /// <summary>
        /// スクロール位置の基準
        /// </summary>
        /// <remarks>
        /// <c>0.5</c> を指定してスクロール位置が <c>0</c> の場合, 中央に最初のセルが配置される
        /// </remarks>
        [SerializeField, Range(0f, 1f)]
        float m_offset = 0;

        /// <summary>
        /// 無限ループ
        /// </summary>
        [SerializeField]
        protected bool m_loop = false;

        /// <summary>
        /// セルのコンテナ(親オブジェクト)
        /// </summary>
        [SerializeField]
        protected RectTransform m_container;

        /// <summary>
        /// スクロールバー
        /// </summary>
        [SerializeField]
        protected ScrollBar m_scrollBar;

        [SerializeField]
        protected RectTransform m_viewPort;

        [SerializeField]
        protected uint m_elementCount = 1;

        /// <summary>
        /// スクロール位置
        /// </summary>
        private float m_position;

        /// <summary>
        /// プレハブ
        /// </summary>
        protected abstract GameObject Prefab { get; }

        /// <summary>
        /// プールコレクション
        /// </summary>
        protected abstract IEnumerator Pool { get; set; }

        uint RepeatIndex(uint index, uint size)
        {
            if (size == 1)
            {
                return 0;
            }
            return index % size;
        }

        private void Reposition(float position)
        {
            m_position = position;
        }

        /// <summary>
        /// スクロールバーの描画領域に合わせセルを更新
        /// </summary>
        public bool Contains(RectTransform rect)
        {
            //

            return true;
        }

        private void Start()
        {
            m_scrollBar.Scrollbar.size = 1f / m_elementCount;
            m_scrollBar.Scrollbar.value = 0f;

#if UNITY_EDITOR
            Debug.Assert(m_scrollBar == null, "m_scrollBar is null.", m_scrollBar);
#endif
        }

        private void Update()
        {
            
        }

        private void Update(float firstPosition, uint firstIndex, bool forceUpdate)
        {
            //uint i = 0;
            //while (Pool.MoveNext())
            //{
            //    var index = firstIndex + i;
            //    var position = firstPosition + i * m_spacing;

            //    if (m_loop)
            //    {
            //        index = 0;
            //    }
            //}

            //スクロールしてビューの外に出たやつを非活性化
            

        }
    }

    public abstract class ScrollElement<Content> : MonoBehaviour
    {
        public uint Index { get; set; } = 0;
    }
}
#endif