using UnityEngine;

public class AINPCResponseGenerator : MonoBehaviour, INPCResponseGenerator
{
    [SerializeField] private NPCPromptBuilder promptBuilder;
    [SerializeField] private bool logPromptToConsole = true;

    public string GenerateResponse(DialogueContext context)
    {
        if (promptBuilder == null)
            return "Ошибка: PromptBuilder не назначен.";

        string prompt = promptBuilder.BuildPrompt(context);

        if (logPromptToConsole)
        {
            Debug.Log("=== AI PROMPT START ===\n" + prompt + "\n=== AI PROMPT END ===");
        }

        return BuildPseudoAIResponse(context);
    }

    private string BuildPseudoAIResponse(DialogueContext context)
    {
        if (context == null)
            return "Мне трудно собраться с мыслями.";

        string status = context.questStatus ?? string.Empty;
        string personality = context.npcPersonality ?? string.Empty;

        string prefix = "Персонаж отвечает спокойно.";

        string normalizedPersonality = personality.ToLowerInvariant();

        if (normalizedPersonality.Contains("мудр"))
            prefix = "Старик говорит размеренно и вдумчиво.";
        else if (normalizedPersonality.Contains("строг"))
            prefix = "Старик отвечает сухо и строго.";
        else if (normalizedPersonality.Contains("добр"))
            prefix = "Старик отвечает мягко и доброжелательно.";
        else if (normalizedPersonality.Contains("загадоч"))
            prefix = "Старик отвечает тихо и уклончиво.";

        switch (status)
        {
            case "NotStarted":
                return prefix + " Если ты готов помочь мне, отыщи " + SafeQuestName(context) + ".";

            case "InProgress":
                return prefix + " Я все еще жду " + SafeQuestName(context) + ". Постарайся найти его.";

            case "Completed":
                return prefix + " Да, теперь я вижу, что дело почти завершено.";

            case "TurnedIn":
                return prefix + " Ты уже помог мне, и я это помню.";

            default:
                return prefix + " Я слушаю тебя.";
        }
    }

    private string SafeQuestName(DialogueContext context)
    {
        return string.IsNullOrWhiteSpace(context.questName) ? "нужный предмет" : context.questName;
    }
}