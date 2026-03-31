using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, QuestStatus> questStatuses = new Dictionary<string, QuestStatus>();
    private Dictionary<string, QuestData> quests = new Dictionary<string, QuestData>();

    [SerializeField] private InventoryManager inventoryManager;

    public IReadOnlyDictionary<string, QuestData> Quests => quests;
    public IReadOnlyDictionary<string, QuestStatus> QuestStatuses => questStatuses;

    public void RegisterQuest(QuestData questData)
    {
        if (questData == null || string.IsNullOrWhiteSpace(questData.questId))
            return;

        if (!quests.ContainsKey(questData.questId))
        {
            quests.Add(questData.questId, questData);
            questStatuses.Add(questData.questId, QuestStatus.NotStarted);
        }
    }

    public QuestStatus GetQuestStatus(string questId)
    {
        if (questStatuses.TryGetValue(questId, out QuestStatus status))
            return status;

        return QuestStatus.NotStarted;
    }

    public void StartQuest(string questId)
    {
        if (!questStatuses.ContainsKey(questId))
            return;

        if (questStatuses[questId] == QuestStatus.NotStarted)
        {
            questStatuses[questId] = QuestStatus.InProgress;
            Debug.Log("Квест начат: " + questId);
        }
    }

    public void CheckQuestProgress(string questId)
    {
        if (!quests.ContainsKey(questId) || !questStatuses.ContainsKey(questId))
            return;

        QuestData quest = quests[questId];

        if (questStatuses[questId] == QuestStatus.InProgress &&
            inventoryManager != null &&
            inventoryManager.HasItem(quest.requiredItemId))
        {
            questStatuses[questId] = QuestStatus.Completed;
            Debug.Log("Квест выполнен: " + questId);
        }
    }

    public void TurnInQuest(string questId)
    {
        if (!quests.ContainsKey(questId) || !questStatuses.ContainsKey(questId))
            return;

        QuestData quest = quests[questId];

        if (questStatuses[questId] == QuestStatus.Completed)
        {
            if (inventoryManager != null && inventoryManager.HasItem(quest.requiredItemId))
            {
                inventoryManager.RemoveItem(quest.requiredItemId);
            }

            questStatuses[questId] = QuestStatus.TurnedIn;
            Debug.Log("Квест сдан: " + questId);
        }
    }

    public List<string> GetQuestDescriptions()
    {
        List<string> result = new List<string>();

        foreach (var pair in quests)
        {
            string questId = pair.Key;
            QuestData quest = pair.Value;
            QuestStatus status = questStatuses[questId];

            string statusText = GetStatusText(status);
            result.Add("[" + statusText + "] " + quest.questName + " — " + quest.description);
        }

        return result;
    }

    private string GetStatusText(QuestStatus status)
    {
        switch (status)
        {
            case QuestStatus.NotStarted:
                return "Не начат";
            case QuestStatus.InProgress:
                return "В процессе";
            case QuestStatus.Completed:
                return "Выполнен";
            case QuestStatus.TurnedIn:
                return "Сдан";
            default:
                return "Неизвестно";
        }
    }
}