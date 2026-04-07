using UnityEngine;

public class NPCDialogueData : MonoBehaviour
{
    [Header("Base Identity")]
    [SerializeField] private string npcName = "Старик";
    [SerializeField] private string npcRole = "Хранитель одинокой сторожки у древних руин";

    [Header("Personality and Speech")]
    [SerializeField] private string npcPersonality = "Мудрый, спокойный, наблюдательный, немного загадочный";
    [SerializeField] private string npcSpeechStyle = "Говорит размеренно, короткими, вдумчивыми фразами, иногда выражается образно";

    [Header("Background and Place")]
    [SerializeField] private string npcBackstory = "Много лет живет рядом с руинами и наблюдает за тем, как люди приходят и уходят";
    [SerializeField] private string npcLocationContext = "Считает это место важным и не любит, когда к древним вещам относятся легкомысленно";

    [Header("Knowledge Boundaries")]
    [SerializeField] private string npcKnowledge = "Знает местность, историю руин, значение артефакта и слухи о прошлом этого места";
    [SerializeField] private string npcUnknowns = "Не знает мыслей игрока, событий за пределами этой местности и вещей, которых не видел сам";

    [Header("Motivation and Hidden Layer")]
    [SerializeField] private string npcMotivation = "Хочет вернуть артефакт и сохранить хрупкое равновесие этого места";
    [SerializeField] private string npcSecret = "Скрывает, что артефакт может быть связан не только с прошлым руин, но и с его собственной историей";

    [Header("Relationship")]
    [SerializeField] private string npcAttitudeToPlayer = "Сначала относится к игроку осторожно, но при уважительном поведении начинает доверять больше";

    [Header("Conversation Dynamics")]
    [SerializeField] private string npcCurrentEmotionalState = "Спокоен и собран, но внутренне напряжен из-за возвращения артефакта";
    [SerializeField] private string npcConversationTendency = "Говорит неторопливо, осторожно и не раскрывает всего сразу, предпочитая намеки прямым объяснениям";

    [Header("Quest Role")]
    [SerializeField] private bool isQuestGiver = true;

    [Header("Dialogue")]
    [SerializeField] private string greetingMessage = "Здравствуй, путник.";
    [SerializeField] private QuestData questData;

    public string NPCName => npcName;
    public string NPCRole => npcRole;
    public string NPCPersonality => npcPersonality;
    public string NPCSpeechStyle => npcSpeechStyle;
    public string NPCBackstory => npcBackstory;
    public string NPCLocationContext => npcLocationContext;
    public string NPCKnowledge => npcKnowledge;
    public string NPCUnknowns => npcUnknowns;
    public string NPCMotivation => npcMotivation;
    public string NPCSecret => npcSecret;
    public string NPCAttitudeToPlayer => npcAttitudeToPlayer;
    public string NPCCurrentEmotionalState => npcCurrentEmotionalState;
    public string NPCConversationTendency => npcConversationTendency;
    public bool IsQuestGiver => isQuestGiver;
    public string GreetingMessage => greetingMessage;
    public QuestData QuestData => questData;
}