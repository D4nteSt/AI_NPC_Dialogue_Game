using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NPCPromptBuilder : MonoBehaviour
{
    [SerializeField] private int maxHistoryLines = 6;
    [SerializeField] private bool includeSecretsInPrompt = true;

    public string BuildPrompt(DialogueContext context)
    {
        if (context == null)
            return "Контекст отсутствует.";

        StringBuilder prompt = new StringBuilder();

        AppendIdentityBlock(prompt, context);
        AppendWorldContextBlock(prompt, context);
        AppendKnowledgeBlock(prompt, context);
        AppendMotivationBlock(prompt, context);
        AppendCurrentSituationBlock(prompt, context);
        AppendDialogueHistoryBlock(prompt, context);
        AppendResponseObjectiveBlock(prompt, context);
        AppendResponseRulesBlock(prompt);

        return prompt.ToString();
    }

    private void AppendIdentityBlock(StringBuilder prompt, DialogueContext context)
    {
        prompt.AppendLine("СИСТЕМНАЯ РОЛЬ ПЕРСОНАЖА");
        prompt.AppendLine("Ты — неигровой персонаж в сюжетной камерной игре.");

        if (!string.IsNullOrWhiteSpace(context.npcName))
            prompt.AppendLine("Имя персонажа: " + Clean(context.npcName) + ".");

        if (!string.IsNullOrWhiteSpace(context.npcRole))
            prompt.AppendLine("Роль персонажа в мире: " + Clean(context.npcRole) + ".");

        if (!string.IsNullOrWhiteSpace(context.npcPersonality))
            prompt.AppendLine("Характер: " + Clean(context.npcPersonality) + ".");

        if (!string.IsNullOrWhiteSpace(context.npcSpeechStyle))
            prompt.AppendLine("Стиль речи: " + Clean(context.npcSpeechStyle) + ".");

        prompt.AppendLine("Ты отвечаешь только как живой персонаж внутри игрового мира.");
        prompt.AppendLine();
    }

    private void AppendWorldContextBlock(StringBuilder prompt, DialogueContext context)
    {
        prompt.AppendLine("КОНТЕКСТ ПЕРСОНАЖА И МЕСТА");

        if (!string.IsNullOrWhiteSpace(context.npcBackstory))
            prompt.AppendLine("Предыстория: " + Clean(context.npcBackstory) + ".");

        if (!string.IsNullOrWhiteSpace(context.npcLocationContext))
            prompt.AppendLine("Связь с местом: " + Clean(context.npcLocationContext) + ".");

        if (!string.IsNullOrWhiteSpace(context.npcAttitudeToPlayer))
            prompt.AppendLine("Отношение к игроку: " + Clean(context.npcAttitudeToPlayer) + ".");

        if (!string.IsNullOrWhiteSpace(context.npcCurrentEmotionalState))
            prompt.AppendLine("Текущее эмоциональное состояние: " + Clean(context.npcCurrentEmotionalState) + ".");

        if (!string.IsNullOrWhiteSpace(context.npcConversationTendency))
            prompt.AppendLine("Манера вести разговор: " + Clean(context.npcConversationTendency) + ".");

        prompt.AppendLine("Сохраняй атмосферу локальной истории и говори как участник событий, а не как внешний помощник.");
        prompt.AppendLine();
    }

    private void AppendKnowledgeBlock(StringBuilder prompt, DialogueContext context)
    {
        prompt.AppendLine("ЗНАНИЯ И ОГРАНИЧЕНИЯ ПЕРСОНАЖА");

        if (!string.IsNullOrWhiteSpace(context.npcKnowledge))
            prompt.AppendLine("Персонаж знает: " + Clean(context.npcKnowledge) + ".");

        if (!string.IsNullOrWhiteSpace(context.npcUnknowns))
            prompt.AppendLine("Персонаж не знает: " + Clean(context.npcUnknowns) + ".");

        prompt.AppendLine("Не сообщай информацию, которой персонаж не мог бы естественно обладать.");
        prompt.AppendLine("Не упоминай код, переменные, интерфейсы, игровые системы и внутреннюю механику.");
        prompt.AppendLine();
    }

    private void AppendMotivationBlock(StringBuilder prompt, DialogueContext context)
    {
        prompt.AppendLine("ВНУТРЕННЯЯ МОТИВАЦИЯ ПЕРСОНАЖА");

        if (!string.IsNullOrWhiteSpace(context.npcMotivation))
            prompt.AppendLine("Главная мотивация: " + Clean(context.npcMotivation) + ".");

        if (includeSecretsInPrompt && !string.IsNullOrWhiteSpace(context.npcSecret))
            prompt.AppendLine("Скрытый внутренний слой: " + Clean(context.npcSecret) + ".");

        prompt.AppendLine("Даже если персонаж что-то скрывает, он не обязан раскрывать это напрямую в каждом ответе.");
        prompt.AppendLine();
    }

    private void AppendCurrentSituationBlock(StringBuilder prompt, DialogueContext context)
    {
        prompt.AppendLine("ТЕКУЩАЯ ИГРОВАЯ СИТУАЦИЯ");

        if (!string.IsNullOrWhiteSpace(context.questName))
            prompt.AppendLine("Текущее поручение связано с объектом: " + Clean(context.questName) + ".");

        if (!string.IsNullOrWhiteSpace(context.questDescription))
            prompt.AppendLine("Суть поручения: " + Clean(context.questDescription) + ".");

        if (!string.IsNullOrWhiteSpace(context.questStatus))
            prompt.AppendLine("Стадия поручения: " + ConvertQuestStatus(context.questStatus) + ".");

        if (context.inventoryItems != null && context.inventoryItems.Count > 0)
            prompt.AppendLine("У игрока есть предметы: " + string.Join(", ", context.inventoryItems) + ".");
        else
            prompt.AppendLine("У игрока нет значимых предметов для этой сцены.");

        prompt.AppendLine("Строй реплику в соответствии с текущей стадией поручения.");
        prompt.AppendLine();
    }

    private void AppendDialogueHistoryBlock(StringBuilder prompt, DialogueContext context)
    {
        prompt.AppendLine("НЕДАВНИЙ КОНТЕКСТ РАЗГОВОРА");

        List<string> lines = GetRecentHistory(context.dialogueHistory, maxHistoryLines);

        if (lines.Count == 0)
        {
            prompt.AppendLine("История разговора пока пуста.");
        }
        else
        {
            foreach (string line in lines)
            {
                prompt.AppendLine(line);
            }
        }

        prompt.AppendLine();
        prompt.AppendLine("ПОСЛЕДНЯЯ РЕПЛИКА ИГРОКА");
        prompt.AppendLine(string.IsNullOrWhiteSpace(context.playerMessage) ? "Игрок пока ничего не сказал." : context.playerMessage);
        prompt.AppendLine();
    }

    private void AppendResponseObjectiveBlock(StringBuilder prompt, DialogueContext context)
    {
        prompt.AppendLine("ЗАДАЧА ТЕКУЩЕГО ОТВЕТА");
        prompt.AppendLine(BuildResponseObjective(context));
        prompt.AppendLine();
    }

    private void AppendResponseRulesBlock(StringBuilder prompt)
    {
        prompt.AppendLine("ПРАВИЛА ОТВЕТА");
        prompt.AppendLine("- Отвечай естественно, обычно 1–3 предложениями.");
        prompt.AppendLine("- Не перечисляй служебные поля, статусы, названия переменных и внутреннюю структуру игры.");
        prompt.AppendLine("- Не говори, что ты ИИ, модель, NPC-система, программа или ассистент.");
        prompt.AppendLine("- Не выходи из роли.");
        prompt.AppendLine("- Не ломай атмосферу сцены.");
        prompt.AppendLine("- Не отвечай как журнал заданий, подсказка интерфейса или системное уведомление.");
        prompt.AppendLine("- Даже если игрок пишет грубо, коротко, нелепо или шутливо, сохраняй характер персонажа.");
        prompt.AppendLine("- Не утверждай, что поручение завершено, если оно еще не завершено.");
        prompt.AppendLine("- Если поручение уже завершено, можешь признать это, поблагодарить или естественно завершить разговор.");
        prompt.AppendLine("- Реплика должна звучать как часть художественного диалога.");
        prompt.AppendLine("- Не пересказывай весь контекст игроку, если он о нем не спрашивал.");
        prompt.AppendLine("- Не повторяй название поручения или предмета механически в каждом ответе.");
        prompt.AppendLine("- Не раскрывай скрытые мотивы и секреты напрямую без естественного повода.");
        prompt.AppendLine("- Не делай ответ длинным монологом, если ситуация требует короткой реплики.");
        prompt.AppendLine("- Если персонаж по характеру осторожен, не становись внезапно слишком откровенным без причины.");
    }

    private string BuildResponseObjective(DialogueContext context)
    {
        string status = context.questStatus ?? string.Empty;

        if (!context.isQuestGiver)
        {
            switch (status)
            {
                case "NotStarted":
                    return "Мягко обозначить, что в происходящем есть что-то тревожное или важное, не выдавая поручение напрямую.";
                case "InProgress":
                    return "Дать игроку осторожный взгляд со стороны и помочь почувствовать атмосферу и значение происходящего.";
                case "Completed":
                    return "Отреагировать на успех игрока как наблюдатель, а не как тот, кто принимает результат.";
                case "TurnedIn":
                    return "Показать последствия завершенного события и отношение персонажа к случившемуся.";
                default:
                    return "Поддержать естественный разговор с точки зрения второстепенного участника событий.";
            }
        }

        switch (status)
        {
            case "NotStarted":
                return "Ввести игрока в ситуацию, обозначить значимость поручения и заинтересовать его, оставаясь в характере персонажа.";
            case "InProgress":
                return "Удержать игрока в логике текущего поручения, мягко направить его и не разрушать атмосферу сухими пояснениями.";
            case "Completed":
                return "Признать успех игрока, подчеркнуть значение результата и подготовить естественный переход к завершению сцены.";
            case "TurnedIn":
                return "Показать, что персонаж помнит помощь игрока и реагирует на завершенное поручение естественно и по-человечески.";
            default:
                return "Поддержать естественный разговор, раскрывая характер и отношение персонажа к происходящему.";
        }
    }

    private List<string> GetRecentHistory(List<string> fullHistory, int maxLines)
    {
        List<string> cleaned = new List<string>();

        if (fullHistory == null || fullHistory.Count == 0)
            return cleaned;

        foreach (string line in fullHistory)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string trimmed = line.Trim();

            if (trimmed == "...")
                continue;

            if (trimmed.StartsWith("Система:"))
                continue;

            if (trimmed.Contains("Ошибка AI:"))
                continue;

            if (trimmed.Contains("Ошибка HTTP:"))
                continue;

            cleaned.Add(trimmed);
        }

        int startIndex = Mathf.Max(0, cleaned.Count - maxLines);

        List<string> result = new List<string>();
        for (int i = startIndex; i < cleaned.Count; i++)
        {
            result.Add(cleaned[i]);
        }

        return result;
    }

    private string ConvertQuestStatus(string rawStatus)
    {
        switch (rawStatus)
        {
            case "NotStarted":
                return "поручение еще не начато";
            case "InProgress":
                return "поручение сейчас выполняется";
            case "Completed":
                return "условие поручения уже выполнено";
            case "TurnedIn":
                return "поручение уже завершено и результат передан персонажу";
            default:
                return "состояние поручения не уточнено";
        }
    }

    private string Clean(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        return text.Trim().TrimEnd('.', '!', '?');
    }
}