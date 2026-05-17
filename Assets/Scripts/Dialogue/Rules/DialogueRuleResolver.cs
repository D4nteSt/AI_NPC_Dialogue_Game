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
            {
                Debug.Log($"Rule [{rule.ruleId}] failed: npcId mismatch. Rule=[{ruleNpcId}] Context=[{contextNpcId}]");
                return false;
            }
        }

        if (rule.requiredIntent != PlayerIntentType.None)
        {
            bool hasIntent =
                context.PlayerIntent == rule.requiredIntent ||
                (context.PlayerIntents != null && context.PlayerIntents.Contains(rule.requiredIntent));

            if (!hasIntent)
            {
                Debug.Log($"Rule [{rule.ruleId}] failed: intent mismatch. Rule=[{rule.requiredIntent}] Context=[{context.PlayerIntent}]");
                return false;
            }
        }

        string ruleQuestId = DialogueDataNormalizer.NormalizeId(rule.requiredQuestId);

        if (!string.IsNullOrWhiteSpace(ruleQuestId))
        {
            if (context.QuestStatuses != null &&
                context.QuestStatuses.TryGetValue(ruleQuestId, out QuestStatus actualStatus))
            {
                if (rule.checkQuestStatus && rule.requiredQuestStatus != actualStatus)
                {
                    Debug.Log($"Rule [{rule.ruleId}] failed: questStatus mismatch. Rule=[{rule.requiredQuestStatus}] Context=[{actualStatus}] QuestId=[{ruleQuestId}]");
                    return false;
                }
            }
            else
            {
                Debug.Log($"Rule [{rule.ruleId}] failed: questId not found in context. Rule=[{ruleQuestId}]");
                return false;
            }
        }
        else
        {
            if (rule.checkQuestStatus)
            {
                if (rule.requiredQuestStatus != context.QuestStatus)
                {
                    Debug.Log($"Rule [{rule.ruleId}] failed: questStatus mismatch without questId. Rule=[{rule.requiredQuestStatus}] Context=[{context.QuestStatus}]");
                    return false;
                }
            }
        }

        string requiredItemId = DialogueDataNormalizer.NormalizeId(rule.requiredInventoryItemId);

        if (!string.IsNullOrWhiteSpace(requiredItemId))
        {
            bool hasItem = context.InventoryItemIds != null &&
                           context.InventoryItemIds.Contains(requiredItemId);

            if (rule.requireItemPresent && !hasItem)
            {
                Debug.Log($"Rule [{rule.ruleId}] failed: required item missing. Item=[{requiredItemId}]");
                return false;
            }

            if (!rule.requireItemPresent && hasItem)
            {
                Debug.Log($"Rule [{rule.ruleId}] failed: item should be absent. Item=[{requiredItemId}]");
                return false;
            }
        }

        Debug.Log("Rule [" + rule.ruleId + "] matched successfully.");
        return true;
    }
}