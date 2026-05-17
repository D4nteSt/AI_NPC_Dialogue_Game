using System.Collections.Generic;
using UnityEngine;

public class DialogueContextBuilder : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private InventoryManager inventoryManager;

    public DialogueContext BuildContext(
        NPCDialogueData npcData,
        string playerMessage,
        List<string> dialogueHistory,
        string lastOutcomeSummary = "")
    {
        DialogueContext context = new DialogueContext();

        if (npcData != null)
        {
            context.npcName = npcData.NPCName;
            context.npcRole = npcData.NPCRole;
            context.npcPersonality = npcData.NPCPersonality;
            context.npcSpeechStyle = npcData.NPCSpeechStyle;

            context.npcBackstory = npcData.NPCBackstory;
            context.npcLocationContext = npcData.NPCLocationContext;

            context.npcKnowledge = npcData.NPCKnowledge;
            context.npcUnknowns = npcData.NPCUnknowns;

            context.npcMotivation = npcData.NPCMotivation;
            context.npcSecret = npcData.NPCSecret;
            context.npcAttitudeToPlayer = npcData.NPCAttitudeToPlayer;

            context.npcCurrentEmotionalState = npcData.NPCCurrentEmotionalState;
            context.npcConversationTendency = npcData.NPCConversationTendency;

            context.isQuestGiver = npcData.IsQuestGiver;
            context.greetingMessage = npcData.GreetingMessage;

            QuestData relevantQuest = GetRelevantQuestData(npcData);

            if (relevantQuest != null)
            {
                context.questId = relevantQuest.questId;
                context.questName = relevantQuest.questName;
                context.questDescription = relevantQuest.description;

                if (questManager != null)
                {
                    QuestStatus status = GetEffectiveQuestStatus(relevantQuest);
                    context.questStatus = status.ToString();
                }
            }
        }

        context.playerMessage = playerMessage;

        if (inventoryManager != null)
        {
            context.inventoryItems = inventoryManager.GetItemNames();
        }

        if (dialogueHistory != null)
        {
            context.dialogueHistory = new List<string>(dialogueHistory);
        }

        context.lastOutcomeSummary = lastOutcomeSummary;

        return context;
    }

    private QuestData GetRelevantQuestData(NPCDialogueData npcData)
    {
        if (npcData == null)
            return null;

        List<QuestData> quests = npcData.GetAllQuestData();

        if (quests == null || quests.Count == 0)
            return null;

        QuestData completedQuest = FindFirstQuestWithStatus(quests, QuestStatus.Completed);
        if (completedQuest != null)
            return completedQuest;

        QuestData inProgressQuest = FindFirstQuestWithStatus(quests, QuestStatus.InProgress);
        if (inProgressQuest != null)
            return inProgressQuest;

        QuestData notStartedQuest = FindFirstQuestWithStatus(quests, QuestStatus.NotStarted);
        if (notStartedQuest != null)
            return notStartedQuest;

        QuestData turnedInQuest = FindFirstQuestWithStatus(quests, QuestStatus.TurnedIn);
        if (turnedInQuest != null)
            return turnedInQuest;

        return quests[0];
    }

    private QuestData FindFirstQuestWithStatus(List<QuestData> quests, QuestStatus requiredStatus)
    {
        if (quests == null || questManager == null)
            return null;

        foreach (QuestData questData in quests)
        {
            if (questData == null || string.IsNullOrWhiteSpace(questData.questId))
                continue;

            QuestStatus status = questManager.GetQuestStatus(questData.questId);

            if (status == requiredStatus)
                return questData;
        }

        return null;
    }

    private QuestStatus GetEffectiveQuestStatus(QuestData questData)
    {
        if (questData == null || string.IsNullOrWhiteSpace(questData.questId))
            return QuestStatus.NotStarted;

        QuestStatus status = questManager != null
            ? questManager.GetQuestStatus(questData.questId)
            : QuestStatus.NotStarted;

        bool playerHasRequiredItem =
            inventoryManager != null &&
            !string.IsNullOrWhiteSpace(questData.requiredItemId) &&
            inventoryManager.HasItem(questData.requiredItemId);

        if (playerHasRequiredItem)
        {
            if (status == QuestStatus.NotStarted || status == QuestStatus.InProgress)
                return QuestStatus.Completed;
        }

        return status;
    }
}