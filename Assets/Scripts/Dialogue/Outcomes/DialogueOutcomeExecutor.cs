using UnityEngine;

public class DialogueOutcomeExecutor : MonoBehaviour, IDialogueOutcomeExecutor
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private NotificationUI notificationUI;

    public void Execute(DialogueOutcome outcome)
    {
        if (outcome == null || !outcome.HasActions)
            return;

        foreach (var action in outcome.Actions)
        {
            ExecuteAction(action);
        }
    }

    private void ExecuteAction(DialogueActionData action)
    {
        if (action == null)
            return;

        switch (action.actionType)
        {
            case DialogueActionType.StartQuest:
                ExecuteStartQuest(action);
                break;

            case DialogueActionType.AdvanceQuest:
                ExecuteAdvanceQuest(action);
                break;

            case DialogueActionType.CompleteQuest:
                ExecuteCompleteQuest(action);
                break;

            case DialogueActionType.TurnInQuest:
                ExecuteTurnInQuest(action);
                break;

            case DialogueActionType.GiveItem:
                ExecuteGiveItem(action);
                break;

            case DialogueActionType.RemoveItem:
                ExecuteRemoveItem(action);
                break;

            case DialogueActionType.ShowNotification:
                ExecuteShowNotification(action);
                break;
        }
    }

    private void ExecuteStartQuest(DialogueActionData action)
    {
        if (questManager == null)
            return;

        string questId = DialogueDataNormalizer.NormalizeId(action.questId);

        if (string.IsNullOrWhiteSpace(questId))
            return;

        questManager.StartQuest(questId);
    }

    private void ExecuteAdvanceQuest(DialogueActionData action)
    {
        if (questManager == null)
            return;

        string questId = DialogueDataNormalizer.NormalizeId(action.questId);

        if (string.IsNullOrWhiteSpace(questId))
            return;

        questManager.CheckQuestProgress(questId);
    }

    private void ExecuteCompleteQuest(DialogueActionData action)
    {
        if (questManager == null)
            return;

        string questId = DialogueDataNormalizer.NormalizeId(action.questId);

        if (string.IsNullOrWhiteSpace(questId))
            return;

        questManager.CheckQuestProgress(questId);
    }

    private void ExecuteTurnInQuest(DialogueActionData action)
    {
        if (questManager == null)
            return;

        string questId = DialogueDataNormalizer.NormalizeId(action.questId);

        if (string.IsNullOrWhiteSpace(questId))
            return;

        questManager.TurnInQuest(questId);
    }

    private void ExecuteGiveItem(DialogueActionData action)
    {
        if (inventoryManager == null)
            return;

        string itemId = DialogueDataNormalizer.NormalizeId(action.itemId);

        if (string.IsNullOrWhiteSpace(itemId))
            return;

        string itemName = DialogueDataNormalizer.NormalizeText(action.itemName);

        if (string.IsNullOrWhiteSpace(itemName))
            itemName = itemId;

        inventoryManager.AddItem(itemId, itemName);
    }

    private void ExecuteRemoveItem(DialogueActionData action)
    {
        if (inventoryManager == null)
            return;

        string itemId = DialogueDataNormalizer.NormalizeId(action.itemId);

        if (string.IsNullOrWhiteSpace(itemId))
            return;

        inventoryManager.RemoveItem(itemId);
    }

    private void ExecuteShowNotification(DialogueActionData action)
    {
        if (notificationUI == null)
            return;

        string notificationText = DialogueDataNormalizer.NormalizeText(action.notificationText);

        if (string.IsNullOrWhiteSpace(notificationText))
            return;

        notificationUI.Show(notificationText);
    }
}