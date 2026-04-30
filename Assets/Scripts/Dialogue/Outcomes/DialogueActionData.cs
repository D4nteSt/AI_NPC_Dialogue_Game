using System;

[Serializable]
public class DialogueActionData
{
    public DialogueActionType actionType = DialogueActionType.None;

    public string questId;

    public string itemId;
    public string itemName;
    public int itemCount = 1;

    public string notificationText;
}