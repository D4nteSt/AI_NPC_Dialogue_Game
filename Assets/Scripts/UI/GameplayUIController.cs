using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject questJournalPanel;
    [SerializeField] private DialogueManager dialogueManager;

    public bool IsInventoryOpen => inventoryPanel != null && inventoryPanel.activeSelf;
    public bool IsQuestJournalOpen => questJournalPanel != null && questJournalPanel.activeSelf;
    public bool IsAnyGameplayPanelOpen => IsInventoryOpen || IsQuestJournalOpen;

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (dialogueManager != null && dialogueManager.IsDialogueOpen)
        {
            if (IsAnyGameplayPanelOpen)
            {
                CloseAllGameplayPanels();
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleQuestJournal();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllGameplayPanels();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;

        bool newState = !inventoryPanel.activeSelf;

        if (newState && questJournalPanel != null)
        {
            questJournalPanel.SetActive(false);
        }

        inventoryPanel.SetActive(newState);
        UpdateCursorState();
    }

    public void ToggleQuestJournal()
    {
        if (questJournalPanel == null) return;

        bool newState = !questJournalPanel.activeSelf;

        if (newState && inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        questJournalPanel.SetActive(newState);
        UpdateCursorState();
    }

    public void CloseAllGameplayPanels()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (questJournalPanel != null)
            questJournalPanel.SetActive(false);

        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        bool shouldShowCursor = IsAnyGameplayPanelOpen;

        Cursor.lockState = shouldShowCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = shouldShowCursor;
    }
}