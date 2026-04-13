using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPAITextService : MonoBehaviour, IAITextService
{
    [SerializeField] private string backendUrl = "http://127.0.0.1:8000/npc-dialogue";

    public async Task<AIResponseData> GenerateTextAsync(AIRequestData requestData)
    {
        if (requestData == null || string.IsNullOrWhiteSpace(requestData.prompt))
        {
            return new AIResponseData
            {
                success = false,
                errorMessage = "Пустой запрос к backend."
            };
        }

        BackendNpcDialogueRequest backendRequest = new BackendNpcDialogueRequest
        {
            npcId = requestData.npcId,
            npcName = requestData.npcName,
            isQuestGiver = requestData.isQuestGiver,
            playerMessage = requestData.playerMessage,
            prompt = requestData.prompt,
            dialogueContext = ConvertContext(requestData.context),
            options = new BackendRequestOptions()
        };

        string json = JsonUtility.ToJson(backendRequest);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        Debug.Log("HTTPAITextService: preparing request to backend...");
        Debug.Log("HTTPAITextService URL: " + backendUrl);
        Debug.Log("HTTPAITextService JSON: " + json);

        using UnityWebRequest webRequest = new UnityWebRequest(backendUrl, "POST");
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        var operation = webRequest.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("HTTPAITextService error: " + webRequest.error);

            return new AIResponseData
            {
                success = false,
                errorMessage = "Ошибка HTTP: " + webRequest.error
            };
        }

        string responseJson = webRequest.downloadHandler.text;
        Debug.Log("HTTPAITextService raw response: " + responseJson);

        if (string.IsNullOrWhiteSpace(responseJson))
        {
            return new AIResponseData
            {
                success = false,
                errorMessage = "Backend вернул пустой ответ."
            };
        }

        BackendNpcDialogueResponse backendResponse =
            JsonUtility.FromJson<BackendNpcDialogueResponse>(responseJson);

        if (backendResponse == null)
        {
            return new AIResponseData
            {
                success = false,
                errorMessage = "Не удалось распарсить ответ backend."
            };
        }

        return new AIResponseData
        {
            success = backendResponse.success,
            responseText = backendResponse.replyText,
            errorMessage = backendResponse.errorMessage
        };
    }

    private BackendDialogueContextData ConvertContext(DialogueContext context)
    {
        if (context == null)
            return new BackendDialogueContextData();

        return new BackendDialogueContextData
        {
            npcRole = context.npcRole,
            npcPersonality = context.npcPersonality,
            npcSpeechStyle = context.npcSpeechStyle,
            npcBackstory = context.npcBackstory,
            npcLocationContext = context.npcLocationContext,
            npcKnowledge = context.npcKnowledge,
            npcUnknowns = context.npcUnknowns,
            npcMotivation = context.npcMotivation,
            npcSecret = context.npcSecret,
            npcAttitudeToPlayer = context.npcAttitudeToPlayer,
            npcCurrentEmotionalState = context.npcCurrentEmotionalState,
            npcConversationTendency = context.npcConversationTendency,
            questId = context.questId,
            questName = context.questName,
            questDescription = context.questDescription,
            questStatus = context.questStatus,
            inventoryItems = context.inventoryItems != null
                ? new System.Collections.Generic.List<string>(context.inventoryItems)
                : new System.Collections.Generic.List<string>(),
            dialogueHistory = FilterDialogueHistory(context.dialogueHistory)
        };
    }

    private System.Collections.Generic.List<string> FilterDialogueHistory(
    System.Collections.Generic.List<string> history)
    {
        var result = new System.Collections.Generic.List<string>();

        if (history == null || history.Count == 0)
            return result;

        foreach (string line in history)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string trimmed = line.Trim();

            if (trimmed == "...")
                continue;

            if (trimmed.Contains("Ошибка AI:"))
                continue;

            if (trimmed.Contains("Ошибка HTTP:"))
                continue;

            if (trimmed.StartsWith("Система:"))
                continue;

            result.Add(trimmed);
        }

        return result;
    }
}