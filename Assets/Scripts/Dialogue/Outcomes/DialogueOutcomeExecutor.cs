using UnityEngine;

public class DialogueOutcomeExecutor : MonoBehaviour, IDialogueOutcomeExecutor
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private NotificationUI notificationUI;
    [SerializeField] private PoliceGateController policeGateController;

    public DialogueOutcomeExecutionResult Execute(DialogueOutcome outcome)
    {
        DialogueOutcomeExecutionResult result = new DialogueOutcomeExecutionResult();

        if (outcome == null || !outcome.HasActions)
            return result;

        foreach (DialogueActionData action in outcome.Actions)
        {
            ExecuteAction(action, result);
        }

        return result;
    }

    private void ExecuteAction(DialogueActionData action, DialogueOutcomeExecutionResult result)
    {
        if (action == null)
            return;

        switch (action.actionType)
        {
            case DialogueActionType.StartQuest:
                ExecuteStartQuest(action, result);
                break;

            case DialogueActionType.AdvanceQuest:
                ExecuteAdvanceQuest(action, result);
                break;

            case DialogueActionType.CompleteQuest:
                ExecuteCompleteQuest(action, result);
                break;

            case DialogueActionType.TurnInQuest:
                ExecuteTurnInQuest(action, result);
                break;

            case DialogueActionType.GiveItem:
                ExecuteGiveItem(action, result);
                break;

            case DialogueActionType.RemoveItem:
                ExecuteRemoveItem(action, result);
                break;

            case DialogueActionType.ShowNotification:
                ExecuteShowNotification(action);
                break;

            case DialogueActionType.OpenGate:
                ExecuteOpenGate(action, result);
                break;

            default:
                Debug.LogWarning("Unknown dialogue action type: " + action.actionType);
                break;
        }
    }

    private void ExecuteStartQuest(DialogueActionData action, DialogueOutcomeExecutionResult result)
    {
        if (questManager == null)
        {
            Debug.LogWarning("ExecuteStartQuest failed: questManager is null.");
            return;
        }

        string questId = DialogueDataNormalizer.NormalizeId(action.questId);

        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogWarning("ExecuteStartQuest failed: questId is empty.");
            return;
        }

        questManager.StartQuest(questId);

        AddActionSummary(
            result,
            action,
            "Текущий персонаж начал для игрока новый этап поручения."
        );
    }

    private void ExecuteAdvanceQuest(DialogueActionData action, DialogueOutcomeExecutionResult result)
    {
        if (questManager == null)
        {
            Debug.LogWarning("ExecuteAdvanceQuest failed: questManager is null.");
            return;
        }

        string questId = DialogueDataNormalizer.NormalizeId(action.questId);

        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogWarning("ExecuteAdvanceQuest failed: questId is empty.");
            return;
        }

        questManager.CheckQuestProgress(questId);

        AddActionSummary(
            result,
            action,
            "Состояние текущего поручения было проверено или продвинуто."
        );
    }

    private void ExecuteCompleteQuest(DialogueActionData action, DialogueOutcomeExecutionResult result)
    {
        if (questManager == null)
        {
            Debug.LogWarning("ExecuteCompleteQuest failed: questManager is null.");
            return;
        }

        string questId = DialogueDataNormalizer.NormalizeId(action.questId);

        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogWarning("ExecuteCompleteQuest failed: questId is empty.");
            return;
        }

        questManager.CheckQuestProgress(questId);

        AddActionSummary(
            result,
            action,
            "Условие текущего этапа поручения выполнено."
        );
    }

    private void ExecuteTurnInQuest(DialogueActionData action, DialogueOutcomeExecutionResult result)
    {
        if (questManager == null)
        {
            Debug.LogWarning("ExecuteTurnInQuest failed: questManager is null.");
            return;
        }

        string questId = DialogueDataNormalizer.NormalizeId(action.questId);

        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogWarning("ExecuteTurnInQuest failed: questId is empty.");
            return;
        }

        questManager.TurnInQuest(questId);

        AddActionSummary(
            result,
            action,
            "Игрок передал текущему персонажу результат этапа поручения."
        );
    }

    private void ExecuteGiveItem(DialogueActionData action, DialogueOutcomeExecutionResult result)
    {
        if (inventoryManager == null)
        {
            Debug.LogWarning("ExecuteGiveItem failed: inventoryManager is null.");
            return;
        }

        string itemId = DialogueDataNormalizer.NormalizeId(action.itemId);

        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning("ExecuteGiveItem failed: itemId is empty.");
            return;
        }

        string itemName = DialogueDataNormalizer.NormalizeText(action.itemName);

        if (string.IsNullOrWhiteSpace(itemName))
            itemName = itemId;

        inventoryManager.AddItem(itemId, itemName);

        AddActionSummary(
            result,
            action,
            "Текущий персонаж выдал игроку предмет: " + itemName + "."
        );
    }

    private void ExecuteRemoveItem(DialogueActionData action, DialogueOutcomeExecutionResult result)
    {
        if (inventoryManager == null)
        {
            Debug.LogWarning("ExecuteRemoveItem failed: inventoryManager is null.");
            return;
        }

        string itemId = DialogueDataNormalizer.NormalizeId(action.itemId);

        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning("ExecuteRemoveItem failed: itemId is empty.");
            return;
        }

        string itemDisplayName = GetInventoryItemDisplayName(itemId);

        inventoryManager.RemoveItem(itemId);

        if (string.IsNullOrWhiteSpace(itemDisplayName))
            itemDisplayName = itemId;

        AddActionSummary(
            result,
            action,
            "Игрок передал или потерял предмет: " + itemDisplayName + "."
        );
    }

    private void ExecuteShowNotification(DialogueActionData action)
    {
        if (notificationUI == null)
        {
            Debug.LogWarning("ExecuteShowNotification failed: notificationUI is null.");
            return;
        }

        string notificationText = DialogueDataNormalizer.NormalizeText(action.notificationText);

        if (string.IsNullOrWhiteSpace(notificationText))
            return;

        notificationUI.Show(notificationText);
    }

    private void AddActionSummary(
        DialogueOutcomeExecutionResult result,
        DialogueActionData action,
        string defaultSummary)
    {
        if (result == null)
            return;

        string responseHint = DialogueDataNormalizer.NormalizeText(action.responseHint);

        if (!string.IsNullOrWhiteSpace(responseHint))
        {
            result.AddSummary(responseHint);
            return;
        }

        result.AddSummary(defaultSummary);
    }

    private string GetInventoryItemDisplayName(string itemId)
    {
        if (inventoryManager == null || inventoryManager.Items == null)
            return string.Empty;

        if (inventoryManager.Items.TryGetValue(itemId, out string itemName))
            return itemName;

        return string.Empty;
    }
    private void ExecuteOpenGate(DialogueActionData action, DialogueOutcomeExecutionResult result)
    {
        if (policeGateController == null)
        {
            Debug.LogWarning("ExecuteOpenGate failed: policeGateController is null.");
            return;
        }

        policeGateController.OpenGate();

        AddActionSummary(
            result,
            action,
            "Полицейский проверил ордер и открыл проход к месту осмотра."
        );
    }
}