using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Model.Input;

namespace uGUI.ADV
{
    /// <summary>
    /// ADVパート用のテキストウィンドウ
    /// </summary>
    public class AdventureTextWindow : MonoBehaviour
    {
        public enum eMode : byte
        {
            Manual,
            Auto,
            Skip,
        }

        /// <summary>
        /// テキストの表示開始位置
        /// memo.
        /// 0文字目から表示するか1文字目から表示するか。
        /// <c>0</c>:最初にテキストボックスが空で表示される。
        /// <c>1</c>:テキストボックスが1文字目から表示される。
        /// </summary>
        const int c_StartTextPosition = 1;// 0 or 1

        [SerializeField]
        Text m_actorName;
        [SerializeField]
        Text m_message;
        [SerializeField, Range(1e-10f, 1f)]
        float m_waitTimePerCharacter = 0.3f;//1文字当たりの待機時間
        [SerializeField]
        GameObject nextMessageArrowObject;
        

        //文字の表示速度
        //messageSpeed

        //ウィンドウの透過度
        //オート再生待ち
        //ボイス終了待機

        [SerializeField]
        List<string> testData = new List<string>(5);

        int m_displayTextIndex = 0;
        float m_elapsedTime = 0;
        eMode mode = eMode.Manual;
        Queue<AdventureTextData> m_dataQueue = null;
        AdventureTextData m_currentData = null;
        int StartTextIndex => c_StartTextPosition - 1;
        string DisplayText => GetText(m_currentData, m_displayTextIndex);
        bool IsEndOfText
        {
            get
            {
                if (m_currentData == null || string.IsNullOrEmpty(m_currentData.Text)) return false;
                // 表示しているindexが文字列の長さを超過したらテキストが最後まで表示された
                return m_currentData.Text.Length <= m_displayTextIndex;
            }
        }

        // テキスト送り.
        bool IsInputNextText => InputModel.Instance.GetButtonDown("Return");
        // 現在のテキスト一括表示.
        bool IsInputDisplayAllText => InputModel.Instance.GetButtonDown("Return");

        // Start is called before the first frame update
        void Start()
        {
            var list = new List<AdventureTextData>();
            testData.ForEach(_ => list.Add(new AdventureTextData { Text = _, ID = 0, ActorName = "tester" })) ;
            Setup(list);
            SetNextText();
            m_message.text = string.Empty;
        }

        // Update is called once per frame
        void Update()
        {
            OnUpdate();
        }

        public void OnUpdate()
        {
            switch (mode)
            {
                case eMode.Manual: OnManualUpdate(); break;
                case eMode.Auto: OnAutoUpdate(); break;
                case eMode.Skip: OnSkipUpdate(); break;
                default: break;
            }
        }

        #region Manual

        private void OnManualUpdate()
        {
            if (m_currentData != null)
                Debug.LogError(m_currentData.Text);

            // 時間経過による更新
            if (m_elapsedTime >= m_waitTimePerCharacter)
            {
                if (IsEndOfText) SetArrowState(true);
                // 一文字更新
                else SetNextCharacter();
                m_elapsedTime = 0;
            }

            // アクションによる更新.
            //例)クリック
            if (IsInputNextText)
            {
                if (IsEndOfText) SetNextText();
                else SetAllCharacter();
            }

            m_elapsedTime += Time.deltaTime;
        }

        #endregion

        #region Auto
        private void OnAutoUpdate() { }
        #endregion

        #region Skip
        private void OnSkipUpdate() { }
        #endregion

        private void Setup(IEnumerable<AdventureTextData>adventureTexts)
        {
            m_dataQueue = new Queue<AdventureTextData>(adventureTexts);
        }

        public void Play()
        {

        }

        /// <summary>
        /// テキスト表示内容の更新.
        /// </summary>
        private void SetNextText()
        {
            if (!m_dataQueue.Any()) return;

            m_currentData = m_dataQueue.Dequeue();
            m_displayTextIndex = StartTextIndex;
            SetArrowState(false);
        }

        /// 一括表示更新.
        private void SetAllCharacter()
        {
            var text = GetText(m_currentData);
            m_displayTextIndex = text.Length;
            m_message.text = text;
        }

        /// 一文字更新.
        private void SetNextCharacter()
        {
            m_displayTextIndex++;
            m_message.text = DisplayText;
        }

        /// 一括表示
        private string GetText(AdventureTextData data) => data != null ? data.Text : string.Empty;

        /// 指定文字まで表示
        private string GetText(AdventureTextData data, int index)
        {
            if (data == null || index < 0 || string.IsNullOrEmpty(data.Text)) return string.Empty;

            // インデックスが表示する文字列数を超過していたら規定文字列数に丸める
            var i = Math.Min(data.Text.Length, index);
            return data.Text.Substring(0, i);
        }

        private void SetArrowState(bool state)
        {
            if (nextMessageArrowObject != null)
            {
                // コールバック処理が刺さってた場合に不要に呼びだしたくないのでフラグ判定する.
                if (nextMessageArrowObject.activeSelf == state) return;
                nextMessageArrowObject.gameObject.SetActive(state);
            }
        }
    }
}