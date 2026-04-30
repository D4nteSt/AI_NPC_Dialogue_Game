using System.Collections.Generic;
using UnityEngine;

public class DialogueContextBuilder : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private InventoryManager inventoryManager;

    public DialogueContext BuildContext(
        NPCDialogueData npcData,
        string playerMessage,
        List<string> dialogueHistory)
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

            if (npcData.QuestData != null)
            {
                context.questId = npcData.QuestData.questId;
                context.questName = npcData.QuestData.questName;
                context.questDescription = npcData.QuestData.description;

                if (questManager != null)
                {
                    QuestStatus status = questManager.GetQuestStatus(npcData.QuestData.questId);

                    bool playerHasRequiredItem =
                        inventoryManager != null &&
                        !string.IsNullOrWhiteSpace(npcData.QuestData.requiredItemId) &&
                        inventoryManager.HasItem(npcData.QuestData.requiredItemId);

                    if (playerHasRequiredItem)
                    {
                        if (status == QuestStatus.NotStarted || status == QuestStatus.InProgress)
                        {
                            status = QuestStatus.Completed;
                        }
                    }

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

        return context;
    }
}