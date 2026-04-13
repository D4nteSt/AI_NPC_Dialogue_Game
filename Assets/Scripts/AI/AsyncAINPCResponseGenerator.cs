using System.Threading.Tasks;
using UnityEngine;

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

        string prompt = promptBuilder.BuildPrompt(context);

        if (logPromptToConsole)
        {
            Debug.Log("=== AI PROMPT START ===\n" + prompt + "\n=== AI PROMPT END ===");
        }

        AIRequestData request = new AIRequestData
        {
            prompt = prompt,
            context = context,
            npcId = BuildNpcId(context),
            npcName = context != null ? context.npcName : string.Empty,
            isQuestGiver = context != null && context.isQuestGiver,
            playerMessage = context != null ? context.playerMessage : string.Empty
        };

        AIResponseData response = await aiTextService.GenerateTextAsync(request);

        if (response == null)
            return "Ошибка: AI-сервис вернул пустой ответ.";

        if (!response.success)
            return "Не удалось получить ответ NPC. " + response.errorMessage;

        if (string.IsNullOrWhiteSpace(response.responseText))
            return "AI не смог сформировать ответ.";

        return response.responseText;
    }

    private string BuildNpcId(DialogueContext context)
    {
        if (context == null || string.IsNullOrWhiteSpace(context.npcName))
            return "unknown_npc";

        return context.npcName.Trim().ToLowerInvariant().Replace(" ", "_");
    }
}