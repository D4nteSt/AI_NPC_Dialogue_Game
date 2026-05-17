using UnityEngine;

public class NPCDialogueData : MonoBehaviour
{
    [Header("Base Identity")]
    [SerializeField] private string npcName = "";
    [SerializeField] private string npcRole = "";

    [Header("Personality and Speech")]
    [SerializeField] private string npcPersonality = "";
    [SerializeField] private string npcSpeechStyle = "";

    [Header("Background and Place")]
    [SerializeField] private string npcBackstory = "";
    [SerializeField] private string npcLocationContext = "";

    [Header("Knowledge Boundaries")]
    [SerializeField] private string npcKnowledge = "";
    [SerializeField] private string npcUnknowns = "";

    [Header("Motivation and Hidden Layer")]
    [SerializeField] private string npcMotivation = "";
    [SerializeField] private string npcSecret = "";

    [Header("Relationship")]
    [SerializeField] private string npcAttitudeToPlayer = "";

    [Header("Conversation Dynamics")]
    [SerializeField] private string npcCurrentEmotionalState = "";
    [SerializeField] private string npcConversationTendency = "";

    [Header("Quest Role")]
    [SerializeField] private bool isQuestGiver = true;

    [Header("Dialogue")]
    [SerializeField] private string greetingMessage = "";
    [SerializeField] private QuestData questData;
    [SerializeField] private QuestData[] additionalQuestData;

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
    public QuestData[] AdditionalQuestData => additionalQuestData;

    public System.Collections.Generic.List<QuestData> GetAllQuestData()
    {
        System.Collections.Generic.List<QuestData> result = new System.Collections.Generic.List<QuestData>();

        if (questData != null)
            result.Add(questData);

        if (additionalQuestData != null)
        {
            foreach (QuestData data in additionalQuestData)
            {
                if (data != null && !result.Contains(data))
                    result.Add(data);
            }
        }

        return result;
    }
}