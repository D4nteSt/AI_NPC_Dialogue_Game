using System.Threading.Tasks;

public interface IAITextService
{
    Task<AIResponseData> GenerateTextAsync(AIRequestData requestData);
}