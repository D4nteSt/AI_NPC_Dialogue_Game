using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private QuestManager questManager;

    private NPCDialogueData currentNPC;
    private bool isDialogueOpen;

    public bool IsDialogueOpen => isDialogueOpen;

    public void StartDialogue(NPCDialogueData npcData)
    {
        if (npcData == null) return;

        currentNPC = npcData;
        isDialogueOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        dialogueUI.Show();
        dialogueUI.SetNPCName(currentNPC.NPCName);
        dialogueUI.ClearHistory();

        string startMessage = GetNPCStartMessage(currentNPC);
        dialogueUI.AddMessage(currentNPC.NPCName, startMessage);
        dialogueUI.ClearInput();
    }

    public void SendPlayerMessage(string playerMessage)
    {
        if (!isDialogueOpen || currentNPC == null) return;
        if (string.IsNullOrWhiteSpace(playerMessage)) return;

        dialogueUI.AddMessage("Игрок", playerMessage);

        string npcResponse = GenerateResponse(playerMessage);
        dialogueUI.AddMessage(currentNPC.NPCName, npcResponse);

        dialogueUI.ClearInput();
    }

    public void CloseDialogue()
    {
        currentNPC = null;
        isDialogueOpen = false;

        dialogueUI.Hide();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private string GetNPCStartMessage(NPCDialogueData npcData)
    {
        if (npcData.QuestData == null || questManager == null)
            return npcData.GreetingMessage;

        questManager.RegisterQuest(npcData.QuestData);

        QuestStatus status = questManager.GetQuestStatus(npcData.QuestData.questId);

        switch (status)
        {
            case QuestStatus.NotStarted:
                return npcData.GreetingMessage + " У меня есть для тебя задание.";

            case QuestStatus.InProgress:
                questManager.CheckQuestProgress(npcData.QuestData.questId);

                if (questManager.GetQuestStatus(npcData.QuestData.questId) == QuestStatus.Completed)
                    return "Ты нашел то, что я просил? Похоже, да.";

                return "Ты уже нашел нужный предмет?";

            case QuestStatus.Completed:
                return "Отлично! Ты принес нужный предмет.";

            case QuestStatus.TurnedIn:
                return "Спасибо за помощь. Ты хорошо справился.";

            default:
                return npcData.GreetingMessage;
        }
    }

    private string GenerateResponse(string playerMessage)
    {
        if (currentNPC.QuestData == null || questManager == null)
            return "Я услышал тебя: \"" + playerMessage + "\"";

        string questId = currentNPC.QuestData.questId;
        QuestStatus status = questManager.GetQuestStatus(questId);

        if (status == QuestStatus.NotStarted)
        {
            questManager.StartQuest(questId);
            return "Прошу тебя, найди и принеси мне предмет: " + currentNPC.QuestData.questName;
        }

        if (status == QuestStatus.InProgress)
        {
            questManager.CheckQuestProgress(questId);

            if (questManager.GetQuestStatus(questId) == QuestStatus.Completed)
            {
                questManager.TurnInQuest(questId);
                return "Прекрасно! Ты выполнил мое поручение.";
            }

            return "Я все еще жду предмет.";
        }

        if (status == QuestStatus.Completed)
        {
            questManager.TurnInQuest(questId);
            return "Прекрасно! Ты выполнил мое поручение.";
        }

        if (status == QuestStatus.TurnedIn)
        {
            return "У меня пока нет новых поручений.";
        }

        return "Я услышал тебя: \"" + playerMessage + "\"";
    }
}