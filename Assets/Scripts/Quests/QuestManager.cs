using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, QuestStatus> questStatuses = new Dictionary<string, QuestStatus>();
    private Dictionary<string, QuestData> quests = new Dictionary<string, QuestData>();

    [SerializeField] private InventoryManager inventoryManager;

    public IReadOnlyDictionary<string, QuestData> Quests => quests;
    public IReadOnlyDictionary<string, QuestStatus> QuestStatuses => questStatuses;

    public event Action QuestsChanged;

    public void RegisterQuest(QuestData questData)
    {
        if (questData == null || string.IsNullOrWhiteSpace(questData.questId))
        {
            Debug.LogWarning("RegisterQuest failed: questData is null or questId is empty.");
            return;
        }

        string questId = DialogueDataNormalizer.NormalizeId(questData.questId);
        questData.questId = questId;

        if (!quests.ContainsKey(questId))
        {
            quests.Add(questId, questData);
            questStatuses.Add(questId, QuestStatus.NotStarted);
            Debug.Log("Квест зарегистрирован: " + questId);
            QuestsChanged?.Invoke();
        }
    }

    public QuestStatus GetQuestStatus(string questId)
    {
        questId = DialogueDataNormalizer.NormalizeId(questId);

        if (questStatuses.TryGetValue(questId, out QuestStatus status))
            return status;

        return QuestStatus.NotStarted;
    }

    public void StartQuest(string questId)
    {
        questId = DialogueDataNormalizer.NormalizeId(questId);

        if (string.IsNullOrWhiteSpace(questId))
        {
            Debug.LogWarning("StartQuest failed: questId is empty.");
            return;
        }

        if (!questStatuses.ContainsKey(questId))
        {
            Debug.LogWarning("StartQuest failed: quest is not registered: " + questId);
            return;
        }

        if (questStatuses[questId] == QuestStatus.NotStarted)
        {
            questStatuses[questId] = QuestStatus.InProgress;
            Debug.Log("Квест начат: " + questId);
            QuestsChanged?.Invoke();
        }
        else
        {
            Debug.Log("StartQuest skipped. Current status: " + questStatuses[questId]);
        }
    }

    public void CheckQuestProgress(string questId)
    {
        questId = DialogueDataNormalizer.NormalizeId(questId);

        if (!quests.ContainsKey(questId) || !questStatuses.ContainsKey(questId))
            return;

        QuestData quest = quests[questId];

        string requiredItemId = DialogueDataNormalizer.NormalizeId(quest.requiredItemId);

        if (questStatuses[questId] == QuestStatus.InProgress &&
            inventoryManager != null &&
            inventoryManager.HasItem(requiredItemId))
        {
            questStatuses[questId] = QuestStatus.Completed;
            Debug.Log("Квест выполнен: " + questId);
            QuestsChanged?.Invoke();
        }
    }

    public void TurnInQuest(string questId)
    {
        questId = DialogueDataNormalizer.NormalizeId(questId);

        if (!quests.ContainsKey(questId) || !questStatuses.ContainsKey(questId))
            return;

        QuestData quest = quests[questId];

        string requiredItemId = DialogueDataNormalizer.NormalizeId(quest.requiredItemId);

        if (questStatuses[questId] == QuestStatus.Completed)
        {
            if (inventoryManager != null && inventoryManager.HasItem(requiredItemId))
            {
                inventoryManager.RemoveItem(requiredItemId);
            }

            questStatuses[questId] = QuestStatus.TurnedIn;
            Debug.Log("Квест сдан: " + questId);
            QuestsChanged?.Invoke();
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