using System;
using System.Collections.Generic;

[Serializable]
public class BackendNpcDialogueRequest
{
    public string npcId;
    public string npcName;
    public bool isQuestGiver;
    public string playerMessage;

    public BackendDialogueContextData dialogueContext;
    public string prompt;
    public BackendRequestOptions options;
}

[Serializable]
public class BackendDialogueContextData
{
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

    public string questId;
    public string questName;
    public string questDescription;
    public string questStatus;

    public List<string> inventoryItems = new List<string>();
    public List<string> dialogueHistory = new List<string>();
}

[Serializable]
public class BackendRequestOptions
{
    public string provider = "mock";
    public string model = "test-model";
    public float temperature = 0.45f;
    public int maxOutputTokens = 96;
    public bool debug = true;
}