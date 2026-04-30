using System.Collections.Generic;

public class DialogueEvaluationContext
{
    public string NpcId;
    public string NpcName;

    public PlayerIntentType PlayerIntent = PlayerIntentType.None;
    public string PlayerMessage;

    public string QuestId;
    public QuestStatus QuestStatus;

    public HashSet<string> InventoryItemIds = new();
}