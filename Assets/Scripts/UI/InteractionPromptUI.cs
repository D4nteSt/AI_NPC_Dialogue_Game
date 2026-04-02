using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private GameplayUIController gameplayUIController;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    private void Update()
    {
        bool shouldHide =
            (dialogueManager != null && dialogueManager.IsDialogueOpen) ||
            (gameplayUIController != null && gameplayUIController.IsAnyGameplayPanelOpen);

        if (shouldHide)
        {
            promptPanel.SetActive(false);
            return;
        }

        if (playerInteraction.CurrentInteractable != null)
        {
            promptPanel.SetActive(true);
            promptText.text = playerInteraction.CurrentInteractable.GetInteractionText();
        }
        else
        {
            promptPanel.SetActive(false);
        }
    }
}