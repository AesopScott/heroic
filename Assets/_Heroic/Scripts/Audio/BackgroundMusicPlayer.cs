using UnityEngine;
using System.Collections.Generic;

namespace Heroic.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip musicClip;
        [SerializeField] private string resourcesClipPath = "Audio/Music/HeroicDemoLoop";
        [SerializeField] private float volume = 0.24f;
        [SerializeField] private bool playOnStart = true;
        [SerializeField] private bool loop = true;
        [SerializeField] private bool retryAfterFirstInput = true;

        private static readonly List<BackgroundMusicPlayer> Instances = new List<BackgroundMusicPlayer>();
        private static bool musicMuted;

        private AudioSource source;
        private bool inputRetryUsed;

        public static bool MusicMuted => musicMuted;

        public static void SetMusicMuted(bool muted)
        {
            musicMuted = muted;
            foreach (BackgroundMusicPlayer player in Instances)
            {
                player.ApplyAudioState();
            }
        }

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            source.volume = volume;

            if (musicClip == null && !string.IsNullOrWhiteSpace(resourcesClipPath))
            {
                musicClip = Resources.Load<AudioClip>(resourcesClipPath);
            }

            ApplyAudioState();
        }

        private void OnEnable()
        {
            if (!Instances.Contains(this))
            {
                Instances.Add(this);
            }

            ApplyAudioState();
        }

        private void OnDisable()
        {
            Instances.Remove(this);
        }

        private void Start()
        {
            if (playOnStart)
            {
                Play();
            }
        }

        private void Update()
        {
            if (!retryAfterFirstInput || inputRetryUsed || source == null || musicClip == null || source.isPlaying)
            {
                return;
            }

            if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                inputRetryUsed = true;
                Play();
            }
        }

        public void SetMusicClip(AudioClip clip)
        {
            musicClip = clip;
            if (source != null)
            {
                source.clip = musicClip;
                ApplyAudioState();
            }
        }

        public void SetVolume(float newVolume)
        {
            volume = Mathf.Clamp01(newVolume);
            if (source != null)
            {
                ApplyAudioState();
            }
        }

        public void Play()
        {
            if (source == null || musicClip == null)
            {
                return;
            }

            source.clip = musicClip;
            source.loop = loop;
            ApplyAudioState();

            if (!source.isPlaying)
            {
                source.Play();
            }
        }

        public void Stop()
        {
            if (source != null)
            {
                source.Stop();
            }
        }

        private void ApplyAudioState()
        {
            if (source == null)
            {
                return;
            }

            source.volume = volume;
            source.mute = musicMuted;
        }
    }
}
