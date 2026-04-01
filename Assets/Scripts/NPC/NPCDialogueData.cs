using UnityEngine;

public class NPCDialogueData : MonoBehaviour
{
    [SerializeField] private string npcName = "Старик";
    [SerializeField] private string npcPersonality = "Мудрый, спокойный, немного загадочный старик.";
    [SerializeField] private string greetingMessage = "Здравствуй, путник.";
    [SerializeField] private QuestData questData;

    public string NPCName => npcName;
    public string NPCPersonality => npcPersonality;
    public string GreetingMessage => greetingMessage;
    public QuestData QuestData => questData;
}