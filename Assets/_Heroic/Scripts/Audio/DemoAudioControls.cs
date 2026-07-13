using UnityEngine;

namespace Heroic.Audio
{
    public class DemoAudioControls : MonoBehaviour
    {
        [SerializeField] private KeyCode toggleMusicMuteKey = KeyCode.M;
        [SerializeField] private KeyCode lowerMasterVolumeKey = KeyCode.Minus;
        [SerializeField] private KeyCode lowerMasterVolumeKeyAlt = KeyCode.KeypadMinus;
        [SerializeField] private KeyCode raiseMasterVolumeKey = KeyCode.Equals;
        [SerializeField] private KeyCode raiseMasterVolumeKeyAlt = KeyCode.KeypadPlus;
        [SerializeField] private float volumeStep = 0.1f;

        private static float masterVolume = 1f;

        public static float MasterVolume => masterVolume;

        private void Awake()
        {
            ApplyMasterVolume();
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleMusicMuteKey))
            {
                BackgroundMusicPlayer.SetMusicMuted(!BackgroundMusicPlayer.MusicMuted);
            }

            if (Input.GetKeyDown(lowerMasterVolumeKey) || Input.GetKeyDown(lowerMasterVolumeKeyAlt))
            {
                SetMasterVolume(masterVolume - volumeStep);
            }
            else if (Input.GetKeyDown(raiseMasterVolumeKey) || Input.GetKeyDown(raiseMasterVolumeKeyAlt))
            {
                SetMasterVolume(masterVolume + volumeStep);
            }
        }

        public void ToggleMusicMute()
        {
            ToggleMusicMuteStatic();
        }

        public void LowerMasterVolume()
        {
            LowerMasterVolumeStatic();
        }

        public void RaiseMasterVolume()
        {
            RaiseMasterVolumeStatic();
        }

        public static void ToggleMusicMuteStatic()
        {
            BackgroundMusicPlayer.SetMusicMuted(!BackgroundMusicPlayer.MusicMuted);
        }

        public static void LowerMasterVolumeStatic()
        {
            SetMasterVolume(masterVolume - 0.1f);
        }

        public static void RaiseMasterVolumeStatic()
        {
            SetMasterVolume(masterVolume + 0.1f);
        }

        public static void SetMasterVolume(float value)
        {
            masterVolume = Mathf.Clamp01(value);
            ApplyMasterVolume();
        }

        public static void ApplyMasterVolume()
        {
            AudioListener.volume = masterVolume;
        }
    }
}
