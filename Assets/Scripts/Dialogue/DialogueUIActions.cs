using UnityEngine;

public class DialogueUIActions : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueUI dialogueUI;

    public void OnCloseButtonClicked()
    {
        dialogueManager.CloseDialogue();
    }
}