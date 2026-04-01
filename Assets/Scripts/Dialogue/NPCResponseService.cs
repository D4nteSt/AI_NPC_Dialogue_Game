using UnityEngine;

public class NPCResponseService : MonoBehaviour
{
    [SerializeField] private MonoBehaviour responseGeneratorBehaviour;

    private INPCResponseGenerator responseGenerator;

    private void Awake()
    {
        responseGenerator = responseGeneratorBehaviour as INPCResponseGenerator;

        if (responseGenerator == null)
        {
            Debug.LogError("Response Generator does not implement INPCResponseGenerator.");
        }
    }

    public string GetResponse(DialogueContext context)
    {
        if (responseGenerator == null)
            return "Ошибка: генератор ответа не настроен.";

        return responseGenerator.GenerateResponse(context);
    }
}