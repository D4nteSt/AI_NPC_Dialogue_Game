using TMPro;
using UnityEngine;

public class DialogueMessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;

    public void SetText(string text)
    {
        messageText.text = text;
    }
}