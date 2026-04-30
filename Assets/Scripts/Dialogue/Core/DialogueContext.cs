using System.Collections.Generic;

[System.Serializable]
public class DialogueContext
{
    public string npcName;
    public string npcRole;
    public string npcPersonality;
    public string npcSpeechStyle;

    public string npcBackstory;
    public string npcLocationContext;

    public string npcKnowledge;
    public string npcUnknowns;

    public string npcMotivation;
    public string npcSecret;
    public string npcAttitudeToPlayer;

    public string npcCurrentEmotionalState;
    public string npcConversationTendency;

    public bool isQuestGiver;

    public string greetingMessage;
    public string playerMessage;

    public string questId;
    public string questName;
    public string questDescription;
    public string questStatus;

    public List<string> inventoryItems = new List<string>();
    public List<string> dialogueHistory = new List<string>();
}