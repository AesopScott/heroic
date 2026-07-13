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
        [SerializeField] private float retryInterval = 0.75f;

        private static readonly List<BackgroundMusicPlayer> Instances = new List<BackgroundMusicPlayer>();
        private static bool musicMuted;

        private AudioSource source;
        private float nextRetryAt;

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

            if (musicClip == null)
            {
                musicClip = CreateFallbackLoop();
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
            if (!retryAfterFirstInput || source == null || musicClip == null || source.isPlaying || musicMuted)
            {
                return;
            }

            if (Time.unscaledTime < nextRetryAt)
            {
                return;
            }

            if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                nextRetryAt = Time.unscaledTime + retryInterval;
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
                source.UnPause();
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

        private static AudioClip CreateFallbackLoop()
        {
            const int sampleRate = 44100;
            const float duration = 8f;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float pulse = 0.55f + 0.45f * Mathf.Sin(2f * Mathf.PI * 0.5f * t);
                float drone = Mathf.Sin(2f * Mathf.PI * 110f * t) * 0.08f;
                float fifth = Mathf.Sin(2f * Mathf.PI * 165f * t) * 0.04f;
                float shimmer = Mathf.Sin(2f * Mathf.PI * 440f * t) * 0.015f * pulse;
                samples[i] = drone + fifth + shimmer;
            }

            AudioClip clip = AudioClip.Create("HeroicFallbackMusicLoop", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
