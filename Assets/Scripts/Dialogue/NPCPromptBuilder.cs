using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NPCPromptBuilder : MonoBehaviour
{
    [SerializeField] private int maxHistoryLinesDebug = 6;
    [SerializeField] private int maxHistoryLinesCompact = 4;
    [SerializeField] private bool includeSecretsInDebugPrompt = true;
    [SerializeField] private bool includeSecretsInCompactPrompt = true;

    public string BuildPrompt(DialogueContext context)
    {
        return BuildPrompt(context, PromptMode.Debug);
    }

    public string BuildPrompt(DialogueContext context, PromptMode mode)
    {
        if (context == null)
            return "Контекст отсутствует.";

        return mode == PromptMode.Compact
            ? BuildCompactPrompt(context)
            : BuildDebugPrompt(context);
    }

    private string BuildDebugPrompt(DialogueContext context)
    {
        StringBuilder prompt = new StringBuilder();

        AppendIdentityBlock(prompt, context);
        AppendWorldContextBlock(prompt, context);
        AppendKnowledgeBlock(prompt, context);
        AppendMotivationBlock(prompt, context, includeSecretsInDebugPrompt);
        AppendCurrentSituationBlock(prompt, context);
        AppendDialogueHistoryBlock(prompt, context, maxHistoryLinesDebug);
        AppendResponseObjectiveBlock(prompt, context);
        AppendResponseRulesBlock(prompt);

        return prompt.ToString();
    }

    private string BuildCompactPrompt(DialogueContext context)
    {
        StringBuilder prompt = new StringBuilder();

        prompt.AppendLine("Ты NPC в сюжетной игре. Отвечай только как персонаж.");
        prompt.AppendLine("Мир:\r\nДетективная сцена у городского архива. Пропала городская печать.\r\nИзвестные персонажи: Инспектор Баттлер — ведет дело; Мира — свидетельница.\r\nНе выдумывай новых важных персонажей, современные технологии и места, которых нет в контексте.\r\nЕсли не знаешь — говори осторожно и предлагай проверить архив, место происшествия или обратиться к Баттлеру.");
        prompt.AppendLine();

        prompt.AppendLine("NPC:");
        prompt.AppendLine("Имя: " + SafeText(context.npcName, "NPC"));
        prompt.AppendLine("Роль: " + CompressRole(context.npcRole));
        prompt.AppendLine("Характер: " + CompressTraitsCompact(context.npcPersonality));
        prompt.AppendLine("Речь: " + CompressSpeech(context.npcSpeechStyle));
        prompt.AppendLine("Отношение: " + CompressAttitude(context.npcAttitudeToPlayer));
        prompt.AppendLine("Состояние: " + CompressState(context.npcCurrentEmotionalState));
        prompt.AppendLine("Тон: " + BuildToneHint(context));
        prompt.AppendLine("Избегай: " + BuildAvoidHint(context));

        if (ShouldIncludeSecretInCompactPrompt(context))
        {
            prompt.AppendLine("Скрыто: " + CompressSecret(context.npcSecret));
        }

        prompt.AppendLine();

        prompt.AppendLine("Ситуация:");
        prompt.AppendLine("Квест: " + SafeText(context.questName, "нет"));
        prompt.AppendLine("Статус: " + SafeText(context.questStatus, "неизвестно"));
        prompt.AppendLine("Предметы: " + FormatInventoryCompact(context.inventoryItems));
        prompt.AppendLine("Задача: " + BuildCompactObjective(context));
        prompt.AppendLine("Тип запроса: " + BuildIntentHint(context));
        prompt.AppendLine();

        prompt.AppendLine("Последние реплики:");
        List<string> lines = GetRecentHistory(context.dialogueHistory, maxHistoryLinesCompact);
        if (lines.Count == 0)
        {
            prompt.AppendLine("Начало разговора.");
        }
        else
        {
            foreach (string line in lines)
            {
                prompt.AppendLine(line);
            }
        }

        prompt.AppendLine();
        prompt.AppendLine("Последняя реплика игрока: " + SafeText(context.playerMessage, "..."));
        if (IsGreetingMessage(context.playerMessage))
        {
            prompt.AppendLine("Особое правило: если игрок просто приветствует, ответь кратко и естественно, не пересказывая задание и не объясняя квест заново.");
        }
        string exampleLines = BuildStyleExamples(context);
        if (!string.IsNullOrWhiteSpace(exampleLines))
        {
            prompt.AppendLine();
            prompt.AppendLine("Примеры тона:");
            prompt.AppendLine(exampleLines);
        }
        prompt.AppendLine();
        prompt.AppendLine("Правила: 1–3 предложения; не выходить из роли; не упоминать систему; не пересказывать контекст; не раскрывать скрытое без повода; не говорить слишком абстрактно; не использовать шаблонные фэнтези-фразы без опоры на сцену.");

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

    private void AppendMotivationBlock(StringBuilder prompt, DialogueContext context, bool includeSecrets)
    {
        prompt.AppendLine("ВНУТРЕННЯЯ МОТИВАЦИЯ ПЕРСОНАЖА");

        if (!string.IsNullOrWhiteSpace(context.npcMotivation))
            prompt.AppendLine("Главная мотивация: " + Clean(context.npcMotivation) + ".");

        if (includeSecrets && !string.IsNullOrWhiteSpace(context.npcSecret))
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

    private void AppendDialogueHistoryBlock(StringBuilder prompt, DialogueContext context, int maxLines)
    {
        prompt.AppendLine("НЕДАВНИЙ КОНТЕКСТ РАЗГОВОРА");

        List<string> lines = GetRecentHistory(context.dialogueHistory, maxLines);

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

    private string BuildCompactObjective(DialogueContext context)
    {
        string status = context.questStatus ?? string.Empty;
        string playerMessage = context.playerMessage ?? string.Empty;

        if (!context.isQuestGiver)
        {
            switch (status)
            {
                case "NotStarted":
                    return "намекнуть, что ситуация важна или тревожна";
                case "InProgress":
                    return "дать осторожный взгляд со стороны";
                case "Completed":
                    return "признать успех игрока как наблюдатель";
                case "TurnedIn":
                    return "отразить последствия завершенного события";
                default:
                    return "поддержать разговор в характере персонажа";
            }
        }

        switch (status)
        {
            case "NotStarted":
                return "ввести в ситуацию и заинтересовать";
            case "InProgress":
                if (playerMessage.Contains("почему") || playerMessage.Contains("что") || playerMessage.Contains("зачем") || playerMessage.Contains("как"))
                    return "ответить осторожно и предметно, частично раскрывая смысл без полного объяснения";
                else return "мягко направить игрока"; 


            case "Completed":
                        return "признать успех и подготовить завершение";
                    case "TurnedIn":
                        return "отреагировать на завершенное поручение";
                    default:
                        return "поддержать разговор в характере персонажа";
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

    private string SafeText(string text, string fallback)
    {
        return string.IsNullOrWhiteSpace(text) ? fallback : Clean(text);
    }

    private string Clean(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        return text.Trim().TrimEnd('.', '!', '?');
    }

    private string CompressTraitsCompact(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "не указано";

        string cleaned = Clean(text).ToLowerInvariant();

        if (cleaned.Contains("мудр") && cleaned.Contains("осторож"))
            return "мудрый, осторожный";

        if (cleaned.Contains("вниматель") && cleaned.Contains("мягк"))
            return "внимательная, мягкая";

        return CompressByWords(cleaned, 6);
    }

    private string CompressSpeech(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "обычная";

        string cleaned = Clean(text).ToLowerInvariant();

        if (cleaned.Contains("намек") || cleaned.Contains("намёк"))
            return "размеренная, краткая, с намеками";

        if (cleaned.Contains("прямо") || cleaned.Contains("прям"))
            return "мягкая, прямая, без загадок";

        return CompressByWords(cleaned, 6);
    }

    private string CompressAttitude(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "нейтральное";

        string cleaned = Clean(text).ToLowerInvariant();

        if (cleaned.Contains("осторож") && cleaned.Contains("довер"))
            return "осторожное доверие";

        if (cleaned.Contains("насторож"))
            return "настороженность";

        return CompressByWords(cleaned, 5);
    }

    private string CompressState(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "спокойствие";

        string cleaned = Clean(text).ToLowerInvariant();

        if (cleaned.Contains("напряж"))
            return "спокойное напряжение";

        if (cleaned.Contains("насторож"))
            return "спокойная настороженность";

        return CompressByWords(cleaned, 5);
    }

    private string CompressRole(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "персонаж локации";

        string cleaned = Clean(text);

        if (cleaned.Length <= 40)
            return cleaned;

        return CompressByWords(cleaned, 6);
    }

    private string CompressSecret(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        string cleaned = Clean(text).ToLowerInvariant();

        if (cleaned.Contains("артефакт") && cleaned.Contains("говор"))
            return "знает об артефакте больше, чем говорит";

        return CompressByWords(cleaned, 8);
    }

    private bool ShouldIncludeSecretInCompactPrompt(DialogueContext context)
    {
        if (context == null || string.IsNullOrWhiteSpace(context.npcSecret))
            return false;

        string playerMessage = context.playerMessage != null
            ? context.playerMessage.ToLowerInvariant()
            : string.Empty;

        string questStatus = context.questStatus ?? string.Empty;

        return questStatus == "InProgress" ||
               questStatus == "Completed" ||
               playerMessage.Contains("артефакт") ||
               playerMessage.Contains("руин") ||
               playerMessage.Contains("квест") ||
               playerMessage.Contains("задание") ||
               playerMessage.Contains("что") ||
               playerMessage.Contains("какой");
    }

    private string CompressByWords(string text, int maxWords)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "не указано";

        string cleaned = Clean(text);
        string[] words = cleaned.Split(' ');

        if (words.Length <= maxWords)
            return cleaned;

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < maxWords; i++)
        {
            if (i > 0)
                sb.Append(" ");

            sb.Append(words[i]);
        }

        return sb.ToString().TrimEnd(',', ';', '.') + "...";
    }

    private string FormatInventoryCompact(List<string> items)
    {
        if (items == null || items.Count == 0)
            return "пусто";

        if (items.Count == 1)
            return items[0];

        return items[0] + " +" + (items.Count - 1);
    }
    private bool IsGreetingMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        string cleaned = text.Trim().ToLowerInvariant();

        return cleaned == "привет" ||
               cleaned == "здравствуй" ||
               cleaned == "здравствуйте" ||
               cleaned == "здарова" ||
               cleaned == "добрый день" ||
               cleaned == "hi" ||
               cleaned == "hello";
    }

    private string BuildToneHint(DialogueContext context)
    {
        string npcName = context.npcName != null ? context.npcName.ToLowerInvariant() : "";
        string role = context.npcRole != null ? context.npcRole.ToLowerInvariant() : "";

        bool isOldMan = npcName.Contains("старик") || role.Contains("хранитель");
        bool isLea = npcName.Contains("лея") || role.Contains("травниц");

        if (isOldMan)
            return "спокойно, коротко, сдержанно; иногда через образы пути, камня, тишины, памяти; говорить скорее намеком, чем легендой";

        if (isLea)
            return "мягко, просто, осторожно; чуть живее и человечнее, чем Старик; меньше загадочности; говорить через ощущение места, леса, троп, тишины и тревоги";

        return "естественно, кратко, в характере персонажа";
    }

    private string BuildAvoidHint(DialogueContext context)
    {
        string npcName = context.npcName != null ? context.npcName.ToLowerInvariant() : "";
        string role = context.npcRole != null ? context.npcRole.ToLowerInvariant() : "";

        bool isOldMan = npcName.Contains("старик") || role.Contains("хранитель");
        bool isLea = npcName.Contains("лея") || role.Contains("травниц");

        if (isOldMan)
            return "не говорить как справочник; не пересказывать квест заново; избегать общих фраз вроде 'древняя сила', 'изменить судьбу', 'проклятие', 'тайны прошлого', если они не опираются на сцену; говорить конкретнее и суше; если игрок просто подтверждает или соглашается, отвечать коротко и не вводить новую тему";

        if (isLea)
            return "не говорить слишком пафосно; не звучать как Старик; избегать абстрактных фраз и лишней загадочности; не преувеличивать значение артефакта до масштаба мира, если персонаж не может знать этого наверняка";

        return "не использовать абстрактные и шаблонные формулировки без опоры на ситуацию";
    }

    private string BuildStyleExamples(DialogueContext context)
    {
        string npcName = context.npcName != null ? context.npcName.ToLowerInvariant() : "";
        string role = context.npcRole != null ? context.npcRole.ToLowerInvariant() : "";

        bool isOldMan = npcName.Contains("старик") || role.Contains("хранитель");
        bool isLea = npcName.Contains("лея") || role.Contains("травниц");

        if (isOldMan)
        {
            return "- \"Не всякая вещь любит чужие руки.\"\n" +
                   "- \"Тише ступай — руины не терпят спешки.\"\n" +
                   "- \"Мудрость тут полезнее силы.\"";
        }

        if (isLea)
        {
            return "- \"Я бы на твоем месте не шла туда без осторожности.\"\n" +
                   "- \"В лесу сейчас тревожно, даже если с виду тихо.\"\n" +
                   "- \"Старик не все говорит сразу, но в одном он прав.\"";
        }

        return "";
    }

    private string BuildIntentHint(DialogueContext context)
    {
        string playerMessage = context.playerMessage != null
            ? context.playerMessage.Trim().ToLowerInvariant()
            : string.Empty;

        if (string.IsNullOrWhiteSpace(playerMessage))
            return "общий разговор; ответь кратко и в характере персонажа.";

        if (IsGreetingMessage(playerMessage))
            return "приветствие; ответь кратко и естественно, не пересказывая задание.";

        if (ContainsAny(playerMessage,
            "где", "куда", "как пройти", "где искать", "куда идти", "где это"))
            return "вопрос о месте; ответь конкретнее и короче, без лишней поэзии.";

        if (ContainsAny(playerMessage,
            "что это", "что за", "почему", "зачем", "в чем", "в чём", "что в нем", "что в нём"))
            return "уточняющий вопрос о смысле; ответь осторожно, но по существу.";

        if (ContainsAny(playerMessage,
            "что делать", "что мне делать", "как мне", "как быть", "что дальше", "что с ним делать"))
            return "просьба о практическом совете; ответь осторожно, но по делу.";

        if (ContainsAny(playerMessage,
            "припасы", "еда", "помощь", "заданий", "задание еще", "задание ещё", "есть работа", "есть еще", "есть ещё"))
            return "прагматичный запрос; ответь по ситуации и по делу, без философских общих фраз.";

        if (ContainsAny(playerMessage,
            "не страшно", "не думаю", "сомневаюсь", "не верю", "вряд ли"))
            return "сомнение игрока; ответь спокойно, без нажима, но сохрани настороженность персонажа.";

        if (ContainsAny(playerMessage, "хорошо", "понял", "запомню", "ладно", "ясно", "проверю"))
            return "подтверждение; ответь кратко, как на принятие совета, без новых объяснений.";

        if (ContainsAny(playerMessage, "думаю", "я смогу", "я справлюсь", "моей мудрости", "я готов", "я найду"))
            return "самоуверенное заявление; ответь сдержанно, можно чуть охладить уверенность игрока, но без длинней лекции.";

        if (ContainsAny(playerMessage, "уважительно", "как с ним разговаривать", "ему всё равно"))
            return "вопрос о поведении; дай короткий практичный совет, без лора.";

        return "общий разговор; ответь кратко, в характере персонажа, по существу текущей реплики.";
    }

    private bool ContainsAny(string text, params string[] variants)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        foreach (string variant in variants)
        {
            if (text.Contains(variant))
                return true;
        }

        return false;
    }
}