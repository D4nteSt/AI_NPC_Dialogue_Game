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
        if (questManager == null || string.IsNullOrWhiteSpace(action.questId))
            return;

        questManager.StartQuest(action.questId);
    }

    private void ExecuteAdvanceQuest(DialogueActionData action)
    {
        if (questManager == null || string.IsNullOrWhiteSpace(action.questId))
            return;

        questManager.CheckQuestProgress(action.questId);
    }

    private void ExecuteCompleteQuest(DialogueActionData action)
    {
        if (questManager == null || string.IsNullOrWhiteSpace(action.questId))
            return;

        questManager.CheckQuestProgress(action.questId);
    }

    private void ExecuteTurnInQuest(DialogueActionData action)
    {
        if (questManager == null || string.IsNullOrWhiteSpace(action.questId))
            return;

        questManager.TurnInQuest(action.questId);
    }

    private void ExecuteGiveItem(DialogueActionData action)
    {
        if (inventoryManager == null || string.IsNullOrWhiteSpace(action.itemId))
            return;

        string itemName = string.IsNullOrWhiteSpace(action.itemName)
            ? action.itemId
            : action.itemName;

        inventoryManager.AddItem(action.itemId, itemName);
    }

    private void ExecuteRemoveItem(DialogueActionData action)
    {
        if (inventoryManager == null || string.IsNullOrWhiteSpace(action.itemId))
            return;

        inventoryManager.RemoveItem(action.itemId);
    }

    private void ExecuteShowNotification(DialogueActionData action)
    {
        if (notificationUI == null || string.IsNullOrWhiteSpace(action.notificationText))
            return;

        notificationUI.Show(action.notificationText);
    }
}