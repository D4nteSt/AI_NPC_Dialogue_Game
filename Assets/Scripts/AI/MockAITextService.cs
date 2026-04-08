using System.Threading.Tasks;
using UnityEngine;

public class MockAITextService : MonoBehaviour, IAITextService
{
    [SerializeField] private float simulatedDelaySeconds = 1.2f;

    public async Task<AIResponseData> GenerateTextAsync(AIRequestData requestData)
    {
        int delayMs = Mathf.RoundToInt(simulatedDelaySeconds * 1000f);
        await Task.Delay(delayMs);

        if (requestData == null || string.IsNullOrWhiteSpace(requestData.prompt))
        {
            return new AIResponseData
            {
                success = false,
                errorMessage = "Пустой prompt."
            };
        }

        return new AIResponseData
        {
            success = true,
            responseText = "Это тестовый ответ AI-сервиса. Prompt получен и обработан."
        };
    }
}