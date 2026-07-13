using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Heroic.UI
{
    public class SkillTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerDownHandler
    {
        private string title;
        private string body;

        private static RectTransform tooltipRoot;
        private static TMP_Text titleText;
        private static TMP_Text bodyText;

        public void Configure(string newTitle, string newBody)
        {
            title = newTitle;
            body = newBody;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(body))
            {
                return;
            }

            EnsureTooltip();
            if (tooltipRoot == null || titleText == null || bodyText == null)
            {
                return;
            }

            titleText.text = title;
            bodyText.text = body;
            tooltipRoot.gameObject.SetActive(true);
            MoveTo(eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (tooltipRoot != null && tooltipRoot.gameObject.activeSelf)
            {
                MoveTo(eventData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideActiveTooltip();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            HideActiveTooltip();
        }

        private void OnDisable()
        {
            HideActiveTooltip();
        }

        public static void HideActiveTooltip()
        {
            if (tooltipRoot != null)
            {
                tooltipRoot.gameObject.SetActive(false);
            }
        }

        private static void EnsureTooltip()
        {
            if (tooltipRoot != null)
            {
                return;
            }

            Canvas canvas = CreateTooltipCanvas();
            GameObject root = new GameObject("SkillTooltip");
            root.transform.SetParent(canvas.transform, false);
            tooltipRoot = root.AddComponent<RectTransform>();
            tooltipRoot.anchorMin = new Vector2(0f, 0f);
            tooltipRoot.anchorMax = new Vector2(0f, 0f);
            tooltipRoot.pivot = new Vector2(0f, 1f);
            tooltipRoot.sizeDelta = new Vector2(360f, 190f);

            Image background = root.AddComponent<Image>();
            background.color = new Color(0.015f, 0.028f, 0.04f, 0.94f);
            background.raycastTarget = false;

            Outline outline = root.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 0.82f, 0.28f, 0.88f);
            outline.effectDistance = new Vector2(2f, -2f);

            titleText = CreateText("Title", root.transform, new Vector2(332f, 42f), new Vector2(14f, -12f), 22f, FontStyles.Bold, new Color(1f, 0.86f, 0.42f, 1f));
            bodyText = CreateText("Body", root.transform, new Vector2(332f, 120f), new Vector2(14f, -56f), 16f, FontStyles.Normal, new Color(0.84f, 0.94f, 1f, 1f));
            tooltipRoot.gameObject.SetActive(false);
            tooltipRoot.SetAsLastSibling();
        }

        private static Canvas CreateTooltipCanvas()
        {
            GameObject canvasObject = new GameObject("SkillTooltipCanvas");
            DontDestroyOnLoad(canvasObject);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 500;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static TMP_Text CreateText(string name, Transform parent, Vector2 size, Vector2 position, float fontSize, FontStyles style, Color color)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;

            TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.color = color;
            text.alignment = TextAlignmentOptions.TopLeft;
            text.enableWordWrapping = true;
            text.raycastTarget = false;
            return text;
        }

        private static void MoveTo(PointerEventData eventData)
        {
            if (tooltipRoot == null || eventData == null)
            {
                return;
            }

            Vector2 position = eventData.position + new Vector2(24f, -18f);
            Vector2 size = tooltipRoot.sizeDelta;
            position.x = Mathf.Min(position.x, Screen.width - size.x - 12f);
            position.y = Mathf.Max(position.y, size.y + 12f);
            tooltipRoot.anchoredPosition = position;
            tooltipRoot.SetAsLastSibling();
        }
    }
}
