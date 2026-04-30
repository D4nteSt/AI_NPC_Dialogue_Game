using UnityEngine;

public class DialogueRuleResolver : MonoBehaviour, IDialogueRuleResolver
{
    [SerializeField] private DialogueRule[] rules;

    public DialogueOutcome Resolve(DialogueEvaluationContext context)
    {
        DialogueOutcome outcome = new DialogueOutcome();

        if (context == null || rules == null || rules.Length == 0)
            return outcome;

        foreach (var rule in rules)
        {
            if (!Matches(rule, context))
                continue;

            outcome.AddActions(rule.actions);
        }

        return outcome;
    }

    private bool Matches(DialogueRule rule, DialogueEvaluationContext context)
    {
        if (rule == null || context == null)
            return false;

        if (!string.IsNullOrWhiteSpace(rule.npcId))
        {
            if (!string.Equals(rule.npcId, context.NpcId, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"Rule [{rule.ruleId}] failed: npcId mismatch. Rule=[{rule.npcId}] Context=[{context.NpcId}]");
                return false;
            }
        }

        if (rule.requiredIntent != PlayerIntentType.None)
        {
            if (rule.requiredIntent != context.PlayerIntent)
            {
                Debug.Log($"Rule [{rule.ruleId}] failed: intent mismatch. Rule=[{rule.requiredIntent}] Context=[{context.PlayerIntent}]");
                return false;
            }
        }

        if (!string.IsNullOrWhiteSpace(rule.requiredQuestId))
        {
            if (!string.Equals(rule.requiredQuestId, context.QuestId, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"Rule [{rule.ruleId}] failed: questId mismatch. Rule=[{rule.requiredQuestId}] Context=[{context.QuestId}]");
                return false;
            }
        }

        if (rule.checkQuestStatus)
        {
            if (rule.requiredQuestStatus != context.QuestStatus)
            {
                Debug.Log($"Rule [{rule.ruleId}] failed: questStatus mismatch. Rule=[{rule.requiredQuestStatus}] Context=[{context.QuestStatus}]");
                return false;
            }
        }

        if (!string.IsNullOrWhiteSpace(rule.requiredInventoryItemId))
        {
            bool hasItem = context.InventoryItemIds.Contains(rule.requiredInventoryItemId);

            if (rule.requireItemPresent && !hasItem)
            {
                Debug.Log($"Rule [{rule.ruleId}] failed: required item missing. Rule=[{rule.requiredInventoryItemId}] Inventory=[{string.Join(", ", context.InventoryItemIds)}]");
                return false;
            }

            if (!rule.requireItemPresent && hasItem)
            {
                Debug.Log($"Rule [{rule.ruleId}] failed: item should be absent but is present. Rule=[{rule.requiredInventoryItemId}]");
                return false;
            }
        }

        Debug.Log($"Rule [{rule.ruleId}] matched successfully.");
        return true;
    }
}