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

    [Header("Dialogue Logic")]
    [SerializeField] private PlayerIntentClassifier playerIntentClassifier;
    [SerializeField] private DialogueRuleResolver dialogueRuleResolver;
    [SerializeField] private DialogueOutcomeExecutor dialogueOutcomeExecutor;
    [SerializeField] private InventoryManager inventoryManager;

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
        RegisterNpcQuests(currentNPC);

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

            DialogueEvaluationContext evaluationContext = BuildEvaluationContext(trimmedPlayerMessage);

            Debug.Log(
                $"EVAL CONTEXT => npcId=[{evaluationContext.NpcId}], " +
                $"intent=[{evaluationContext.PlayerIntent}], " +
                $"questId=[{evaluationContext.QuestId}], " +
                $"questStatus=[{evaluationContext.QuestStatus}], " +
                $"items=[{string.Join(", ", evaluationContext.InventoryItemIds)}]"
            );

            string lastOutcomeSummary = string.Empty;

            if (dialogueRuleResolver != null && dialogueOutcomeExecutor != null)
            {
                DialogueOutcome outcome = dialogueRuleResolver.Resolve(evaluationContext);

                if (outcome != null && outcome.HasActions)
                {
                    DialogueOutcomeExecutionResult executionResult =
                        dialogueOutcomeExecutor.Execute(outcome);

                    if (executionResult != null && executionResult.HasSummary)
                    {
                        lastOutcomeSummary = executionResult.ToPromptText();

                        Debug.Log("OUTCOME SUMMARY:\n" + lastOutcomeSummary);
                    }
                }
            }

            if (chatUI != null)
                chatUI.AddNpcMessage("...");

            //HandleQuestLogicBeforeResponse();

            DialogueContext context = contextBuilder.BuildContext(
                currentNPC,
                trimmedPlayerMessage,
                dialogueHistory,
                lastOutcomeSummary
            );

            string npcResponse = await responseService.GetResponseAsync(context);

            //HandleQuestLogicAfterResponse();

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
                    return npcData.GreetingMessage + " У меня есть дело, в котором потребуется внимательный человек.";

                case QuestStatus.InProgress:
                    questManager.CheckQuestProgress(npcData.QuestData.questId);

                    if (questManager.GetQuestStatus(npcData.QuestData.questId) == QuestStatus.Completed)
                        return "Вы нашли улику? Давайте посмотрим.";

                    return "Продолжайте осмотр. В таком деле мелочей не бывает.";

                case QuestStatus.Completed:
                    return "Вы нашли улику? Давайте посмотрим.";

                case QuestStatus.TurnedIn:
                    return "Улика принята. Это не конец расследования, но теперь у нас есть направление.";

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
                        return "Вы что-то нашли? Надеюсь, это поможет инспектору.";

                    return npcData.GreetingMessage;

                case QuestStatus.Completed:
                    return "Вы что-то нашли? Надеюсь, это поможет инспектору.";

                case QuestStatus.TurnedIn:
                    return "Инспектор забрал улику? Тогда, возможно, он поймет, куда она ведет.";

                default:
                    return npcData.GreetingMessage;
            }
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

    private DialogueEvaluationContext BuildEvaluationContext(string playerMessage)
    {
        PlayerIntentType intent = PlayerIntentType.None;

        if (playerIntentClassifier != null)
            intent = playerIntentClassifier.Classify(playerMessage);

        string npcId = currentNPC != null
            ? DialogueDataNormalizer.NormalizeId(currentNPC.NPCName)
            : string.Empty;

        string questId = currentNPC != null && currentNPC.QuestData != null
            ? DialogueDataNormalizer.NormalizeId(currentNPC.QuestData.questId)
            : string.Empty;

        QuestStatus questStatus = QuestStatus.NotStarted;

        if (!string.IsNullOrWhiteSpace(questId) && questManager != null)
            questStatus = questManager.GetQuestStatus(questId);

        DialogueEvaluationContext context = new DialogueEvaluationContext
        {
            NpcId = npcId,
            NpcName = currentNPC != null ? currentNPC.NPCName : string.Empty,
            PlayerMessage = playerMessage,
            PlayerIntent = intent,
            QuestId = questId,
            QuestStatus = questStatus
        };

        if (currentNPC != null && questManager != null)
        {
            foreach (QuestData questData in currentNPC.GetAllQuestData())
            {
                if (questData == null || string.IsNullOrWhiteSpace(questData.questId))
                    continue;

                string normalizedQuestId = DialogueDataNormalizer.NormalizeId(questData.questId);
                QuestStatus status = questManager.GetQuestStatus(questData.questId);

                if (!context.QuestStatuses.ContainsKey(normalizedQuestId))
                    context.QuestStatuses.Add(normalizedQuestId, status);
            }
        }

        if (inventoryManager != null)
        {
            foreach (var itemId in inventoryManager.Items.Keys)
            {
                context.InventoryItemIds.Add(DialogueDataNormalizer.NormalizeId(itemId));
            }
        }

        return context;
    }
    private void RegisterNpcQuests(NPCDialogueData npcData)
    {
        if (npcData == null || questManager == null)
            return;

        foreach (QuestData questData in npcData.GetAllQuestData())
        {
            questManager.RegisterQuest(questData);
        }
    }
}