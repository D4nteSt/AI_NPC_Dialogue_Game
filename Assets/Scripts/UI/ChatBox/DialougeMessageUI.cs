using TMPro;
using UnityEngine;

public class DialogueMessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private MessageBubbleMaxWidth bubbleWidthController;

    public void SetText(string text)
    {
        if (messageText != null)
            messageText.text = text;

        Canvas.ForceUpdateCanvases();

        if (bubbleWidthController != null)
            bubbleWidthController.RefreshWidth();
    }
}