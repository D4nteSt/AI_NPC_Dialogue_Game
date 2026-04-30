using System;
using System.Linq;
using UnityEngine;

public class PlayerIntentClassifier : MonoBehaviour
{
    public PlayerIntentType Classify(string playerMessage)
    {
        if (string.IsNullOrWhiteSpace(playerMessage))
            return PlayerIntentType.None;

        string text = Normalize(playerMessage);

        if (IsGreeting(text))
            return PlayerIntentType.Greeting;

        if (IsAcceptTask(text))
            return PlayerIntentType.AcceptTask;

        if (IsRefuseTask(text))
            return PlayerIntentType.RefuseTask;

        if (IsAskAboutQuest(text))
            return PlayerIntentType.AskAboutQuest;

        if (IsAskAboutLocation(text))
            return PlayerIntentType.AskAboutLocation;

        if (IsAskForReward(text))
            return PlayerIntentType.AskForReward;

        if (IsAskForItem(text))
            return PlayerIntentType.AskForItem;

        if (IsReturnWithItem(text))
            return PlayerIntentType.ReturnWithItem;

        if (IsConfirm(text))
            return PlayerIntentType.Confirm;

        if (IsDecline(text))
            return PlayerIntentType.Decline;

        if (IsQuestion(text))
            return PlayerIntentType.AskQuestion;

        return PlayerIntentType.GeneralTalk;
    }

    private string Normalize(string input)
    {
        return input.Trim().ToLowerInvariant();
    }

    private bool ContainsAny(string text, params string[] variants)
    {
        return variants.Any(v => text.Contains(v));
    }

    private bool EqualsAny(string text, params string[] variants)
    {
        return variants.Any(v => text.Equals(v, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsGreeting(string text)
    {
        return EqualsAny(text, "привет", "здравствуй", "здравствуйте", "добрый день", "доброго дня")
               || ContainsAny(text, "привет", "здравствуй", "здравствуйте");
    }

    private bool IsAcceptTask(string text)
    {
        return ContainsAny(text,
            "я помогу",
            "я согласен",
            "согласен",
            "хорошо, я сделаю",
            "ладно, я сделаю",
            "берусь",
            "готов помочь",
            "я возьмусь",
            "я выполню",
            "давай",
            "хорошо");
    }

    private bool IsRefuseTask(string text)
    {
        return ContainsAny(text,
            "не буду",
            "не хочу",
            "отказываюсь",
            "я не согласен",
            "не согласен",
            "нет, не буду",
            "не стану");
    }

    private bool IsAskAboutQuest(string text)
    {
        return ContainsAny(text,
            "задание",
            "поручение",
            "что нужно сделать",
            "что ты хочешь",
            "какая помощь нужна",
            "чем помочь",
            "в чем проблема");
    }

    private bool IsAskAboutLocation(string text)
    {
        return ContainsAny(text,
            "где",
            "куда",
            "как найти",
            "в какой стороне",
            "где это",
            "где искать",
            "где находятся");
    }

    private bool IsAskForReward(string text)
    {
        return ContainsAny(text,
            "награда",
            "что я получу",
            "что мне за это будет",
            "чем заплатишь",
            "какая будет награда",
            "что дашь");
    }

    private bool IsAskForItem(string text)
    {
        return ContainsAny(text,
            "дай мне",
            "можешь дать",
            "нужен предмет",
            "что мне взять",
            "что мне нужно для этого",
            "ты можешь дать мне");
    }

    private bool IsReturnWithItem(string text)
    {
        return ContainsAny(text,
            "я принес",
            "вот он",
            "вот предмет",
            "я нашел",
            "я достал",
            "держи",
            "забирай",
            "это то что ты просил");
    }

    private bool IsConfirm(string text)
    {
        return EqualsAny(text, "да", "ага", "угу", "понял", "хорошо", "ладно", "ясно")
               || ContainsAny(text, "я понял", "хорошо", "ладно", "ясно");
    }

    private bool IsDecline(string text)
    {
        return EqualsAny(text, "нет", "неа")
               || ContainsAny(text, "не думаю", "не хочу", "не надо", "не буду");
    }

    private bool IsQuestion(string text)
    {
        return text.Contains("?") || ContainsAny(text,
            "кто", "что", "почему", "зачем", "как", "где", "когда");
    }
}