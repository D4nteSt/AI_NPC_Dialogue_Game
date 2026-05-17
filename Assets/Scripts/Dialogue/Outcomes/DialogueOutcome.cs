using System.Collections.Generic;

public class DialogueOutcome
{
    public List<DialogueActionData> Actions { get; } = new();

    public bool HasActions => Actions.Count > 0;

    public string npcResponseHint;

    public void AddAction(DialogueActionData action)
    {
        if (action == null)
            return;

        Actions.Add(action);
    }

    public void AddActions(IEnumerable<DialogueActionData> actions)
    {
        if (actions == null)
            return;

        foreach (var action in actions)
        {
            AddAction(action);
        }
    }
}