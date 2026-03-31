using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI dialogueHistoryText;
    [SerializeField] private TMP_InputField playerInputField;

    public void Show()
    {
        dialoguePanel.SetActive(true);
    }

    public void Hide()
    {
        dialoguePanel.SetActive(false);
    }

    public void SetNPCName(string npcName)
    {
        npcNameText.text = npcName;
    }

    public void ClearHistory()
    {
        dialogueHistoryText.text = string.Empty;
    }

    public void AddMessage(string sender, string message)
    {
        dialogueHistoryText.text += sender + ": " + message + "\n";
    }

    public string GetInputText()
    {
        return playerInputField.text;
    }

    public void ClearInput()
    {
        playerInputField.text = string.Empty;
        playerInputField.ActivateInputField();
    }
}