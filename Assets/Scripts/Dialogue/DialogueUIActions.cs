using UnityEngine;

public class DialogueUIActions : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueUI dialogueUI;

    public async void OnSendButtonClicked()
    {
        string message = dialogueUI.GetInputText();
        await dialogueManager.SendPlayerMessageAsync(message);
    }

    public void OnCloseButtonClicked()
    {
        dialogueManager.CloseDialogue();
    }
}