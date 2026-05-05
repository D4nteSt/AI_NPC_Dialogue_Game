using System.Collections.Generic;
using UnityEngine;

public class QuestJournalUI : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private QuestEntryUI questEntryPrefab;
    [SerializeField] private GameObject emptyTextObject;

    [Header("Display Settings")]
    [SerializeField] private bool showNotStartedQuests = false;
    [SerializeField] private bool showTurnedInQuests = true;

    private readonly List<GameObject> spawnedEntries = new List<GameObject>();

    private void OnEnable()
    {
        if (questManager != null)
            questManager.QuestsChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (questManager != null)
            questManager.QuestsChanged -= Refresh;
    }

    public void Refresh()
    {
        ClearSpawnedEntries();

        if (questManager == null || contentRoot == null || questEntryPrefab == null)
            return;

        bool hasVisibleQuests = false;

        foreach (var pair in questManager.Quests)
        {
            string questId = pair.Key;
            QuestData questData = pair.Value;
            QuestStatus status = questManager.GetQuestStatus(questId);

            if (!ShouldShowQuest(status))
                continue;

            QuestEntryUI entryUI = Instantiate(questEntryPrefab, contentRoot);
            entryUI.SetData(questData, status);

            spawnedEntries.Add(entryUI.gameObject);
            hasVisibleQuests = true;
        }

        if (emptyTextObject != null)
            emptyTextObject.SetActive(!hasVisibleQuests);
    }

    private bool ShouldShowQuest(QuestStatus status)
    {
        if (status == QuestStatus.NotStarted && !showNotStartedQuests)
            return false;

        if (status == QuestStatus.TurnedIn && !showTurnedInQuests)
            return false;

        return true;
    }

    private void ClearSpawnedEntries()
    {
        for (int i = 0; i < spawnedEntries.Count; i++)
        {
            if (spawnedEntries[i] != null)
                Destroy(spawnedEntries[i]);
        }

        spawnedEntries.Clear();
    }
}