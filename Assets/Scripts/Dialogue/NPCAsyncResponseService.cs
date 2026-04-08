using System.Threading.Tasks;
using UnityEngine;

public class NPCAsyncResponseService : MonoBehaviour
{
    [SerializeField] private MonoBehaviour responseGeneratorBehaviour;

    private IAsyncNPCResponseGenerator responseGenerator;

    private void Awake()
    {
        responseGenerator = responseGeneratorBehaviour as IAsyncNPCResponseGenerator;

        if (responseGenerator == null)
        {
            Debug.LogError("Response Generator does not implement IAsyncNPCResponseGenerator.");
        }
    }

    public async Task<string> GetResponseAsync(DialogueContext context)
    {
        if (responseGenerator == null)
            return "Ошибка: асинхронный генератор ответа не настроен.";

        return await responseGenerator.GenerateResponseAsync(context);
    }
}