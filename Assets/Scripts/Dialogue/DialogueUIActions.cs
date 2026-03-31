using UnityEngine;

public class DialogueUIActions : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueUI dialogueUI;

    public void OnSendButtonClicked()
    {
        Debug.Log("Нажата кнопка Send");
        string message = dialogueUI.GetInputText();
        dialogueManager.SendPlayerMessage(message);
    }

    public void OnCloseButtonClicked()
    {
        Debug.Log("Нажата кнопка Close");
        dialogueManager.CloseDialogue();
    }
}