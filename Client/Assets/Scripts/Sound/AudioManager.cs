using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Sound
{
    using SoundTable = Dictionary<string, AudioData>;
    using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
//[ExecuteAlways]
#endif
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        //  serialize param
        [Header("Sound Manager Parameter")]
        [SerializeField, Tooltip("チャネル数"), Range(1, 16)] int channelNum = 1;
        [SerializeField, Tooltip("ONならメインカメラではなく、このオブジェクトがListenerを担当する")] bool isAttachedListener = false;

        //  private param!
        [SerializeField, HideInInspector] List<AudioChannel> channels = new List<AudioChannel>();
        public List<AudioChannel> ChannelList { get { return channels; } }
        //Dictionary<string, SoundData> sounds;
        SoundTable sounds;

        //  CallBack
        /// <summary>
        /// サウンド再生のコールバック.
        /// <br>* 再生開始
        /// <br>* 再生終了
        /// <br>* 一時停止時
        /// <br>* 一時停止解除
        /// <br>* フェードイン
        /// <br>* フェードアウト
        /// </summary>
        public delegate void SoundCallback();

        protected override void Awake()
        {
            //基底クラスの処理
            base.Awake();
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Instance.ChannelList.Clear();
                return;
            }
#endif

            //Listenerのセット
            SetListener();
            //チャンネルの生成
            CreateChannel();
            //チャンネルの取得
            //SetChannel();
        }

        private void Update()
        {
#if UNITY_EDITOR
            return;
#endif
        }

        /// <summary>
        /// AudioListenerのセット
        /// </summary>
        [ContextMenu("SetAudioListener")]
        void SetListener()
        {
            //AudioListenerを設定

            //メインカメラの参照が取得できない場合(カメラのタグをMainCameraに変えてしまうので注意)
            if (Camera.main == null)
            {
                GameObject.Find("Main Camera").tag = "MainCamera";
            }

            AudioListener destroy = null;//破棄させるListener
            GameObject add = null;//Listenerを追加するGameObject

            //Listenerをこのオブジェクトに付属させる
            if (isAttachedListener)
            {
                //カメラにListenerが付いていたら破棄オブジェクトにする
                destroy = Camera.main.GetComponent<AudioListener>() ?? null;

                //オブジェクトにListenerが付いていなかったら、このオブジェクトをListenerを付与するオブジェクトにする
                add = GetComponent<AudioListener>() ? null : this.gameObject;
            }
            //Listenerをメインカメラに付属させる
            else
            {
                //このオブジェクトにListenerが付いていたら破棄オブジェクトにする
                destroy = GetComponent<AudioListener>() ?? null;

                //カメラにListenerが付いていなかったら、メインカメラをListenerを付与するオブジェクトにする
                add = Camera.main.GetComponent<AudioListener>() ? null : Camera.main.gameObject;
            }

            //破棄するListenerが存在すればListenerを破棄する
            if (destroy)
            {
#if UNITY_EDITOR
                DestroyImmediate(destroy);
#else
            Destroy(destroy);
#endif
            }

            //Listenerが付いていなかったらオブジェクトに追加する
            if (add) { add.AddComponent<AudioListener>(); }
        }
        /// <summary>
        /// チャンネルの用意
        /// </summary>
        [ContextMenu("SetChannel")]
        void CreateChannel()
        {
            AudioSource[] sources = this.gameObject.GetComponents<AudioSource>();
            int createCount = 0;

            //一つもAudioSourceがアタッチされてない場合
            if (sources == null)
            {
                createCount = channelNum;
            }
            //AudioSourceの個数がチャネル数と一致しない場合
            else if (sources.Length != channelNum)
            {
                //コンポーネントの個数を1個にする(RequireComponentで1個は付いてしまうため)
                for (int i = 0; i < sources.Length - 1; ++i)
                {
#if UNITY_EDITOR
                    GameObject.DestroyImmediate(sources[i]);
#else
                Destroy(sources[i]);
#endif
                }

                //チャネルの1番目のPlayOnAwakeをOFF
                GetComponent<AudioSource>().playOnAwake = false;

                //"チャネル生成数 - 1" を追加数とする
                createCount = channelNum - 1;
            }

            //コンポーネントを追加数分だけ追加する
            for (int i = 0; i < createCount; ++i)
            {
                //追加したコンポーネントを取得
                var audioSource = this.gameObject.AddComponent<AudioSource>();

                //PlayOnAwakeをOFFにする
                audioSource.playOnAwake = false;
            }
        }

        public void SetChannel(AudioChannel channel)
        {
            if (ChannelList.Count <= channel.Index)
            {
                ChannelList.Add(channel);
            }
            else if (ChannelList.Any(x=>x.Index==channel.Index))
            {
                ChannelList[channel.Index] = channel;
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    Debug.Log($"Log: Update channel -> index{channel.Index}");
                }
#endif
            }
            else
            {
                Debug.LogError($"Error: AudioManager.SetChannel ChannelList.Count:{ChannelList.Count},Index:{channel.Index}");
            }
        }

        [ContextMenu("ClearChannelList")]
        public void ClearChannelList()
        {
            ChannelList.Clear();
        }

        [Conditional("UNITY_EDITOR")]
        public void RefreshChannelList()
        {
            ChannelList.Clear();
            var channels = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(AudioChannel)).Select(x => x as AudioChannel);
            foreach(var channel in channels)
            {
                Debug.Log("channel", this);
                SetChannel(channel);
            }
        }
        //SoundChannel GetChannel(int index)
        //{
        //    if (index < 0 || channelNum <= index)
        //    {
        //        return null;
        //    }

        //    return channels[index];
        //}

        public void Play(int soundIndex, int channelIndex = 0, bool isLoop = false)
        {
            channelIndex = Math.Clamp(channelIndex, 0, channelNum);
        }


    }
}