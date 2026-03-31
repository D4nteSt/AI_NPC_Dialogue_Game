using System.Text;
using TMPro;
using UnityEngine;

public class QuestJournalUI : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private TextMeshProUGUI questJournalContentText;

    private void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (questManager == null || questJournalContentText == null)
            return;

        var quests = questManager.GetQuestDescriptions();

        if (quests.Count == 0)
        {
            questJournalContentText.text = "Нет активных записей";
            return;
        }

        StringBuilder builder = new StringBuilder();

        foreach (string questLine in quests)
        {
            builder.AppendLine(questLine);
        }

        questJournalContentText.text = builder.ToString();
    }
}