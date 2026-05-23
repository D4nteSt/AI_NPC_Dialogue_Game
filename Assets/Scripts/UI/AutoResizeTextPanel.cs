using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class AutoResizeTextPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private TextMeshProUGUI text;

    [Header("Padding")]
    [SerializeField] private float horizontalPadding = 32f;
    [SerializeField] private float verticalPadding = 20f;

    [Header("Size Limits")]
    [SerializeField] private float minWidth = 260f;
    [SerializeField] private float maxWidth = 560f;
    [SerializeField] private float minHeight = 50f;
    [SerializeField] private float maxHeight = 180f;

    [Header("Behavior")]
    [SerializeField] private bool resizeWidth = true;
    [SerializeField] private bool resizeHeight = true;
    [SerializeField] private bool updateTextRect = true;

    private string lastText;

    private void Reset()
    {
        panelRect = GetComponent<RectTransform>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Awake()
    {
        if (panelRect == null)
            panelRect = GetComponent<RectTransform>();

        Refresh();
    }

    private void LateUpdate()
    {
        if (text == null)
            return;

        if (lastText != text.text)
            Refresh();
    }

    public void Refresh()
    {
        if (panelRect == null || text == null)
            return;

        lastText = text.text;

        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Overflow;

        float preferredWidthLimit = Mathf.Max(50f, maxWidth - horizontalPadding);
        Vector2 preferred = text.GetPreferredValues(text.text, preferredWidthLimit, Mathf.Infinity);

        float targetWidth = panelRect.sizeDelta.x;
        float targetHeight = panelRect.sizeDelta.y;

        if (resizeWidth)
        {
            targetWidth = Mathf.Clamp(
                preferred.x + horizontalPadding,
                minWidth,
                maxWidth
            );
        }

        float textWidth = Mathf.Max(50f, targetWidth - horizontalPadding);

        if (resizeHeight)
        {
            Vector2 preferredHeight = text.GetPreferredValues(text.text, textWidth, Mathf.Infinity);

            targetHeight = Mathf.Clamp(
                preferredHeight.y + verticalPadding,
                minHeight,
                maxHeight
            );
        }

        panelRect.sizeDelta = new Vector2(targetWidth, targetHeight);

        if (updateTextRect)
        {
            RectTransform textRect = text.rectTransform;

            textRect.anchorMin = new Vector2(0f, 0f);
            textRect.anchorMax = new Vector2(1f, 1f);
            textRect.pivot = new Vector2(0.5f, 0.5f);

            float xPadding = horizontalPadding * 0.5f;
            float yPadding = verticalPadding * 0.5f;

            textRect.offsetMin = new Vector2(xPadding, yPadding);
            textRect.offsetMax = new Vector2(-xPadding, -yPadding);
        }
    }
}