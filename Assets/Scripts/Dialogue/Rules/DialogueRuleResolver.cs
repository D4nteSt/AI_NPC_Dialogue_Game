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

        string ruleNpcId = DialogueDataNormalizer.NormalizeId(rule.npcId);
        string contextNpcId = DialogueDataNormalizer.NormalizeId(context.NpcId);

        if (!string.IsNullOrWhiteSpace(ruleNpcId))
        {
            if (ruleNpcId != contextNpcId)
                return false;
        }

        if (rule.requiredIntent != PlayerIntentType.None)
        {
            if (rule.requiredIntent != context.PlayerIntent)
                return false;
        }

        string ruleQuestId = DialogueDataNormalizer.NormalizeId(rule.requiredQuestId);
        string contextQuestId = DialogueDataNormalizer.NormalizeId(context.QuestId);

        if (!string.IsNullOrWhiteSpace(ruleQuestId))
        {
            if (ruleQuestId != contextQuestId)
                return false;
        }

        if (rule.checkQuestStatus)
        {
            if (rule.requiredQuestStatus != context.QuestStatus)
                return false;
        }

        string requiredItemId = DialogueDataNormalizer.NormalizeId(rule.requiredInventoryItemId);

        if (!string.IsNullOrWhiteSpace(requiredItemId))
        {
            bool hasItem = context.InventoryItemIds.Contains(requiredItemId);

            if (rule.requireItemPresent && !hasItem)
                return false;

            if (!rule.requireItemPresent && hasItem)
                return false;
        }

        Debug.Log("Rule [" + rule.ruleId + "] matched successfully.");
        return true;
    }
}