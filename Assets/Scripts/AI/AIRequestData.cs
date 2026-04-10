[System.Serializable]
public class AIRequestData
{
    public string prompt;
    public DialogueContext context;
    public string npcId;
    public string npcName;
    public bool isQuestGiver;
    public string playerMessage;
}