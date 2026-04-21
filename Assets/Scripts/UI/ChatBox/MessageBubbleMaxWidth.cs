using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
public class MessageBubbleMaxWidth : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private float maxWidth = 420f;
    [SerializeField] private float horizontalPadding = 32f;

    private void Reset()
    {
        layoutElement = GetComponent<LayoutElement>();
    }

    public void RefreshWidth()
    {
        if (messageText == null || layoutElement == null)
            return;

        float preferredTextWidth = messageText.GetPreferredValues(messageText.text, maxWidth, 0f).x;
        float targetWidth = Mathf.Min(maxWidth, preferredTextWidth + horizontalPadding);

        layoutElement.preferredWidth = targetWidth;
    }
}
