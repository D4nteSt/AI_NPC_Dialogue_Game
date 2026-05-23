using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayerIntentClassifier : MonoBehaviour
{
    public PlayerIntentType Classify(string playerMessage)
    {
        List<PlayerIntentType> intents = ClassifyAll(playerMessage);
        return intents.Count > 0 ? intents[0] : PlayerIntentType.None;
    }

    public List<PlayerIntentType> ClassifyAll(string playerMessage)
    {
        List<PlayerIntentType> intents = new List<PlayerIntentType>();

        if (string.IsNullOrWhiteSpace(playerMessage))
        {
            intents.Add(PlayerIntentType.None);
            return intents;
        }

        string text = Normalize(playerMessage);

        if (IsGreeting(text))
            intents.Add(PlayerIntentType.Greeting);

        if (IsReturnWithItem(text))
            intents.Add(PlayerIntentType.ReturnWithItem);

        if (IsRefuseTask(text))
            intents.Add(PlayerIntentType.RefuseTask);

        if (IsAskForReward(text))
            intents.Add(PlayerIntentType.AskForReward);

        if (IsAskForItem(text))
            intents.Add(PlayerIntentType.AskForItem);

        if (IsAcceptTask(text))
            intents.Add(PlayerIntentType.AcceptTask);

        if (IsAskAboutQuest(text))
            intents.Add(PlayerIntentType.AskAboutQuest);

        if (IsAskAboutLocation(text))
            intents.Add(PlayerIntentType.AskAboutLocation);

        if (IsConfirm(text))
            intents.Add(PlayerIntentType.Confirm);

        if (IsDecline(text))
            intents.Add(PlayerIntentType.Decline);

        if (intents.Count == 0 && IsQuestion(text))
            intents.Add(PlayerIntentType.AskQuestion);

        if (intents.Count == 0)
            intents.Add(PlayerIntentType.GeneralTalk);

        return intents;
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
            "что мне делать",
            "что дальше",
            "что делать дальше",
            "что именно делать дальше",
            "что теперь",
            "что теперь делать",
            "какой следующий шаг",
            "следующий шаг",
            "куда дальше",
            "куда идти дальше",
            "куда ведет улика",
            "куда ведет эта улика",
            "как найти виновного",
            "как нам найти виновного",
            "как выйти на виновного",
            "как определить виновного",
            "кто мог это сделать",
            "кого проверить",
            "кого допросить",
            "кого опросить",
            "с кем поговорить дальше",
            "кому могла принадлежать ткань",
            "кому принадлежит ткань",
            "у кого был доступ",
            "кто имел доступ",
            "боковой вход",
            "служебный вход",
            "вечером пропажи",
            "вечер пропажи",
            "день пропажи",
            "в день пропажи",
            "в момент пропажи",
            "где вы были",
            "где ты был",
            "вы были у архива",
            "ты был у архива",
            "вы были у бокового входа",
            "ты был у бокового входа",
            "что вы там делали",
            "что ты там делал",
            "какими делами вы были заняты",
            "какими делами ты был занят",
            "чем вы занимались",
            "чем ты занимался",
            "алиби",
            "ваше алиби",
            "твое алиби",
            "клочок ткани",
            "ткань",
            "рукав",
            "обрывок",
            "гвоздь",
            "свидетель",
            "свидетельница",
            "показания",
            "противоречия",
            "противоречивые показания",
            "вы противоречите",
            "ты противоречишь",
            "это странно",
            "подозрительно",
            "имели доступ",
            "доступ к архиву",
            "доступ к документам",
            "доступ к печати");
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
        // Прямая просьба или намерение получить предмет/документ.
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
            "хочу получить",
            "нужно получить",
            "надо получить",
            "я возьму",
            "давайте я возьму",
            "хочу взять",
            "можно взять",
            "заберу",
            "я заберу",
            "можно забрать",
            "нужен предмет",
            "нужна вещь",
            "нужен ордер",
            "нужно разрешение",
            "нужен пропуск",
            "мне нужен ордер",
            "мне нужно разрешение",
            "мне нужен пропуск",
            "получить ордер",
            "взять ордер",
            "забрать ордер",
            "возьму ордер",
            "давайте я возьму ордер",
            "что мне взять",
            "что мне нужно взять",
            "что взять с собой",
            "что мне понадобится",
            "есть что нибудь для осмотра"))
        {
            return true;
        }

        bool mentionsAccessDocument = ContainsAny(text,
            "разрешение",
            "пропуск",
            "допуск",
            "доступ",
            "документ",
            "бумагу",
            "бумаги",
            "ордер",
            "предписание",
            "разрешающий документ",
            "направление");

        bool wantsToReceive = ContainsAny(text,
            "дай",
            "дайте",
            "давай",
            "давайте",
            "выдай",
            "выдайте",
            "получить",
            "возьму",
            "заберу",
            "взять",
            "забрать",
            "нужен",
            "нужно",
            "требуется");

        if (mentionsAccessDocument && wantsToReceive)
            return true;

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
                                 "потребуется",
                                 "возьму",
                                 "заберу",
                                 "получить",
                                 "взять",
                                 "забрать");

        if (mentionsAccessDocument && (mentionsInspection || politeRequest))
            return true;

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
            "думаю это улика",
            // Для полицейского
            "вот мой ордер",
            "вот ордер",
            "вот разрешение",
            "вот моё разрешение",
            "показываю ордер",
            "предъявляю ордер",
            "показываю разрешение",
            "предъявляю разрешение",
            "у меня есть ордер",
            "у меня ордер",
            "у меня есть разрешение",
            "у меня разрешение",
            "ордер на осмотр",
            "держите ордер",
            "возьмите ордер",
            "проверьте ордер",
            "проверьте мой ордер",
            "я пришел с ордером",
            "я пришёл с разрешением",
            "держите разрешение",
            "возьмите разрешение",
            "проверьте разрешение",
            "проверьте моё разрешение",
            "я пришел с разрешением",
            "я пришёл с разрешением");
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