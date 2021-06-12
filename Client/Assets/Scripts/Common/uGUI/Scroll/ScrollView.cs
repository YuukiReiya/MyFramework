using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Common;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace uGUI
{
    #region 列挙体
    public enum ScrollDirection
    {
        LeftToRight = 1,
        RightToLeft,
        UpToBottom,
        BottomToUp,
    }
    #endregion

    #region インタフェース
    /*
     *===================================================== 
     * 継承先クラスのインスペクタから継承元の関数を実行出来るようにしたい！
     *=====================================================
     * (失敗)
     * 呼びだしたい関数(継承元)をコンテキストメニューに設定して継承先のインスペクタ(歯車)からアクセス出来るようにする。
     * - 継承元クラスでコンテキストメニューに追加しても継承先クラスのインスペクタで表示されなかった。
     * - Genericクラス実装のためクラスを用意する度に同じ処理をコンテキストメニューに追加するのは手間。
     * 
     * (成功)
     * Editor拡張を使ってインスペクタにボタンを設置。
     * 呼びだしたい関数をボタン押下で任意に呼びだせるようにする。
     * - interfaceのoutが肝。
     * - interface無しだとキャスト出来ない。
     * - 参考:https://gamedev.stackexchange.com/questions/178747/how-to-create-an-editor-class-for-a-generic-class
     */
    interface IScrollView<out OT> where OT : class
    {
        //ScrollDirection ScrollDirectionImpl { get; set; }
#if UNITY_EDITOR
        /*
         * ScrollViewの継承先クラスのインスペクタに共通で載せたい処理の関数はココに宣言を書く.
         * - 定義はScrollViewに書き、実装をWarpする。
         * - 機能的には多重ディスパッチに近い気がする → 動的なディスパッチ？。
         */
        //void SetupRenderableViewElementImpl();
        Vector2 GizmosOffsetImpl { get; set; }
        bool IsDrawGizmosImpl { get; set; }
        Color GizmosColorImpl { get; set; }
#endif
    }
    #endregion
    [RequireComponent(typeof(ScrollRect))]
    public abstract class ScrollView<T> : UIBehaviour, IScrollView<T> where T : class
    {
        [SerializeField]
        ScrollRect m_scrollRect;
        [SerializeField]
        GameObject m_elementPrefab;
        [SerializeField]
        int m_viewElementCount;// 描画するセルの数
        [SerializeField]
        float m_margin;
        [SerializeField]
        float m_space;
        [SerializeField]
        Vector2 m_elementSize;

        [NonSerialized]
        public ScrollDirection m_scrollDirection = ScrollDirection.UpToBottom;

        #region Impl
        //ScrollRect IScrollView<T>.ScrollRectImpl 
        //{ 
        //    get { return m_scrollRect; }
        //    set { m_scrollRect = value; }
        //}
        //GameObject IScrollView<T>.ElementPrefab
        //{
        //    get { return m_elementPrefab; }
        //    set { m_elementPrefab = value; }
        //}
        //ScrollDirection IScrollView<T>.ScrollDirectionImpl
        //{
        //    get { return m_scrollDirection; }
        //    set { m_scrollDirection = value; }
        //}
        //int IScrollView<T>.ViewElementCountImpl
        //{
        //    get { return m_viewElementCount; }
        //    set { m_viewElementCount = value; }
        //}
        //float IScrollView<T>.MarginImpl
        //{
        //    get { return m_margin; }
        //    set { m_margin = value; }
        //}
        //float IScrollView<T>.SpaceImpl
        //{
        //    get { return m_space; }
        //    set { m_space = value; }
        //}
        //Vector2 IScrollView<T>.ElementSizeImpl
        //{
        //    get { return m_elementSize; }
        //    set { m_elementSize = value; }
        //}
        #endregion

        int m_prevRenderdIndex;//前に描画していた要素のインデックス
        Vector2 m_cachedPosition;

        protected int ElementLength => Elements.Count;
        public abstract List<T> Elements { get; set; }

        protected LinkedList<IScrollElement<T>> CachedElements => new LinkedList<IScrollElement<T>>();
        private RectTransform ViewPort => m_scrollRect.viewport;
        private RectTransform Content => m_scrollRect.content;
        /// <summary>
        /// ビュー描画領域
        /// </summary>
        private int RenderableViewElementCount => 0;

        //public event Action<float> OnValueChanged;

        /// <summary>
        /// 現在の描画領域における開始インデックス.
        /// TODO:精度に難あるかも…
        /// </summary>
        int CurrentStartIndex => (int)(Math.Truncate(ScrolledMovementValue - m_margin) / (ElementSize + m_space));

        int MaxVisibleElementCount => (int)(Math.Ceiling(ViewportArea / (ElementSize + m_space)) + 2);// 中途半端にスクロールした時に両端が見えてしまうので ＋2

        float ElementSize => m_elementSize.y;

        /// <summary>
        /// 要素の占める領域.
        /// </summary>
        float ContentAreaSize => m_margin * 2 + ElementSize * ElementLength + m_space * (ElementLength - 1);

        /// <summary>
        /// 描画領域.
        /// </summary>
        float ViewportArea => m_scrollRect.viewport.rect.height;

        /// <summary>
        /// スクロールして移動した移動量.
        /// </summary>
        float ScrolledMovementValue => (ContentAreaSize - ViewportArea) * m_scrollRect.normalizedPosition.y;

        #region UNITY_EDITOR
#if UNITY_EDITOR
        [NonSerialized]
        public Vector3 m_gizmosOffset = Vector3.zero;
        [NonSerialized]
        public bool m_isDrawGizmos = true;
        [NonSerialized]
        public Color m_drawGizmosColor = Color.red;

        Vector2 IScrollView<T>.GizmosOffsetImpl
        {
            get { return m_gizmosOffset; }
            set { m_gizmosOffset = value; }
        }

        bool IScrollView<T>.IsDrawGizmosImpl
        {
            get { return m_isDrawGizmos; }
            set { m_isDrawGizmos = value; }
        }

        Color IScrollView<T>.GizmosColorImpl
        {
            get { return m_drawGizmosColor; }
            set { m_drawGizmosColor = value; }
        }

        #region GameViewのスクリーンサイズ
        private static string[] GameSceneScreenSize
        {
            get
            {
                return UnityStats.screenRes.Split('x');
            }
        }

        private float GSScreenWidth
        {
            get { return float.Parse(GameSceneScreenSize[0]); }
        }

        private float GSScreentHeight
        {
            get { return float.Parse(GameSceneScreenSize[1]); }
        }
        #endregion

        protected override void Reset()
        {
            if (TryGetComponent(out m_scrollRect))
            {
                m_scrollRect.movementType = ScrollRect.MovementType.Clamped;
            }
        }

        protected void OnDrawGizmos()
        {
            if (!m_isDrawGizmos) return;
            Gizmos.color = m_drawGizmosColor;
            Gizmos.DrawWireCube(m_gizmosOffset + ViewPort.localPosition + new Vector3(ViewPort.rect.center.x, ViewPort.rect.center.y), m_elementSize);
        }

        [Conditional("UNITY_EDITOR")/*, ContextMenu("ビュー内に表示可能な要素数を計算する.")*/]
        private void SetupRenderableViewElement<U>()
        {
            if (ViewPort == null)
            {
                UnityEngine.Debug.LogError($"ViewPort is null.");
                return;
            }

            RectTransform rect = null;
            if (!m_elementPrefab.TryGetComponent(out rect))
            {
                // RectTransform 取得失敗.
                UnityEngine.Debug.LogError($"Failed to get RectTransform from elementPrefab.");
                return;
            }
            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(rect);
            var size = bounds.size;
            var renderableArea = ViewPort.rect.height - (m_margin * 2);
            m_elementSize = size;
            m_viewElementCount = Mathf.FloorToInt(renderableArea / (m_space + size.y));
        }
#endif
        #endregion

        protected override void Start()
        {
            m_scrollRect.onValueChanged.AddListener(OnValueChanged);

            #region 初期化忘れチェック
#if UNITY_EDITOR
            Assert.IsNotNull(Content);
            Assert.IsNotNull(ViewPort);
            // 描画セル数が0だと表示されないのでエラーを出す。
            Assert.AreNotEqual(0, m_viewElementCount, $"<color=red>ViewElementNum is zero!</color>");
#endif
            #endregion
        }

        public virtual void Setup()
        {
#if UNITY_EDITOR
            //  親が設定されてない
            if (Content == null)
            {
                UnityEngine.Debug.LogError($"Element parent is null.");
                return;
            }
            // プレハブにコンポーネントが刺さってない
            if (m_elementPrefab.GetComponent<IScrollElement<T>>() == null)
            {
                UnityEngine.Debug.LogError($"Element prefab is not attatched component.");
                return;
            }
#endif
            // 描画領域に収まる分だけキャッシュを作る
            for (int i = 0; i < m_viewElementCount; i++)
            {
                var element = Instantiate(m_elementPrefab, Content);
                element.SetActive(false);
                CachedElements.AddLast(element.GetComponent<IScrollElement<T>>());
            }
        }

        /// <summary>
        /// スクロール位置に合わせ表示する要素を更新する.
        /// </summary>
        /// <param name="isForceRefresh"></param>
        protected void UpdateElementView(bool isForceRefresh = false)
        {
            // 更新不要なら処理しない
            if (!isForceRefresh && ElementLength == 0) return;

            // スクロール位置の描画開始インデックス
            var startIndex = CurrentStartIndex;

            // 要素のインデックス
            var index = 0;

            // 強制再更新
            if (isForceRefresh)
            {
                // 初期化
                index = startIndex;

                // 描画領域内の要素を開始インデックスから映り得る範囲全てに再更新をかける.
                while (index < startIndex + MaxVisibleElementCount)
                {
                    var temp = CachedElements.First();
                    CachedElements.RemoveFirst();
                    temp.Setup(Elements[index]);
                    CachedElements.AddLast(temp);
                    index++;
                }

                // 描画インデックス更新
                m_prevRenderdIndex = startIndex;
                return;
            }

            var endIndex = Math.Min(startIndex + MaxVisibleElementCount, CachedElements.Count - 1);


            // 不要な更新を避けるために描画するインデックスと前回の描画インデックスを比較、
            // 描画し得る要素は前回の更新でキャッシュしたはずなので差分だけ更新をかける
            if (startIndex > m_prevRenderdIndex)
            {
                index = Math.Max(startIndex, m_prevRenderdIndex + MaxVisibleElementCount);
                while (index < startIndex + MaxVisibleElementCount)
                {
                    var temp = CachedElements.First.Value;
                    CachedElements.RemoveFirst();
                    temp.Setup(Elements[index]);
                    CachedElements.AddLast(temp);
                    index++;
                }
            }
            else if (startIndex < m_prevRenderdIndex)
            {
                index = Math.Min(m_prevRenderdIndex - 1, startIndex + MaxVisibleElementCount - 1);
                while (index >= startIndex)
                {
                    var temp = CachedElements.Last.Value;
                    CachedElements.RemoveLast();
                    temp.Setup(Elements[index]);
                    CachedElements.AddFirst(temp);
                    index--;
                }
            }

            for (int i = 0; i < CachedElements.Count; i++)
            {

            }

            //描画可能
            m_prevRenderdIndex = startIndex;
            //
        }

        /// NOTE
        /// ScrollRectのOnValueChangeの引数は"移動量"ではなく"位置"
        private void OnValueChanged(Vector2 position)
        {
            var pos = position.y;

            //var startIndex = GetRenderdStartIndex(pos);
            //var endIndex = Math.Min(startIndex + MaxVisibleElementCount, ElementLength);

            UpdateElementView();

            // キャッシュ更新
            m_cachedPosition = position;
        }

        /// <summary>
        /// 指定位置の描画開始インデックス.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private int GetRenderdStartIndex(float position)
        {
            return (int)(Math.Truncate(ScrolledMovementValue - m_margin) / (ElementSize + m_space));
        }


        /// <summary>
        /// 描画している要素が変わったか.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsRenderdElementChanged(float position)
        {


            return false;
        }
    }

    #region Editor拡張
#if UNITY_EDITOR
    [CustomEditor(typeof(ScrollView<>), editorForChildClasses: true)]
    public class ScrollViewInspecter : Editor
    {
        IScrollView<object> instance = null;
        IScrollView<object> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (IScrollView<object>)target;
                }
                return instance;
            }
        }
        #region SerializedProperty
        SerializedProperty ScrollRectProp => serializedObject.FindProperty("m_scrollRect");
        SerializedProperty ElementPrefabProp => serializedObject.FindProperty("m_elementPrefab");
        SerializedProperty ViewElementCountProp => serializedObject.FindProperty("m_viewElementCount");
        SerializedProperty MarginProp => serializedObject.FindProperty("m_margin");
        SerializedProperty SpaceProp=> serializedObject.FindProperty("m_space");
        SerializedProperty ElementSizeProp => serializedObject.FindProperty("m_elementSize");
        #endregion

        private static Dictionary<string, GUIStyle> EditorGUIStylesDic = new Dictionary<string, GUIStyle>();

        //TODO:GUIStyleはコンストラクタから定義出来ないので、関数に移してOnEnableに移譲する。もっといいやり方あれば変えたい…
        private void SetupGUIStyleDic()
        {
            EditorGUIStylesDic.Clear();
            EditorGUIStylesDic = new Dictionary<string, GUIStyle>
            {
                /*
                 *===================================================== 
                 * キーの命名規則
                 *===================================================== 
                 * [文法]
                 * 関数名と表示対象をアンダースコアで繋ぐ。
                 * <関数名>_<表示対象>
                 * 使用箇所が複数ある場合。
                 * COMMONもしくはCMN等の汎用性が見て取れる命名を用いる。
                 */
                { "DrawGizmosFlag_Toggle",new GUIStyle{ alignment=TextAnchor.MiddleCenter} },// DrawGizmosFlagって関数のToggle表示に対して使うって意味。
            };
        }
        
        private void OnEnable()
        {
            SetupGUIStyleDic();
        }

        public override void OnInspectorGUI()
        {
            // 更新
            serializedObject.Update();

            // 通常のインスペクタ
            //base.OnInspectorGUI();

            // 「要素(セル)の情報」
            DrawProperty();

            // 「描画可能なセル数を自動で計算する.」GUIボタン表示
            DrawSetupRenderableViewElementButton();

            // 「要素のサイズを計算する.」GUIボタン表示
            DrawCalculateElementSizeButton();

            // 「ギズモ関係」
            DrawGizmos();

            // 保存
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSetupRenderableViewElementButton()
        {
            if (GUILayout.Button($"ScrollRectに描画可能なセル数を計算する."))
            {
                if (Instance != null)
                {
                    SetupRenderableViewElement();
                }
                else
                {
                    UnityEngine.Debug.Log($"<color=orange>{target.GetType().Name}</color> instance is null.");
                }
            }
        }

        private void DrawCalculateElementSizeButton()
        {
            if (GUILayout.Button($"要素のサイズを計算する."))
            {
                if (Instance != null)
                {
                    CalculateElementSize();
                }
                else
                {
                    UnityEngine.Debug.Log($"<color=orange>{target.GetType().Name}</color> instance is null.");
                }
            }
        }

        private void DrawProperty()
        {
            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.PropertyField(ScrollRectProp, new GUIContent($"スクロールレクト"));
                EditorGUILayout.PropertyField(ElementPrefabProp, new GUIContent($"要素用のプレハブ(セル)"));
                EditorGUILayout.PropertyField(ViewElementCountProp, new GUIContent($"要素の描画数"));
                EditorGUILayout.LabelField($"要素の大きさ");
                EditorGUILayout.PropertyField(ElementSizeProp, new GUIContent($""));
                EditorGUILayout.PropertyField(MarginProp, new GUIContent($"余白のサイズ(最初と最後のセル)"));
                EditorGUILayout.PropertyField(SpaceProp, new GUIContent($"要素ごとの間隔"));
            }
        }

        private void DrawGizmos()
        {
            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                // 「ギズモ表示フラグ.」
                DrawGizmosFlag();

                // 「ギズモの表示カラー」
                DrawGizmosColor();

                // 「ギズモの表示位置調整.」
                DrawGizmosOffset();
            }
        }

        private void DrawGizmosFlag()
        {
                GUILayout.Label($"ギズモ表示フラグ.");
                Instance.IsDrawGizmosImpl = EditorGUILayout.Toggle(Instance.IsDrawGizmosImpl);
        }

        private void DrawGizmosColor()
        {
            GUILayout.Label($"ギズモの表示カラー");
            Instance.GizmosColorImpl = EditorGUILayout.ColorField(Instance.GizmosColorImpl);
        }

        private void DrawGizmosOffset()
        {
            GUILayout.Label($"ギズモの表示位置調整.");
            Instance.GizmosOffsetImpl = EditorGUILayout.Vector2Field($"", Instance.GizmosOffsetImpl);
        }

        private void SetupRenderableViewElement()
        {
            var scrollRect = ScrollRectProp.objectReferenceValue as ScrollRect;

            var ViewPort = scrollRect?.viewport;
            if (ViewPort == null)
            {
                UnityEngine.Debug.LogError($"ViewPort is null.");
                return;
            }

            var margin = MarginProp.floatValue;
            var space = SpaceProp.floatValue;
            var renderableArea = ViewPort.rect.height - (margin * 2);
            ViewElementCountProp.intValue = Mathf.FloorToInt(renderableArea / (space + ElementSizeProp.vector2Value.y)); ;
        }

        private void CalculateElementSize()
        {
            var scrollRect = ScrollRectProp.objectReferenceValue as ScrollRect;

            var ViewPort = scrollRect?.viewport;
            if (ViewPort == null)
            {
                UnityEngine.Debug.LogError($"ViewPort is null.");
                return;
            }

            RectTransform rect = null;
            var Prefab = ElementPrefabProp.objectReferenceValue as GameObject;
            if (!Prefab.TryGetComponent(out rect))
            {
                // RectTransform 取得失敗.
                UnityEngine.Debug.LogError($"Failed to get RectTransform from elementPrefab.");
                return;
            }

            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(rect);
            var size = bounds.size;
            ElementSizeProp.vector2Value = size;
        }
    }
#endif
    #endregion
}