using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCDialogueData dialogueData;
    [SerializeField] private DialogueManager dialogueManager;

    public void Interact()
    {
        if (dialogueData == null || dialogueManager == null) return;

        dialogueManager.StartDialogue(dialogueData);
    }

    public string GetInteractionText()
    {
        if (dialogueData == null)
            return "Нажмите E для взаимодействия";

        return "Нажмите E, чтобы поговорить с " + dialogueData.NPCName;
    }
}