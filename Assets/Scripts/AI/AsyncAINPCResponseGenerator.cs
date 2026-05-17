using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Audio.ProcessorInstance;

public class AsyncAINPCResponseGenerator : MonoBehaviour, IAsyncNPCResponseGenerator
{
    [SerializeField] private NPCPromptBuilder promptBuilder;
    [SerializeField] private MonoBehaviour aiTextServiceBehaviour;
    [SerializeField] private bool logPromptToConsole = true;

    private IAITextService aiTextService;

    private void Awake()
    {
        aiTextService = aiTextServiceBehaviour as IAITextService;

        if (aiTextService == null)
        {
            Debug.LogError("AI Text Service does not implement IAITextService.");
        }
    }

    public async Task<string> GenerateResponseAsync(DialogueContext context)
    {
        if (promptBuilder == null)
            return "Ошибка: PromptBuilder не назначен.";

        if (aiTextService == null)
            return "Ошибка: AI Text Service не настроен.";

        string debugPrompt = promptBuilder.BuildPrompt(context, PromptMode.Debug);
        string compactPrompt = promptBuilder.BuildPrompt(context, PromptMode.Compact);

        if (logPromptToConsole)
        {
            Debug.Log("=== AI DEBUG PROMPT START ===\n" + debugPrompt + "\n=== AI DEBUG PROMPT END ===");
            Debug.Log("=== AI COMPACT PROMPT START ===\n" + compactPrompt + "\n=== AI COMPACT PROMPT END ===");
        }

        AIRequestData request = new AIRequestData
        {
            prompt = compactPrompt,
            context = context,
            npcId = BuildNpcId(context),
            npcName = context != null ? context.npcName : string.Empty,
            isQuestGiver = context != null && context.isQuestGiver,
            playerMessage = context != null ? context.playerMessage : string.Empty
        };

        AIResponseData response = await aiTextService.GenerateTextAsync(request);

        if (response == null)
            return "Ошибка AI: AI-сервис вернул пустой ответ.";

        if (!response.success)
            return "Ошибка AI: " + response.errorMessage;

        if (string.IsNullOrWhiteSpace(response.responseText))
            return "Ошибка AI: AI не смог сформировать ответ.";

        return CleanNpcResponse(response.responseText);

    }

    private string BuildNpcId(DialogueContext context)
    {
        if (context == null || string.IsNullOrWhiteSpace(context.npcName))
            return "unknown_npc";

        return context.npcName.Trim().ToLowerInvariant().Replace(" ", "_");
    }

    private string CleanNpcResponse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        string result = text.Trim();

        if ((result.StartsWith("«") && result.EndsWith("»")) ||
            (result.StartsWith("\"") && result.EndsWith("\"")))
        {
            result = result.Substring(1, result.Length - 2).Trim();
        }

        return result;
    }
}