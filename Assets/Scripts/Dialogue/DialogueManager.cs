using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private DialogueUI dialogueUI;          // только show/hide панели
    [SerializeField] private DialogueChatUI chatUI;          // новый чат-бокс

    [Header("Systems")]
    [SerializeField] private QuestManager questManager;
    [SerializeField] private DialogueContextBuilder contextBuilder;
    [SerializeField] private NPCAsyncResponseService responseService;

    private NPCDialogueData currentNPC;
    private bool isDialogueOpen;
    private bool isWaitingForResponse;

    private readonly List<string> dialogueHistory = new();

    public bool IsDialogueOpen => isDialogueOpen;
    public bool IsWaitingForResponse => isWaitingForResponse;

    private void OnEnable()
    {
        if (chatUI != null)
            chatUI.PlayerMessageSubmitted += HandlePlayerMessageSubmitted;
    }

    private void OnDisable()
    {
        if (chatUI != null)
            chatUI.PlayerMessageSubmitted -= HandlePlayerMessageSubmitted;
    }

    public void StartDialogue(NPCDialogueData npcData)
    {
        if (npcData == null) return;

        currentNPC = npcData;
        isDialogueOpen = true;
        isWaitingForResponse = false;
        dialogueHistory.Clear();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (dialogueUI != null)
            dialogueUI.Show();

        if (chatUI != null)
        {
            chatUI.SetNpcName(currentNPC.NPCName);
            chatUI.ClearMessages();
            chatUI.SetInputInteractable(true);
            chatUI.ClearInput();
            chatUI.FocusInput();
        }

        string startMessage = GetNPCStartMessage(currentNPC);
        AddDialogueLine(currentNPC.NPCName + ": " + startMessage);

        if (chatUI != null)
            chatUI.AddNpcMessage(startMessage);
    }

    private async void HandlePlayerMessageSubmitted(string playerMessage)
    {
        await SendPlayerMessageAsync(playerMessage);
    }

    public async Task SendPlayerMessageAsync(string playerMessage)
    {
        if (!isDialogueOpen || currentNPC == null) return;
        if (isWaitingForResponse) return;
        if (string.IsNullOrWhiteSpace(playerMessage)) return;

        isWaitingForResponse = true;

        if (chatUI != null)
            chatUI.SetInputInteractable(false);

        try
        {
            string trimmedPlayerMessage = playerMessage.Trim();

            AddDialogueLine("Игрок: " + trimmedPlayerMessage);

            if (chatUI != null)
                chatUI.AddPlayerMessage(trimmedPlayerMessage);

            if (chatUI != null)
                chatUI.AddNpcMessage("...");

            HandleQuestLogicBeforeResponse();

            DialogueContext context = contextBuilder.BuildContext(
                currentNPC,
                trimmedPlayerMessage,
                dialogueHistory
            );

            string npcResponse = await responseService.GetResponseAsync(context);

            HandleQuestLogicAfterResponse();

            bool isTechnicalError = IsTechnicalMessage(npcResponse);

            if (chatUI != null)
                chatUI.RemoveLastMessage(); // убираем "..."

            if (!isTechnicalError)
            {
                AddDialogueLine(currentNPC.NPCName + ": " + npcResponse);

                if (chatUI != null)
                    chatUI.AddNpcMessage(npcResponse);
            }
            else
            {
                if (chatUI != null)
                    chatUI.AddSystemMessage(npcResponse);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SendPlayerMessageAsync failed: " + ex);

            if (chatUI != null)
            {
                chatUI.RemoveLastMessage();
                chatUI.AddSystemMessage("Ошибка: " + ex.Message);
            }
        }
        finally
        {
            if (chatUI != null)
            {
                chatUI.ClearInput();
                chatUI.SetInputInteractable(true);
                chatUI.FocusInput();
            }

            isWaitingForResponse = false;
        }
    }

    public void CloseDialogue()
    {
        currentNPC = null;
        isDialogueOpen = false;
        isWaitingForResponse = false;
        dialogueHistory.Clear();

        if (chatUI != null)
            chatUI.ClearMessages();

        if (dialogueUI != null)
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

    private void RebuildChatUIFromHistory()
    {
        if (chatUI == null)
            return;

        chatUI.ClearMessages();

        foreach (string line in dialogueHistory)
        {
            int separatorIndex = line.IndexOf(": ");
            if (separatorIndex <= 0)
                continue;

            string sender = line.Substring(0, separatorIndex);
            string message = line.Substring(separatorIndex + 2);

            if (sender == "Игрок")
                chatUI.AddPlayerMessage(message);
            else if (sender == "Система")
                chatUI.AddSystemMessage(message);
            else
                chatUI.AddNpcMessage(message);
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

    private bool IsTechnicalMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return true;

        string normalized = message.Trim().ToLowerInvariant();

        return normalized.StartsWith("ошибка ai:") ||
               normalized.StartsWith("ошибка http:") ||
               normalized.StartsWith("ошибка:") ||
               normalized.Contains("backend вернул пустой ответ") ||
               normalized.Contains("не удалось распарсить ответ backend") ||
               normalized.Contains("пустой запрос к backend") ||
               normalized.Contains("cannot connect") ||
               normalized.Contains("connection refused") ||
               normalized.Contains("destination host") ||
               normalized.Contains("timeout") ||
               normalized == "...";
    }
}