using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayerIntentClassifier : MonoBehaviour
{
    public PlayerIntentType Classify(string playerMessage)
    {
        if (string.IsNullOrWhiteSpace(playerMessage))
            return PlayerIntentType.None;

        string text = Normalize(playerMessage);

        // Важно: более конкретные намерения проверяем раньше общих.
        if (IsGreeting(text))
            return PlayerIntentType.Greeting;

        if (IsReturnWithItem(text))
            return PlayerIntentType.ReturnWithItem;

        if (IsRefuseTask(text))
            return PlayerIntentType.RefuseTask;

        if (IsAcceptTask(text))
            return PlayerIntentType.AcceptTask;

        if (IsAskForReward(text))
            return PlayerIntentType.AskForReward;

        if (IsAskForItem(text))
            return PlayerIntentType.AskForItem;

        if (IsAskAboutQuest(text))
            return PlayerIntentType.AskAboutQuest;

        if (IsAskAboutLocation(text))
            return PlayerIntentType.AskAboutLocation;

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
        string text = input.Trim().ToLowerInvariant();
        text = text.Replace('ё', 'е');

        // Заменяем знаки препинания пробелами, чтобы фразы легче ловились.
        text = Regex.Replace(text, @"[^\p{L}\p{N}\s]", " ");

        // Схлопываем несколько пробелов в один.
        text = Regex.Replace(text, @"\s+", " ");

        return text.Trim();
    }

    private bool ContainsAny(string text, params string[] variants)
    {
        return variants.Any(v => text.Contains(v));
    }

    private bool ContainsAll(string text, params string[] variants)
    {
        return variants.All(v => text.Contains(v));
    }

    private bool EqualsAny(string text, params string[] variants)
    {
        return variants.Any(v => text.Equals(v, StringComparison.OrdinalIgnoreCase));
    }

    private bool HasQuestionForm(string text)
    {
        return ContainsAny(text,
            "можно",
            "могу ли",
            "мог бы",
            "могли бы",
            "не могли бы",
            "позволите",
            "разрешите",
            "вы можете",
            "можете",
            "есть ли возможность",
            "мне нужно",
            "мне нужен",
            "мне нужна",
            "мне нужны",
            "я могу",
            "как мне");
    }

    private bool IsGreeting(string text)
    {
        return EqualsAny(text,
                   "привет",
                   "здравствуй",
                   "здравствуйте",
                   "добрый день",
                   "доброго дня",
                   "добрый вечер")
               || ContainsAny(text,
                   "привет",
                   "здравствуй",
                   "здравствуйте",
                   "добрый день",
                   "добрый вечер");
    }

    private bool IsAcceptTask(string text)
    {
        // Не считаем любое "хорошо" согласием на квест.
        // Лучше требовать смысл помощи/участия/начала дела.
        return ContainsAny(text,
            "я помогу",
            "готов помочь",
            "я готов помочь",
            "я согласен помочь",
            "согласен помочь",
            "берусь за дело",
            "я берусь",
            "я возьмусь",
            "я займусь этим",
            "я займусь делом",
            "я выполню",
            "я сделаю",
            "можете рассчитывать на меня",
            "попробую разобраться",
            "я осмотрю место",
            "я проверю",
            "я расследую",
            "давайте начнем",
            "давайте приступим",
            "начнем расследование",
            "приступим к делу",
            "я готов");
    }

    private bool IsRefuseTask(string text)
    {
        return ContainsAny(text,
            "не буду",
            "не хочу",
            "отказываюсь",
            "я отказываюсь",
            "я не согласен",
            "не согласен",
            "нет не буду",
            "не стану",
            "это не мое дело",
            "разбирайтесь сами",
            "не хочу вмешиваться");
    }

    private bool IsAskAboutQuest(string text)
    {
        return ContainsAny(text,
            "что случилось",
            "что произошло",
            "в чем дело",
            "какое дело",
            "что за дело",
            "расскажите о деле",
            "что нужно сделать",
            "что от меня требуется",
            "какая помощь нужна",
            "чем помочь",
            "в чем проблема",
            "что пропало",
            "кого подозреваете",
            "есть подозреваемые",
            "что известно",
            "какие факты",
            "какие обстоятельства",
            "с чего начать",
            "что мне делать");
    }

    private bool IsAskAboutLocation(string text)
    {
        // Вопросы о месте, но не о пропуске/разрешении.
        if (ContainsAny(text, "разрешение", "пропуск", "допуск", "доступ"))
            return false;

        return ContainsAny(text,
            "где",
            "куда",
            "как найти",
            "в какой стороне",
            "где это",
            "где искать",
            "где находится",
            "где находятся",
            "куда идти",
            "куда мне идти",
            "где место преступления",
            "где архив",
            "где склад",
            "где кабинет",
            "где осмотреть");
    }

    private bool IsAskForReward(string text)
    {
        return ContainsAny(text,
            "награда",
            "вознаграждение",
            "плата",
            "оплата",
            "что я получу",
            "что мне за это будет",
            "чем заплатите",
            "чем заплатишь",
            "какая будет награда",
            "что дадите за помощь",
            "будет ли награда");
    }

    private bool IsAskForItem(string text)
    {
        // Прямая просьба выдать предмет.
        if (ContainsAny(text,
            "дай мне",
            "дайте мне",
            "можешь дать",
            "можете дать",
            "выдайте",
            "выдать мне",
            "получить у вас",
            "можно получить",
            "могу получить",
            "нужен предмет",
            "нужна вещь",
            "что мне взять",
            "что мне нужно взять",
            "что взять с собой",
            "что мне понадобится",
            "есть что нибудь для осмотра"))
        {
            return true;
        }

        // Детективный случай: игрок просит разрешение/пропуск/доступ.
        bool mentionsAccessDocument = ContainsAny(text,
            "разрешение",
            "пропуск",
            "допуск",
            "доступ",
            "документ",
            "бумагу",
            "бумаги",
            "ордер",
            "направление");

        bool mentionsInspection = ContainsAny(text,
            "осмотр",
            "осмотреть",
            "осмотра",
            "место преступления",
            "архив",
            "склад",
            "кабинет",
            "пройти",
            "пропустили",
            "пустили",
            "войти",
            "допустили");

        bool politeRequest = HasQuestionForm(text)
                             || ContainsAny(text,
                                 "попросить",
                                 "хотел бы попросить",
                                 "хотелось бы получить",
                                 "необходимо получить",
                                 "понадобится",
                                 "потребуется");

        if (mentionsAccessDocument && (mentionsInspection || politeRequest))
            return true;

        // Просьба о конкретном полезном предмете.
        bool mentionsTool = ContainsAny(text,
            "фонарь",
            "ключ",
            "записка",
            "улика",
            "перчатки",
            "лампа",
            "инструмент",
            "средство",
            "предмет");

        if (mentionsTool && politeRequest)
            return true;

        return false;
    }

    private bool IsReturnWithItem(string text)
    {
        return ContainsAny(text,
            "я принес",
            "я принесла",
            "принес улику",
            "принес улики",
            "я нашел",
            "я нашла",
            "я обнаружил",
            "я обнаружила",
            "у меня есть улика",
            "у меня есть доказательство",
            "вот улика",
            "вот доказательство",
            "вот обрывок",
            "вот ткань",
            "вот он",
            "вот она",
            "держите",
            "держи",
            "забирайте",
            "это то что вы искали",
            "это то что вы просили",
            "посмотрите на это",
            "взгляните на это",
            "думаю это важно",
            "думаю это улика");
    }

    private bool IsConfirm(string text)
    {
        return EqualsAny(text,
                   "да",
                   "ага",
                   "угу",
                   "понял",
                   "поняла",
                   "хорошо",
                   "ладно",
                   "ясно",
                   "разумеется",
                   "конечно")
               || ContainsAny(text,
                   "я понял",
                   "я поняла",
                   "хорошо понял",
                   "хорошо я понял",
                   "ладно понял",
                   "ясно");
    }

    private bool IsDecline(string text)
    {
        return EqualsAny(text, "нет", "неа")
               || ContainsAny(text,
                   "не думаю",
                   "не хочу",
                   "не надо",
                   "не буду",
                   "не уверен",
                   "не уверена",
                   "вряд ли");
    }

    private bool IsQuestion(string text)
    {
        return ContainsAny(text,
            "кто",
            "что",
            "почему",
            "зачем",
            "как",
            "где",
            "когда",
            "можно",
            "могу ли",
            "могли бы",
            "можете ли");
    }
}