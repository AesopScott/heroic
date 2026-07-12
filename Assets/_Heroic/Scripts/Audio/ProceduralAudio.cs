using System.Collections.Generic;
using UnityEngine;

namespace Heroic.Audio
{
    public static class ProceduralAudio
    {
        private const int SampleRate = 44100;
        private static readonly Dictionary<string, AudioClip> ClipCache = new Dictionary<string, AudioClip>();

        public static AudioClip Tone(string key, float frequency, float duration, float volume, float descend = 0f)
        {
            if (ClipCache.TryGetValue(key, out AudioClip cachedClip))
            {
                return cachedClip;
            }

            int sampleCount = Mathf.Max(1, Mathf.CeilToInt(SampleRate * duration));
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float percent = i / (float)sampleCount;
                float envelope = Mathf.Sin(percent * Mathf.PI);
                float currentFrequency = Mathf.Max(40f, frequency - descend * percent);
                samples[i] = Mathf.Sin(2f * Mathf.PI * currentFrequency * i / SampleRate) * volume * envelope;
            }

            AudioClip clip = AudioClip.Create(key, sampleCount, 1, SampleRate, false);
            clip.SetData(samples, 0);
            ClipCache[key] = clip;
            return clip;
        }
    }
}
