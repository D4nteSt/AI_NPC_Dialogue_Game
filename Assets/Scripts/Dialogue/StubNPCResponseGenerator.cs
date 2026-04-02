using System;
using System.Linq;
using UnityEngine;

public class StubNPCResponseGenerator : MonoBehaviour, INPCResponseGenerator
{
    public string GenerateResponse(DialogueContext context)
    {
        if (context == null)
            return "Мне трудно ответить, я будто потерял нить разговора.";

        string status = context.questStatus ?? string.Empty;
        string playerMessage = Normalize(context.playerMessage);
        string personalityStyle = GetPersonalityPrefix(context.npcPersonality);

        switch (status)
        {
            case "NotStarted":
                return BuildNotStartedResponse(context, personalityStyle, playerMessage);

            case "InProgress":
                return BuildInProgressResponse(context, personalityStyle, playerMessage);

            case "Completed":
                return BuildCompletedResponse(context, personalityStyle, playerMessage);

            case "TurnedIn":
                return BuildTurnedInResponse(context, personalityStyle, playerMessage);

            default:
                return BuildFallbackResponse(context, personalityStyle, playerMessage);
        }
    }

    private string BuildNotStartedResponse(DialogueContext context, string personalityStyle, string playerMessage)
    {
        if (ContainsAny(playerMessage, "привет", "здравствуй", "добрый день"))
        {
            return personalityStyle + " Приветствую тебя. У меня есть просьба: отыщи для меня " +
                   SafeQuestName(context) + ".";
        }

        if (ContainsAny(playerMessage, "что", "зачем", "нужно", "помочь", "задание"))
        {
            return personalityStyle + " Мне нужен " + SafeQuestName(context) +
                   ". " + SafeQuestDescription(context) + ".";
        }

        return personalityStyle + " Если ты готов помочь, найди для меня " +
               SafeQuestName(context) + ". " + SafeQuestDescription(context) + ".";
    }

    private string BuildInProgressResponse(DialogueContext context, string personalityStyle, string playerMessage)
    {
        bool hasQuestItem = HasQuestItem(context);

        if (hasQuestItem)
        {
            return personalityStyle + " Я чувствую, что нужная вещь уже у тебя. Покажи мне " +
                   SafeQuestName(context) + ".";
        }

        if (ContainsAny(playerMessage, "ищу", "ищем", "поиск", "ищется"))
        {
            return personalityStyle + " Продолжай поиски. " + SafeQuestName(context) +
                   " должен быть где-то поблизости.";
        }

        if (ContainsAny(playerMessage, "где", "куда", "найти"))
        {
            return personalityStyle + " Осмотрись внимательнее вокруг. Такие вещи редко лежат на виду, " +
                   "но и бесследно не исчезают.";
        }

        if (ContainsAny(playerMessage, "не могу", "не нашел", "не нашёл", "сложно"))
        {
            return personalityStyle + " Не спеши. Терпение и внимание приведут тебя к цели.";
        }

        return personalityStyle + " Я все еще жду " + SafeQuestName(context) +
               ". Пока он не найден, наше дело не завершено.";
    }

    private string BuildCompletedResponse(DialogueContext context, string personalityStyle, string playerMessage)
    {
        if (ContainsAny(playerMessage, "держи", "вот", "принес", "принёс", "нашел", "нашёл"))
        {
            return personalityStyle + " Да, это именно то, что было нужно. Ты хорошо справился.";
        }

        return personalityStyle + " Вижу, ты выполнил мою просьбу. Передай мне " +
               SafeQuestName(context) + ", и дело будет завершено.";
    }

    private string BuildTurnedInResponse(DialogueContext context, string personalityStyle, string playerMessage)
    {
        if (ContainsAny(playerMessage, "спасибо", "благодарю"))
        {
            return personalityStyle + " И тебе спасибо. Не каждый готов довести дело до конца.";
        }

        if (ContainsAny(playerMessage, "еще", "ещё", "задание", "работа", "поручение"))
        {
            return personalityStyle + " Пока у меня нет нового поручения, но, возможно, позже оно появится.";
        }

        return personalityStyle + " Ты уже помог мне, и я этого не забуду.";
    }

    private string BuildFallbackResponse(DialogueContext context, string personalityStyle, string playerMessage)
    {
        if (ContainsAny(playerMessage, "привет", "здравствуй"))
        {
            return personalityStyle + " Приветствую тебя, путник.";
        }

        return personalityStyle + " Я услышал тебя, но мне нужно больше ясности, чтобы ответить точнее.";
    }

    private string GetPersonalityPrefix(string personality)
    {
        string normalized = Normalize(personality);

        if (normalized.Contains("мудр"))
            return "Старик задумчиво кивает.";

        if (normalized.Contains("строг"))
            return "Старик смотрит на тебя строго.";

        if (normalized.Contains("добр"))
            return "Старик мягко улыбается.";

        if (normalized.Contains("загадоч"))
            return "Старик говорит тихо и с прищуром.";

        return "Старик отвечает спокойно.";
    }

    private bool HasQuestItem(DialogueContext context)
    {
        if (context.inventoryItems == null || context.inventoryItems.Count == 0)
            return false;

        string questName = Normalize(context.questName);

        return context.inventoryItems.Any(item =>
            Normalize(item).Contains(questName) || questName.Contains(Normalize(item)));
    }

    private string SafeQuestName(DialogueContext context)
    {
        return string.IsNullOrWhiteSpace(context.questName) ? "нужный предмет" : context.questName;
    }

    private string SafeQuestDescription(DialogueContext context)
    {
        return string.IsNullOrWhiteSpace(context.questDescription)
            ? "Найди и принеси его мне"
            : context.questDescription;
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