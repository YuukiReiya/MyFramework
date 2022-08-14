using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioChannel : BehaviourBase
    {
        public AudioSource Source { get; set; } = null;
        public bool IsPlayed { get; set; } = false;
        [SerializeField]
        public int Index { get; private set; }
        private FadeState state = FadeState.None;

        // event
        /// <summary>
        /// 再生開始
        /// </summary>
        public event AudioManager.SoundCallback OnPlayStarted = null;
        /// <summary>
        /// 再生終了
        /// </summary>
        public event AudioManager.SoundCallback OnPlayFinished = null;
        /// <summary>
        /// 再生停止
        /// </summary>
        public event AudioManager.SoundCallback OnPlayStoped = null;
        /// <summary>
        /// 一時停止
        /// </summary>
        public event AudioManager.SoundCallback OnPaused = null;
        /// <summary>
        /// 一時停止解除
        /// </summary>
        public event AudioManager.SoundCallback OnUnpaused = null;
        /// <summary>
        /// フェードイン(小さくなる方)終了時
        /// </summary>
        public event AudioManager.SoundCallback OnFadeInFinished = null;
        /// <summary>
        /// フェードアウト(大きくなる方)終了時
        /// </summary>
        public event AudioManager.SoundCallback OnFadeOutFinished = null;

        //  enum
        private enum FadeState { None, FadeIn, FadeOut }

        protected override void Awake()
        {
            state = FadeState.None;
        }
    }
}
