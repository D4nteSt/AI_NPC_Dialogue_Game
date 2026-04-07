using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NPCPromptBuilder : MonoBehaviour
{
    [SerializeField] private int maxHistoryLines = 6;

    public string BuildPrompt(DialogueContext context)
    {
        if (context == null)
            return "Контекст отсутствует.";

        StringBuilder prompt = new StringBuilder();

        AppendRoleBlock(prompt, context);
        AppendSituationBlock(prompt, context);
        AppendInventoryBlock(prompt, context);
        AppendDialogueHistoryBlock(prompt, context);
        AppendResponseRulesBlock(prompt);

        return prompt.ToString();
    }

    private void AppendRoleBlock(StringBuilder prompt, DialogueContext context)
    {
        prompt.AppendLine("Ты — неигровой персонаж в сюжетной игре на Unity.");
        prompt.AppendLine();

        if (!string.IsNullOrWhiteSpace(context.npcName))
        {
            prompt.AppendLine("Твое имя: " + context.npcName + ".");
        }

        if (!string.IsNullOrWhiteSpace(context.npcPersonality))
        {
            prompt.AppendLine("Твой характер и манера поведения: " + context.npcPersonality + ".");
        }

        prompt.AppendLine("Отвечай как живой персонаж внутри игрового мира, а не как система, модель или ассистент.");
        prompt.AppendLine();
    }

    private void AppendSituationBlock(StringBuilder prompt, DialogueContext context)
    {
        prompt.AppendLine("Текущая игровая ситуация:");

        if (!string.IsNullOrWhiteSpace(context.questName))
        {
            prompt.AppendLine("- Связанный квест: " + context.questName + ".");
        }

        if (!string.IsNullOrWhiteSpace(context.questDescription))
        {
            prompt.AppendLine("- Суть квеста: " + context.questDescription + ".");
        }

        if (!string.IsNullOrWhiteSpace(context.questStatus))
        {
            prompt.AppendLine("- Внутренний статус квеста: " + ConvertQuestStatus(context.questStatus) + ".");
        }

        prompt.AppendLine("- Учитывай текущую стадию задания в своем ответе.");
        prompt.AppendLine();
    }

    private void AppendInventoryBlock(StringBuilder prompt, DialogueContext context)
    {
        prompt.AppendLine("Сведения о предмете у игрока:");

        if (context.inventoryItems != null && context.inventoryItems.Count > 0)
        {
            prompt.AppendLine("- У игрока есть: " + string.Join(", ", context.inventoryItems) + ".");
        }
        else
        {
            prompt.AppendLine("- У игрока сейчас нет значимых предметов для этого разговора.");
        }

        prompt.AppendLine();
    }

    private void AppendDialogueHistoryBlock(StringBuilder prompt, DialogueContext context)
    {
        prompt.AppendLine("Недавний контекст разговора:");

        List<string> lines = GetRecentHistory(context.dialogueHistory, maxHistoryLines);

        if (lines.Count == 0)
        {
            prompt.AppendLine("- Это начало разговора или история пуста.");
        }
        else
        {
            foreach (string line in lines)
            {
                prompt.AppendLine("- " + line);
            }
        }

        prompt.AppendLine();
        prompt.AppendLine("Последняя реплика игрока:");
        prompt.AppendLine(context.playerMessage);
        prompt.AppendLine();
    }

    private void AppendResponseRulesBlock(StringBuilder prompt)
    {
        prompt.AppendLine("Правила ответа:");
        prompt.AppendLine("- Отвечай кратко и естественно, обычно 1–3 предложениями.");
        prompt.AppendLine("- Не перечисляй служебные поля, статусы, названия переменных и внутреннюю структуру игры.");
        prompt.AppendLine("- Не говори, что ты ИИ, языковая модель, NPC-система или программа.");
        prompt.AppendLine("- Не выходи из роли персонажа.");
        prompt.AppendLine("- Если игрок говорит грубо, странно или слишком коротко, все равно отвечай в характере персонажа.");
        prompt.AppendLine("- Если квест еще не завершен, не веди себя так, будто задание уже выполнено.");
        prompt.AppendLine("- Если квест завершен, можешь благодарить, подтверждать результат или переходить к следующей естественной реплике.");
        prompt.AppendLine("- Ответ должен звучать как реплика персонажа в мире игры.");
    }

    private List<string> GetRecentHistory(List<string> fullHistory, int maxLines)
    {
        List<string> result = new List<string>();

        if (fullHistory == null || fullHistory.Count == 0)
            return result;

        int startIndex = Mathf.Max(0, fullHistory.Count - maxLines);

        for (int i = startIndex; i < fullHistory.Count; i++)
        {
            result.Add(fullHistory[i]);
        }

        return result;
    }

    private string ConvertQuestStatus(string rawStatus)
    {
        switch (rawStatus)
        {
            case "NotStarted":
                return "задание еще не начато";
            case "InProgress":
                return "задание выполняется";
            case "Completed":
                return "условие задания уже выполнено";
            case "TurnedIn":
                return "задание уже сдано";
            default:
                return "состояние задания неизвестно";
        }
    }
}