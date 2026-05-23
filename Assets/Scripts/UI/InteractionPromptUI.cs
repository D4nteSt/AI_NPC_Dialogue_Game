using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private GameplayUIController gameplayUIController;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("UI")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private AutoResizeTextPanel autoResizeTextPanel;

    private string lastPromptText;

    private void Update()
    {
        bool shouldHide =
            (dialogueManager != null && dialogueManager.IsDialogueOpen) ||
            (gameplayUIController != null && gameplayUIController.IsAnyGameplayPanelOpen);

        if (shouldHide)
        {
            HidePrompt();
            return;
        }

        if (playerInteraction != null && playerInteraction.CurrentInteractable != null)
        {
            string interactionText = playerInteraction.CurrentInteractable.GetInteractionText();
            ShowPrompt(interactionText);
        }
        else
        {
            HidePrompt();
        }
    }

    private void ShowPrompt(string text)
    {
        if (promptPanel != null && !promptPanel.activeSelf)
            promptPanel.SetActive(true);

        if (promptText == null)
            return;

        if (lastPromptText == text)
            return;

        lastPromptText = text;
        promptText.text = text;

        if (autoResizeTextPanel != null)
            autoResizeTextPanel.Refresh();
    }

    private void HidePrompt()
    {
        lastPromptText = string.Empty;

        if (promptPanel != null && promptPanel.activeSelf)
            promptPanel.SetActive(false);
    }
}