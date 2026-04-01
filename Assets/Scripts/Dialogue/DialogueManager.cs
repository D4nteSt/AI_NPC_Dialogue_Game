using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private DialogueContextBuilder contextBuilder;
    [SerializeField] private NPCResponseService responseService;

    private NPCDialogueData currentNPC;
    private bool isDialogueOpen;

    private List<string> dialogueHistory = new List<string>();

    public bool IsDialogueOpen => isDialogueOpen;

    public void StartDialogue(NPCDialogueData npcData)
    {
        if (npcData == null) return;

        currentNPC = npcData;
        isDialogueOpen = true;
        dialogueHistory.Clear();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        dialogueUI.Show();
        dialogueUI.SetNPCName(currentNPC.NPCName);
        dialogueUI.ClearHistory();

        string startMessage = GetNPCStartMessage(currentNPC);
        AddDialogueLine(currentNPC.NPCName + ": " + startMessage);
        dialogueUI.AddMessage(currentNPC.NPCName, startMessage);
        dialogueUI.ClearInput();
    }

    public void SendPlayerMessage(string playerMessage)
    {
        if (!isDialogueOpen || currentNPC == null) return;
        if (string.IsNullOrWhiteSpace(playerMessage)) return;

        dialogueUI.AddMessage("Игрок", playerMessage);
        AddDialogueLine("Игрок: " + playerMessage);

        HandleQuestLogicBeforeResponse();

        DialogueContext context = contextBuilder.BuildContext(
            currentNPC,
            playerMessage,
            dialogueHistory
        );

        string npcResponse = responseService.GetResponse(context);

        HandleQuestLogicAfterResponse();

        dialogueUI.AddMessage(currentNPC.NPCName, npcResponse);
        AddDialogueLine(currentNPC.NPCName + ": " + npcResponse);

        dialogueUI.ClearInput();
    }

    public void CloseDialogue()
    {
        currentNPC = null;
        isDialogueOpen = false;
        dialogueHistory.Clear();

        dialogueUI.Hide();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void AddDialogueLine(string line)
    {
        dialogueHistory.Add(line);
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

    private void HandleQuestLogicBeforeResponse()
    {
        if (currentNPC == null || currentNPC.QuestData == null || questManager == null)
            return;

        string questId = currentNPC.QuestData.questId;
        QuestStatus status = questManager.GetQuestStatus(questId);

        if (status == QuestStatus.NotStarted)
        {
            questManager.StartQuest(questId);
            return;
        }

        if (status == QuestStatus.InProgress)
        {
            questManager.CheckQuestProgress(questId);
        }
    }

    private void HandleQuestLogicAfterResponse()
    {
        if (currentNPC == null || currentNPC.QuestData == null || questManager == null)
            return;

        string questId = currentNPC.QuestData.questId;
        QuestStatus status = questManager.GetQuestStatus(questId);

        if (status == QuestStatus.Completed)
        {
            questManager.TurnInQuest(questId);
        }
    }
}