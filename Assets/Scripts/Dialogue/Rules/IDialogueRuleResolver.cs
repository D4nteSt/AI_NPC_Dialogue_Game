public interface IDialogueRuleResolver
{
    DialogueOutcome Resolve(DialogueEvaluationContext context);
}