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

        string trimmedPlayerMessage = playerMessage.Trim();

        // 1. Игрок: добавить в историю и UI только один раз
        AddDialogueLine("Игрок: " + trimmedPlayerMessage);
        RebuildDialogueUIFromHistory();

        // 2. Временно показать, что NPC думает (только в UI)
        dialogueUI.AddMessage(currentNPC.NPCName, "...");

        HandleQuestLogicBeforeResponse();

        DialogueContext context = contextBuilder.BuildContext(
            currentNPC,
            trimmedPlayerMessage,
            dialogueHistory
        );

        string npcResponse = await responseService.GetResponseAsync(context);

        HandleQuestLogicAfterResponse();

        bool isTechnicalError = IsTechnicalMessage(npcResponse);

        // 3. Если ответ нормальный — добавить его в историю
        if (!isTechnicalError)
        {
            AddDialogueLine(currentNPC.NPCName + ": " + npcResponse);
        }

        // 4. Пересобрать UI из чистой истории
        RebuildDialogueUIFromHistory();

        // 5. Если это техошибка — показать отдельно, но не сохранять в историю
        if (isTechnicalError)
        {
            dialogueUI.AddMessage("Система", npcResponse);
        }

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
        if (string.IsNullOrWhiteSpace(line))
            return;

        dialogueHistory.Add(line.Trim());
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

    private bool IsTechnicalMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return true;

        string normalized = message.Trim();

        return normalized.StartsWith("Ошибка AI:") ||
               normalized.StartsWith("Ошибка HTTP:") ||
               normalized.StartsWith("Ошибка:") ||
               normalized == "...";
    }

    private void RebuildDialogueUIFromHistory()
    {
        dialogueUI.ClearHistory();

        foreach (string line in dialogueHistory)
        {
            int separatorIndex = line.IndexOf(": ");
            if (separatorIndex <= 0)
                continue;

            string sender = line.Substring(0, separatorIndex);
            string message = line.Substring(separatorIndex + 2);

            dialogueUI.AddMessage(sender, message);
        }
    }

}