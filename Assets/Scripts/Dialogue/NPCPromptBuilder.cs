using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NPCPromptBuilder : MonoBehaviour
{
    [SerializeField] private int maxHistoryLinesDebug = 6;
    [SerializeField] private int maxHistoryLinesCompact = 4;
    [SerializeField] private bool includeSecretsInDebugPrompt = true;
    [SerializeField] private bool includeSecretsInCompactPrompt = true;
    [SerializeField] private SceneDialogueContext sceneContext;

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
        AppendSceneContextBlock(prompt);
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
        AppendSceneContextBlock(prompt);
        prompt.AppendLine();

        prompt.AppendLine("NPC:");
        prompt.AppendLine("Имя: " + SafeText(context.npcName, "NPC"));
        prompt.AppendLine("Важно: сейчас говорит только этот персонаж: " + SafeText(context.npcName, "NPC") + ".");
        prompt.AppendLine("Не отвечай от имени других персонажей. Не начинай ответ с имени другого персонажа.");
        prompt.AppendLine("Если игрок спрашивает о другом персонаже, текущий NPC может только рассказать о нем со своей точки зрения.");
        prompt.AppendLine("Роль: " + SafeText(context.npcRole, "персонаж сцены"));
        prompt.AppendLine("Характер: " + SafeText(context.npcPersonality, "не указано"));
        prompt.AppendLine("Речь: " + SafeText(context.npcSpeechStyle, "естественная"));
        prompt.AppendLine("Отношение к игроку: " + SafeText(context.npcAttitudeToPlayer, "нейтральное"));
        prompt.AppendLine("Состояние: " + SafeText(context.npcCurrentEmotionalState, "спокойное"));
        prompt.AppendLine("Манера разговора: " + SafeText(context.npcConversationTendency, "говорит естественно"));
        prompt.AppendLine();
        prompt.AppendLine("Знания NPC:");
        prompt.AppendLine("Знает: " + SafeText(context.npcKnowledge, "только то, что видел или слышал в сцене"));
        prompt.AppendLine("Не знает: " + SafeText(context.npcUnknowns, "неизвестные ему факты"));

        if (ShouldIncludeSecretInCompactPrompt(context))
        {
            prompt.AppendLine("Скрыто: " + SafeText(context.npcSecret, ""));
        }

        prompt.AppendLine();

        prompt.AppendLine("Ситуация:");
        prompt.AppendLine("Квест: " + SafeText(context.questName, "нет"));
        prompt.AppendLine("Статус: " + SafeText(context.questStatus, "неизвестно"));
        prompt.AppendLine("Предметы: " + FormatInventoryCompact(context.inventoryItems));
        if (context.inventoryItems == null || context.inventoryItems.Count == 0)
        {
            prompt.AppendLine("Важно: у игрока сейчас нет значимых предметов. Не говори, предмет уже у него на руках, если это не указано ниже в игровом результате.");
        }
        prompt.AppendLine("Задача: " + BuildCompactObjective(context));
        prompt.AppendLine("Тип запроса: " + BuildIntentHint(context));
        if (!string.IsNullOrWhiteSpace(context.lastOutcomeSummary))
        {
            prompt.AppendLine();
            prompt.AppendLine("Только что произошло из-за реплики игрока:");
            prompt.AppendLine(context.lastOutcomeSummary.Trim()); 
            prompt.AppendLine("Это актуальный игровой результат. Если он противоречит предыдущей истории диалога, опирайся на этот результат, а не на старые реплики.");
            prompt.AppendLine("Отрази это в ответе естественно, как текущий персонаж.");
        }
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
            prompt.AppendLine("Особое правило: если игрок просто приветствует, ответь кратко и естественно, не пересказывая задание и не объясняя квест заново.\nЕсли игрок получил предмет через действие правила, ответь так, будто текущий персонаж действительно выдал этот предмет.");
        }
        string exampleLines = BuildStyleExamples(context);
        if (!string.IsNullOrWhiteSpace(exampleLines))
        {
            prompt.AppendLine();
            prompt.AppendLine("Примеры тона:");
            prompt.AppendLine(exampleLines);
        }
        prompt.AppendLine();
        prompt.AppendLine("Правила: 1–3 предложения; отвечай только от имени текущего NPC; не начинай ответ именем другого персонажа; не заключай ответ в кавычки; не используй формат цитаты;" +
            " не выходить из роли; не упоминать систему; не пересказывать контекст; не раскрывать скрытое без повода; не выдумывать факты, персонажей, места или технологии;" +
            " не утверждай, что игрок получил предмет, документ, ордер, разрешение или материалы дела, если это не указано в блоке \"Предметы\" или в блоке \"Только что произошло из-за реплики игрока\"; если предмет еще не выдан, говори, что можешь его выдать, а не что он уже у игрока; говорить конкретно и с опорой на сцену.");
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
        prompt.AppendLine("- Отвечай только от имени текущего персонажа.");
        prompt.AppendLine("- Не начинай ответ с имени другого персонажа.");
        prompt.AppendLine("- Не разыгрывай диалог за других NPC.");
        prompt.AppendLine("- Если игрок получил предмет через действие правила, ответь так, будто текущий персонаж действительно выдал этот предмет.");
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
                    return "отреагировать на найденный результат или улику и направить игрока к следующему естественному шагу.";
                case "TurnedIn":
                    return "отреагировать на завершенный этап поручения, не объявляя всю историю полностью законченной, если это не следует из контекста.";
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
                return "отреагировать на последствия уже переданного результата с точки зрения своего знания и роли.";
            default:
                return "Поддержать естественный разговор, раскрывая характер и отношение персонажа к происходящему.";
        }
    }

    private string BuildCompactObjective(DialogueContext context)
    {
        string status = context.questStatus ?? string.Empty;
        string playerMessage = context.playerMessage != null
            ? context.playerMessage.ToLowerInvariant()
            : string.Empty;

        if (!context.isQuestGiver)
        {
            switch (status)
            {
                case "NotStarted":
                    return "поддержать разговор как участник сцены, не выдавая поручение напрямую";

                case "InProgress":
                    return "дать осторожный взгляд со стороны, опираясь только на свои знания";

                case "Completed":
                    return "отреагировать на найденную улику или результат как наблюдатель, не принимая его вместо квестодателя";

                case "TurnedIn":
                    return "отреагировать на последствия переданной улики, не объявляя всю историю полностью законченной";

                default:
                    return "поддержать разговор в характере персонажа";
            }
        }

        switch (status)
        {
            case "NotStarted":
                return "ввести игрока в ситуацию и обозначить поручение в характере персонажа";

            case "InProgress":
                if (ContainsAny(playerMessage, "почему", "что", "зачем", "как", "каким образом"))
                    return "ответить предметно, не раскрывая больше, чем персонаж может знать";

                return "направить игрока к следующему логичному шагу";

            case "Completed":
                return "признать, что игрок нашел нужный результат или улику, и предложить передать ее";

            case "TurnedIn":
                return "отреагировать на уже переданный результат как на завершенный этап, но не объявлять всю историю полностью раскрытой";

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
                return "текущий этап еще не начат";

            case "InProgress":
                return "текущий этап выполняется";

            case "Completed":
                return "условие текущего этапа выполнено, результат еще не передан";

            case "TurnedIn":
                return "этап поручения завершен, результат уже передан персонажу";

            default:
                return "состояние текущего этапа не уточнено";
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

    private bool ShouldIncludeSecretInCompactPrompt(DialogueContext context)
    {
        if (!includeSecretsInCompactPrompt)
            return false;

        if (context == null || string.IsNullOrWhiteSpace(context.npcSecret))
            return false;

        string playerMessage = context.playerMessage != null
            ? context.playerMessage.ToLowerInvariant()
            : string.Empty;

        string questStatus = context.questStatus ?? string.Empty;

        if (questStatus == "Completed" || questStatus == "TurnedIn")
            return true;

        return ContainsAny(playerMessage,
            "что",
            "почему",
            "зачем",
            "как",
            "подозрева",
            "секрет",
            "скрыва",
            "улика",
            "доказательство",
            "видел",
            "видела",
            "знаешь",
            "правда");
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
        if (!string.IsNullOrWhiteSpace(context.npcSpeechStyle))
            return Clean(context.npcSpeechStyle);

        if (!string.IsNullOrWhiteSpace(context.npcConversationTendency))
            return Clean(context.npcConversationTendency);

        return "говори естественно, кратко и в характере персонажа";
    }

    private string BuildAvoidHint(DialogueContext context)
    {
        return "не выдумывай факты, не меняй роли персонажей, не раскрывай неизвестное персонажу, не говори как справочник";
    }

    private string BuildStyleExamples(DialogueContext context)
    {
        return string.Empty;
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
            "помощь", "есть работа", "есть еще", "есть ещё", "что мне нужно", "что понадобится"))
            return "прагматичный запрос; ответь по ситуации и по делу, без лишних рассуждений.";

        if (ContainsAny(playerMessage,
            "не страшно", "не думаю", "сомневаюсь", "не верю", "вряд ли"))
            return "сомнение игрока; ответь спокойно, без нажима, но сохрани настороженность персонажа.";

        if (ContainsAny(playerMessage,
            "ордер", "разрешение", "пропуск", "допуск", "доступ", "документ") &&
            ContainsAny(playerMessage,
            "осмотр", "осмотреть", "место преступления", "архив", "пройти", "пропустили", "войти",
            "возьму", "заберу", "получить", "взять", "забрать", "нужен", "нужно"))
        {
            return "просьба о разрешении или документе для доступа; если этот предмет был выдан через игровой результат, упомяни его выдачу; если не был выдан, не говори, что он уже у игрока.";
        }

        if (ContainsAny(playerMessage, "хорошо", "понял", "запомню", "ладно", "ясно", "проверю"))
            return "подтверждение; ответь кратко, как на принятие совета, без новых объяснений.";

        if (ContainsAny(playerMessage, "думаю", "я смогу", "я справлюсь", "я готов", "я найду", "уверен", "уверена"))
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
    private void AppendSceneContextBlock(StringBuilder prompt)
    {
        if (sceneContext == null)
            return;

        prompt.AppendLine("МИР И СЦЕНА");

        if (!string.IsNullOrWhiteSpace(sceneContext.WorldDescription))
            prompt.AppendLine(sceneContext.WorldDescription.Trim());

        if (!string.IsNullOrWhiteSpace(sceneContext.KnownCharacters))
        {
            prompt.AppendLine();
            prompt.AppendLine("Известные персонажи:");
            prompt.AppendLine(sceneContext.KnownCharacters.Trim());
        }

        if (!string.IsNullOrWhiteSpace(sceneContext.WorldRules))
        {
            prompt.AppendLine();
            prompt.AppendLine("Ограничения мира:");
            prompt.AppendLine(sceneContext.WorldRules.Trim());
        }

        prompt.AppendLine();
    }
}