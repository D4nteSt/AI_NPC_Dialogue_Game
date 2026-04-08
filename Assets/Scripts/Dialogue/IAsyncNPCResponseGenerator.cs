using System.Threading.Tasks;

public interface IAsyncNPCResponseGenerator
{
    Task<string> GenerateResponseAsync(DialogueContext context);
}