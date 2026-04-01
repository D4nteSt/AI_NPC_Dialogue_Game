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
            context.npcPersonality = npcData.NPCPersonality;
            context.greetingMessage = npcData.GreetingMessage;

            if (npcData.QuestData != null)
            {
                context.questId = npcData.QuestData.questId;
                context.questName = npcData.QuestData.questName;
                context.questDescription = npcData.QuestData.description;

                if (questManager != null)
                {
                    context.questStatus = questManager
                        .GetQuestStatus(npcData.QuestData.questId)
                        .ToString();
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