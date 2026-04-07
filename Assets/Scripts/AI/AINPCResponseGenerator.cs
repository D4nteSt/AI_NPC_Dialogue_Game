using UnityEngine;

public class AINPCResponseGenerator : MonoBehaviour, INPCResponseGenerator
{
    [SerializeField] private NPCPromptBuilder promptBuilder;
    [SerializeField] private bool logPromptToConsole = true;

    public string GenerateResponse(DialogueContext context)
    {
        if (promptBuilder == null)
            return "Ошибка: PromptBuilder не назначен.";

        string prompt = promptBuilder.BuildPrompt(context);

        if (logPromptToConsole)
        {
            Debug.Log("=== AI PROMPT START ===\n" + prompt + "\n=== AI PROMPT END ===");
        }

        return BuildPseudoAIResponse(context);
    }

    private string BuildPseudoAIResponse(DialogueContext context)
    {
        if (context == null)
            return "Мне трудно собраться с мыслями.";

        string npcName = Normalize(context.npcName);
        string role = Normalize(context.npcRole);
        string personality = Normalize(context.npcPersonality);
        string speechStyle = Normalize(context.npcSpeechStyle);
        string emotionalState = Normalize(context.npcCurrentEmotionalState);
        string playerMessage = Normalize(context.playerMessage);
        string status = context.questStatus ?? string.Empty;

        bool isLea = npcName.Contains("лея") || role.Contains("травниц") || role.Contains("помощниц");
        bool isOldMan = npcName.Contains("старик") || role.Contains("хранител");

        switch (status)
        {
            case "NotStarted":
                return BuildNotStartedResponse(context, isLea, isOldMan, playerMessage, personality, speechStyle, emotionalState);

            case "InProgress":
                return BuildInProgressResponse(context, isLea, isOldMan, playerMessage, personality, speechStyle, emotionalState);

            case "Completed":
                return BuildCompletedResponse(context, isLea, isOldMan, playerMessage, personality, speechStyle, emotionalState);

            case "TurnedIn":
                return BuildTurnedInResponse(context, isLea, isOldMan, playerMessage, personality, speechStyle, emotionalState);

            default:
                return BuildFallbackResponse(context, isLea, isOldMan, playerMessage, personality, speechStyle, emotionalState);
        }
    }

    private string BuildNotStartedResponse(
        DialogueContext context,
        bool isLea,
        bool isOldMan,
        string playerMessage,
        string personality,
        string speechStyle,
        string emotionalState)
    {
        if (isLea)
        {
            if (ContainsAny(playerMessage, "какое", "что за", "что именно", "да?", "да"))
            {
                return "Речь о старой реликвии из руин. Я бы не тревожила ее без причины, но оставлять ее там тоже не стоит.";
            }

            if (ContainsAny(playerMessage, "помочь", "зачем", "что случилось"))
            {
                return "Если коротко, в руинах осталась вещь, которую лучше не оставлять без присмотра. Старик просит вернуть ее, и, думаю, он прав.";
            }

            return "Я не стала бы тревожить тебя без повода. В руинах осталась вещь, из-за которой это место будто не может успокоиться.";
        }

        if (isOldMan)
        {
            if (ContainsAny(playerMessage, "какое", "что за", "что именно", "да?", "да"))
            {
                return "Есть вещь, что не должна долго лежать среди руин. Найди ее и принеси мне — так будет лучше для всех.";
            }

            if (ContainsAny(playerMessage, "зачем", "почему", "что случилось"))
            {
                return "Не всякую древнюю вещь стоит оставлять там, где ее будит чужой взгляд. Верни ее мне, и этого пока будет достаточно.";
            }

            return "У меня есть просьба, путник. В руинах покоится старый предмет, и ему не место среди заброшенных камней.";
        }

        if (ContainsAny(playerMessage, "какое", "что", "зачем"))
        {
            return "Мне нужна помощь с одной старой вещью. Она осталась у руин, и лучше бы ей вернуться сюда.";
        }

        return "Мне нужна помощь. Есть вещь, которую следует вернуть прежде, чем она навлечет лишнюю беду.";
    }

    private string BuildInProgressResponse(
        DialogueContext context,
        bool isLea,
        bool isOldMan,
        string playerMessage,
        string personality,
        string speechStyle,
        string emotionalState)
    {
        bool hasQuestItem = HasQuestItem(context);

        if (hasQuestItem)
        {
            if (isLea)
                return "Похоже, ты уже нашел то, что искал. Тогда не тяни — лучше отнеси это старику поскорее.";

            if (isOldMan)
                return "Я вижу, путь твой был не напрасен. Если предмет у тебя, не держи его при себе дольше нужного.";

            return "Похоже, нужная вещь уже у тебя. Лучше скорее завершить начатое.";
        }

        if (isLea)
        {
            if (ContainsAny(playerMessage, "нет", "не нашел", "не нашёл", "не нашла", "не нашёл ещё", "нет ещё"))
            {
                return "Тогда не спеши и смотри внимательнее. У руин легко пропустить важное, если идти туда без мысли.";
            }

            if (ContainsAny(playerMessage, "где", "куда", "искать", "найти"))
            {
                return "Ищи ближе к старым камням и не сходи с тропы без нужды. Там место тихое, но не пустое.";
            }

            if (ContainsAny(playerMessage, "страшно", "опасно", "боюсь"))
            {
                return "Осторожность здесь не слабость. Просто не лезь напролом и не трогай лишнего.";
            }

            return "Если решишь продолжать, будь внимателен. В этих местах важнее смотреть под ноги и по сторонам, чем торопиться.";
        }

        if (isOldMan)
        {
            if (ContainsAny(playerMessage, "нет", "не нашел", "не нашёл", "нет ещё"))
            {
                return "Значит, час еще не настал. Ищи терпеливо — древние вещи редко даются тому, кто торопится.";
            }

            if (ContainsAny(playerMessage, "где", "куда", "искать", "найти"))
            {
                return "Туда, где камень помнит больше, чем люди. У руин ищи внимательнее — вещь не любит праздного взгляда.";
            }

            if (ContainsAny(playerMessage, "опасно", "страшно", "боюсь"))
            {
                return "Опасность не всегда шумит, путник. Но уважение к месту порой надежнее храбрости.";
            }

            return "Путь еще не окончен. Не ищи силу — ищи след, и предмет сам откроется внимательному.";
        }

        if (ContainsAny(playerMessage, "где", "искать", "найти"))
            return "Поищи у руин внимательнее. Вероятно, нужная вещь рядом.";

        return "Поручение еще не завершено. Попробуй осмотреть место у руин еще раз.";
    }

