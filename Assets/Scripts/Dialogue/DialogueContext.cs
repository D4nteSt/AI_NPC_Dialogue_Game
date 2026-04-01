using System.Collections.Generic;

[System.Serializable]
public class DialogueContext
{
    public string npcName;
    public string npcPersonality;
    public string greetingMessage;

    public string playerMessage;

    public string questId;
    public string questName;
    public string questDescription;
    public string questStatus;

    public List<string> inventoryItems = new List<string>();
    public List<string> dialogueHistory = new List<string>();
}
