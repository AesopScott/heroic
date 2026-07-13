using Heroic.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heroic.UI
{
    public class AudioControlsPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private Button muteButton;
        [SerializeField] private Button lowerButton;
        [SerializeField] private Button raiseButton;

        private void Awake()
        {
            EnsureUi();
            WireButtons();
        }

        private void Update()
        {
            RefreshStatus();
        }

        private void EnsureUi()
        {
            RectTransform rect = transform as RectTransform;
            if (rect == null)
            {
                rect = gameObject.AddComponent<RectTransform>();
            }

            rect.sizeDelta = new Vector2(260f, 74f);

            Image panel = GetComponent<Image>();
            if (panel == null)
            {
                panel = gameObject.AddComponent<Image>();
            }

            panel.color = new Color(0.02f, 0.05f, 0.08f, 0.72f);
            statusText ??= CreateLabel("AudioStatus", new Vector2(244f, 26f), new Vector2(0f, 20f), 16f);
            muteButton ??= CreateButton("MuteButton", "Mute", new Vector2(86f, 28f), new Vector2(-82f, -18f));
            lowerButton ??= CreateButton("LowerButton", "-", new Vector2(54f, 28f), new Vector2(0f, -18f));
            raiseButton ??= CreateButton("RaiseButton", "+", new Vector2(54f, 28f), new Vector2(62f, -18f));
        }

        private void WireButtons()
        {
            muteButton.onClick.RemoveListener(DemoAudioControls.ToggleMusicMuteStatic);
            lowerButton.onClick.RemoveListener(DemoAudioControls.LowerMasterVolumeStatic);
            raiseButton.onClick.RemoveListener(DemoAudioControls.RaiseMasterVolumeStatic);
            muteButton.onClick.AddListener(DemoAudioControls.ToggleMusicMuteStatic);
            lowerButton.onClick.AddListener(DemoAudioControls.LowerMasterVolumeStatic);
            raiseButton.onClick.AddListener(DemoAudioControls.RaiseMasterVolumeStatic);
        }

        private void RefreshStatus()
        {
            if (statusText == null)
            {
                return;
            }

            int volumePercent = Mathf.RoundToInt(DemoAudioControls.MasterVolume * 100f);
            string muteState = BackgroundMusicPlayer.MusicMuted ? "Music Muted" : "Music On";
            statusText.text = $"Audio {volumePercent}%  |  {muteState}";
        }

        private TMP_Text CreateLabel(string name, Vector2 size, Vector2 position, float fontSize)
        {
            GameObject labelObject = new GameObject(name);
            labelObject.transform.SetParent(transform, false);
            RectTransform rect = labelObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;

            TMP_Text label = labelObject.AddComponent<TextMeshProUGUI>();
            label.alignment = TextAlignmentOptions.Center;
            label.fontSize = fontSize;
            label.fontStyle = FontStyles.Bold;
            label.color = new Color(0.76f, 0.94f, 1f, 1f);
            label.raycastTarget = false;
            return label;
        }

        private Button CreateButton(string name, string labelText, Vector2 size, Vector2 position)
        {
            GameObject buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(transform, false);
            RectTransform rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.08f, 0.18f, 0.24f, 0.95f);

            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;

            TMP_Text label = CreateLabel("Label", size, Vector2.zero, 15f);
            label.transform.SetParent(buttonObject.transform, false);
            label.text = labelText;
            label.color = Color.white;
            return button;
        }
    }
}
