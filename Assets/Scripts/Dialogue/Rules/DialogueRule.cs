using System;

[Serializable]
public class DialogueRule
{
    public string ruleId;
    public string npcId;

    public PlayerIntentType requiredIntent = PlayerIntentType.None;

    public string requiredQuestId;
    public bool checkQuestStatus;
    public QuestStatus requiredQuestStatus;

    public string requiredInventoryItemId;
    public bool requireItemPresent;


    public DialogueActionData[] actions;
}