using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine.Networking;
using Expansion;

namespace Editor.Expansion
{
    public class SoundPlayer : EditorWindow
    {
        private static SoundPlayer Instance { get; } = new SoundPlayer();

        private const string WindowName = @"Sound Player in Editor";
        #region XML
        private const string RootTag = @"Root";
        private const string ResourcesTag = @"Resources";
        private const string ResourceTag = @"Resource";
        private const string EnableTag = @"enable";
        private const string UriTag = @"uri";
        private string xmlUri = @"Assets/Editor/SoundPlayer/editor.xml";
        #endregion
        private AudioSource source = null;
        private AudioListener listener = null;
        private List<AudioClip> clipList = null;
        private bool isStreaming = true;
        private int index = 0;
        // Update
        private bool isUpdate = true;

        [ExecuteInEditMode]
        private void Update()
        {
            if (!isUpdate) return;

            if (!Instance.source.isPlaying)
            {
                NextAudioList();
            }
        }

        [MenuItem("Sound/PlayInEditor")]
        private static void PlayInEditor()
        {
            if (Instance.clipList.Count <= 0)
            {
                Debug.LogError($"clip list is nothing.");
                return;
            }

            Instance.index = Math.Clamp(Instance.index, 0, Instance.clipList.Count);
            Instance.source.clip = Instance.clipList[Instance.index];
            Instance.source.Play();
        }

        [MenuItem("Sound/SoundPlayerInEditorWindow")]
        private static void ShowInEditorWindow()
        {
            EditorWindow.GetWindow<SoundPlayer>(true, WindowName, true);
        }

        private void Setup()
        {
            Cleanup();
            listener = FindObjectOfType<AudioListener>(true);
            if(listener==null)
            {
                listener = new AudioListener();
            }
            if (clipList == null)
            {
                clipList = new List<AudioClip>();
            }
            source = FindObjectOfType<AudioSource>();
            if(source == null)
            {
                source = new GameObject("AudioSource").AddComponent<AudioSource>();
            }
        }

        private void Cleanup()
        {
            if (clipList != null)
            {
                clipList.ForEach(clip =>
                {
                    if (clip != null)
                    {
                        clip.UnloadAudioData();
                    }
                    clip = null;
                });
                clipList.Clear();
            }
            clipList = null;
            listener = null;
            if (source != null)
            {
                source.Stop();
            }
            source = null;
        }

        [MenuItem("Assets/Sound/LoadAndPlayInXML")]
        private static void LoadAudioClipAndPlayInXML()
        {
            var ext = Path.GetExtension(Instance.xmlUri).ToLower();
            if (ext != @".xml" || !File.Exists(Instance.xmlUri))
            {
                Debug.LogError($"Extension:{ext} uri:{Instance.xmlUri}");
                Instance.Cleanup();
                return;
            }

            Instance.Setup();

            var doc = XDocument.Load(Instance.xmlUri);
            var root = doc.Element(RootTag);
            var resources = root.Elements(ResourcesTag);
            var elements = resources.Elements(ResourceTag);
            foreach (var element in elements)
            {
                var enable = element.Element(EnableTag).Value;
                var uri = WindowsExpansion.Convert(element.Element(UriTag).Value);

                if (!enable.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (!File.Exists(uri))
                {
                    Debug.LogError($"FileNotFoundException:{uri}");
                    continue;
                }

                var e = Instance.GetDownloadAudioClip(uri, Instance.isStreaming, (clip) =>
                {
                    if (clip != null)
                    {
                        Instance.clipList.Add(clip);
                    }
                });
                while (e.MoveNext()) { }
            }
            PlayInEditor();
        }

        [MenuItem("Assets/Sound/NextAudioList")]
        private static void NextAudioList()
        {
            if (Instance.clipList.Count <= 0)
            {
                Debug.LogError($"clip list is nothing.");
                return;
            }

            Instance.index = ++Instance.index % Instance.clipList.Count;
            Instance.source.clip = Instance.clipList[Instance.index];
            if (Instance.source.isPlaying)
            {
                Instance.source.Stop();
            }
            Instance.source.Play();
        }

        [MenuItem("Assets/Sound/PrevAudioList")]
        private static void PrevAudioList()
        {
            if (Instance.clipList.Count <= 0)
            {
                Debug.LogError($"clip list is nothing.");
                return;
            }

            Instance.index = --Instance.index < 0 ? Instance.clipList.Count - 1 : Instance.index % Instance.clipList.Count;
            Instance.source.clip = Instance.clipList[Instance.index];
            if (Instance.source.isPlaying)
            {
                Instance.source.Stop();
            }
            Instance.source.Play();
        }

        private IEnumerator GetDownloadAudioClip(string uri, bool isStreaming, Action<AudioClip> action)
        {
            using (var request = UnityWebRequestMultimedia.GetAudioClip(uri, GetAudioType(uri)))
            {
                ((DownloadHandlerAudioClip)request.downloadHandler).streamAudio = isStreaming;

                request.SendWebRequest();
                while (!request.isDone) yield return null;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var content = DownloadHandlerAudioClip.GetContent(request);
                    action?.Invoke(content);
                }
                else
                {
                    Debug.LogError($"Result:{request.result}{Environment.NewLine}Error:{request.error}{Environment.NewLine}uri:{uri}");
                }
            }
        }

        private static AudioType GetAudioType(string uri)
        {
            return Path.GetExtension(uri).ToLower() switch
            {
                @".mp3" => AudioType.MPEG,
                @".wav" => AudioType.WAV,
                @".wave" => AudioType.WAV,
                @".ogg" => AudioType.OGGVORBIS,
                _ => AudioType.UNKNOWN,
            };
        }
    }
}