    private string BuildCompletedResponse(
        DialogueContext context,
        bool isLea,
        bool isOldMan,
        string playerMessage,
        string personality,
        string speechStyle,
        string emotionalState)
    {
        if (isLea)
        {
            if (ContainsAny(playerMessage, "держи", "вот", "принес", "принёс", "нашел", "нашёл"))
            {
                return "Да, это она. Честно говоря, я рада, что она больше не лежит там.";
            }

            return "Похоже, ты справился. Лучше не носи эту вещь с собой дольше, чем нужно.";
        }

        if (isOldMan)
        {
            if (ContainsAny(playerMessage, "держи", "вот", "принес", "принёс", "нашел", "нашёл"))
            {
                return "Да... это тот самый предмет. Ты сделал доброе дело, хоть, быть может, сам еще не знаешь насколько.";
            }

            return "Вижу, ты исполнил просьбу. Осталось только вернуть вещи ее надлежащее место.";
        }

        return "Похоже, дело почти завершено. Теперь важно довести его до конца.";
    }

    private string BuildTurnedInResponse(
        DialogueContext context,
        bool isLea,
        bool isOldMan,
        string playerMessage,
        string personality,
        string speechStyle,
        string emotionalState)
    {
        if (isLea)
        {
            if (ContainsAny(playerMessage, "закончил", "всё", "все", "это все", "это всё"))
            {
                return "С этим — да. Но мне кажется, такие истории редко заканчиваются в тот же миг, как вещь возвращается на место.";
            }

            if (ContainsAny(playerMessage, "спасибо", "благодарю"))
            {
                return "И тебе спасибо. Не каждый стал бы помогать просто потому, что его попросили.";
            }

            if (ContainsAny(playerMessage, "задание", "поручение", "еще", "ещё"))
            {
                return "Пока нет. Но если здесь снова что-то сдвинется с места, я это замечу.";
            }

            return "Теперь здесь будто тише. Надеюсь, так и останется.";
        }

        if (isOldMan)
        {
            if (ContainsAny(playerMessage, "закончил", "всё", "все"))
            {
                return "На сегодня — да. Но у старых мест долгая память, и не всякая история кончается сразу.";
            }

            if (ContainsAny(playerMessage, "спасибо", "благодарю"))
            {
                return "Благодарность уместна в обе стороны, путник. Ты сделал больше, чем кажется на первый взгляд.";
            }

            if (ContainsAny(playerMessage, "задание", "поручение", "еще", "ещё"))
            {
                return "Пока молчит и лес, и камень. А когда они молчат, лучше не тревожить их напрасно.";
            }

            return "Ты исполнил просьбу, и этого я не забуду.";
        }

        return "Поручение уже завершено. Теперь можно перевести дух.";
    }

    private string BuildFallbackResponse(
        DialogueContext context,
        bool isLea,
        bool isOldMan,
        string playerMessage,
        string personality,
        string speechStyle,
        string emotionalState)
    {
        if (isLea)
        {
            if (ContainsAny(playerMessage, "привет", "здравствуй"))
                return "Здравствуй. Здесь редко встречаешь новых людей, так что я невольно присматриваюсь.";

            return "Скажи яснее. Я слушаю, просто не люблю делать выводы раньше времени.";
        }

        if (isOldMan)
        {
            if (ContainsAny(playerMessage, "привет", "здравствуй"))
                return "И тебе мирного дня, путник.";

            return "Слова порой ведут не хуже тропы. Скажи яснее, если хочешь услышать ясный ответ.";
        }

        return "Я слушаю тебя.";
    }

    private bool HasQuestItem(DialogueContext context)
    {
        if (context.inventoryItems == null || context.inventoryItems.Count == 0)
            return false;

        string questName = Normalize(context.questName);

        foreach (string item in context.inventoryItems)
        {
            string normalizedItem = Normalize(item);

            if (normalizedItem.Contains(questName) || questName.Contains(normalizedItem))
                return true;
        }

        return false;
    }

    private bool ContainsAny(string source, params string[] variants)
    {
        if (string.IsNullOrWhiteSpace(source))
            return false;

        foreach (string variant in variants)
        {
            if (source.Contains(variant))
                return true;
        }

        return false;
    }

    private string Normalize(string text)
    {
        return string.IsNullOrWhiteSpace(text)
            ? string.Empty
            : text.Trim().ToLowerInvariant();
    }
}