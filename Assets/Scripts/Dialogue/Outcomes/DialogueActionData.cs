using UnityEngine;

[System.Serializable]
public class DialogueActionData
{
    public DialogueActionType actionType;

    [Header("Quest")]
    public string questId;

    [Header("Item")]
    public string itemId;
    public string itemName;
    public int itemCount = 1;

    [Header("Notification")]
    [TextArea(2, 4)]
    public string notificationText;

    [Header("NPC Response Hint")]
    [TextArea(2, 5)]
    public string responseHint;
}