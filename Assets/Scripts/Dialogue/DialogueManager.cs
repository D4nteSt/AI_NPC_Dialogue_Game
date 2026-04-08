using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private DialogueContextBuilder contextBuilder;
    [SerializeField] private NPCAsyncResponseService responseService;

    private NPCDialogueData currentNPC;
    private bool isDialogueOpen;
    private bool isWaitingForResponse;

    private List<string> dialogueHistory = new List<string>();

    public bool IsDialogueOpen => isDialogueOpen;
    public bool IsWaitingForResponse => isWaitingForResponse;

    public void StartDialogue(NPCDialogueData npcData)
    {
        if (npcData == null) return;

        currentNPC = npcData;
        isDialogueOpen = true;
        isWaitingForResponse = false;
        dialogueHistory.Clear();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        dialogueUI.Show();
        dialogueUI.SetNPCName(currentNPC.NPCName);
        dialogueUI.ClearHistory();
        dialogueUI.SetInputInteractable(true);

        string startMessage = GetNPCStartMessage(currentNPC);
        AddDialogueLine(currentNPC.NPCName + ": " + startMessage);
        dialogueUI.AddMessage(currentNPC.NPCName, startMessage);
        dialogueUI.ClearInput();
    }

    public async Task SendPlayerMessageAsync(string playerMessage)
    {
        if (!isDialogueOpen || currentNPC == null) return;
        if (isWaitingForResponse) return;
        if (string.IsNullOrWhiteSpace(playerMessage)) return;

        isWaitingForResponse = true;
        dialogueUI.SetInputInteractable(false);

        dialogueUI.AddMessage("Игрок", playerMessage);
        AddDialogueLine("Игрок: " + playerMessage);

        HandleQuestLogicBeforeResponse();

        DialogueContext context = contextBuilder.BuildContext(
            currentNPC,
            playerMessage,
            dialogueHistory
        );

        dialogueUI.AddMessage(currentNPC.NPCName, "...");
        int thinkingMessageIndex = dialogueHistory.Count;
        AddDialogueLine(currentNPC.NPCName + ": ...");

        string npcResponse = await responseService.GetResponseAsync(context);

        HandleQuestLogicAfterResponse();

        ReplaceLastDialogueUIMessage(currentNPC.NPCName, npcResponse);
        ReplaceLastDialogueHistoryLine(currentNPC.NPCName + ": " + npcResponse);

        dialogueUI.ClearInput();
        dialogueUI.SetInputInteractable(true);

        isWaitingForResponse = false;
    }

    public void CloseDialogue()
    {
        currentNPC = null;
        isDialogueOpen = false;
        isWaitingForResponse = false;
        dialogueHistory.Clear();

        dialogueUI.Hide();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void AddDialogueLine(string line)
    {
        dialogueHistory.Add(line);
    }

    private void ReplaceLastDialogueHistoryLine(string newLine)
    {
        if (dialogueHistory.Count == 0) return;
        dialogueHistory[dialogueHistory.Count - 1] = newLine;
    }

    private void ReplaceLastDialogueUIMessage(string sender, string message)
    {
        // Упрощенный вариант: пересобираем весь текст из истории
        dialogueUI.ClearHistory();

        ReplaceLastDialogueHistoryLine(sender + ": " + message);

        foreach (string line in dialogueHistory)
        {
            int separatorIndex = line.IndexOf(": ");
            if (separatorIndex > 0)
            {
                string lineSender = line.Substring(0, separatorIndex);
                string lineMessage = line.Substring(separatorIndex + 2);
                dialogueUI.AddMessage(lineSender, lineMessage);
            }
        }
    }

    private string GetNPCStartMessage(NPCDialogueData npcData)
    {
        if (npcData == null)
            return string.Empty;

        if (npcData.QuestData == null || questManager == null)
            return npcData.GreetingMessage;

        questManager.RegisterQuest(npcData.QuestData);

        QuestStatus status = questManager.GetQuestStatus(npcData.QuestData.questId);

        if (npcData.IsQuestGiver)
        {
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
        else
        {
            switch (status)
            {
                case QuestStatus.NotStarted:
                    return npcData.GreetingMessage;

                case QuestStatus.InProgress:
                    questManager.CheckQuestProgress(npcData.QuestData.questId);

                    if (questManager.GetQuestStatus(npcData.QuestData.questId) == QuestStatus.Completed)
                        return "Похоже, ты уже нашел нужную вещь. Тогда лучше не мешкать.";

                    return npcData.GreetingMessage;

                case QuestStatus.Completed:
                    return "Похоже, дело уже почти закончено.";

                case QuestStatus.TurnedIn:
                    return "Кажется, здесь стало спокойнее.";

                default:
                    return npcData.GreetingMessage;
            }
        }
    }

    private void HandleQuestLogicBeforeResponse()
    {
        if (currentNPC == null || currentNPC.QuestData == null || questManager == null)
            return;

        if (!currentNPC.IsQuestGiver)
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

        if (!currentNPC.IsQuestGiver)
            return;

        string questId = currentNPC.QuestData.questId;
        QuestStatus status = questManager.GetQuestStatus(questId);

        if (status == QuestStatus.Completed)
        {
            questManager.TurnInQuest(questId);
        }
    }
}