using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Expansion
{
    public static class RectTransformExpansion
    {
        /// <summary>
        /// 矩形の角の4点
        /// </summary>
        //private static Vector3[] m_corners = new Vector3[4];

        /// <summary>
        /// 接触判定.
        /// <br>矩形の一部分でも、もう片方の矩形に接触していたら<c>true</c>を返す</br>
        /// </summary>
        public static bool Contact(this RectTransform self, RectTransform rect)
        {
            var selfBounds = self.GetBounds();
            var targetBounds = rect.GetBounds();
            return selfBounds.Intersects(targetBounds);
        }

        /// <summary>
        /// 含有判定.
        /// <br>矩形の中に矩形が収まっていたら<c>true</c>を返す</br>
        /// </summary>
        /// <param name="self"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool Contains(this RectTransform self, RectTransform rect)
        {
            var selfBounds = self.GetBounds();
            var targetBounds = rect.GetBounds();
            return selfBounds.Contains(new Vector3(targetBounds.min.x, targetBounds.min.y, 0)) &&
                      selfBounds.Contains(new Vector3(targetBounds.min.x, targetBounds.max.y, 0)) &&
                      selfBounds.Contains(new Vector3(targetBounds.max.x, targetBounds.min.y, 0)) &&
                      selfBounds.Contains(new Vector3(targetBounds.max.x, targetBounds.max.y, 0));
        }

        /// <summary>
        /// 境界取得.
        /// <br>矩形の構成された領域を返す。</br>
        /// </summary>
        public static Bounds GetBounds(this RectTransform self)
        {
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            //TODO
            //GetWorldCornersが回転考慮しない頂点の取得だった場合、
            //必ず左下の頂点がmin、右上の頂点がmaxになるはずなのでインデックスで直取得のほうがいいかも。
            Vector3[] corners = new Vector3[4];
            self.GetWorldCorners(corners);
            min = Vector3.Min(corners.Min(), min);
            max = Vector3.Max(corners.Max(), max);
            max.z = 0f;
            min.z = 0f;

            //対角線を納めるようにバウンスを算出
            Bounds bounds = new Bounds(min, Vector3.zero);
            //(デフォなら最小:左下、最大:右上のはず)
            bounds.Encapsulate(max);
            return bounds;
        }

    }
}