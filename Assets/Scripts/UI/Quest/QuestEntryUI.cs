using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image statusBackground;

    public void SetData(QuestData questData, QuestStatus status)
    {
        if (questData == null)
            return;

        if (questNameText != null)
            questNameText.text = questData.questName;

        if (descriptionText != null)
            descriptionText.text = questData.description;

        if (statusText != null)
            statusText.text = GetStatusText(status);

        ApplyStatusVisual(status);
    }

    private string GetStatusText(QuestStatus status)
    {
        switch (status)
        {
            case QuestStatus.NotStarted:
                return "Не начат";

            case QuestStatus.InProgress:
                return "В процессе";

            case QuestStatus.Completed:
                return "Готов к сдаче";

            case QuestStatus.TurnedIn:
                return "Сдан";

            default:
                return "Неизвестно";
        }
    }

    private void ApplyStatusVisual(QuestStatus status)
    {
        if (statusBackground == null)
            return;

        switch (status)
        {
            case QuestStatus.NotStarted:
                statusBackground.color = new Color(0.45f, 0.45f, 0.45f, 0.8f);
                break;

            case QuestStatus.InProgress:
                statusBackground.color = new Color(0.9f, 0.65f, 0.2f, 0.85f);
                break;

            case QuestStatus.Completed:
                statusBackground.color = new Color(0.2f, 0.75f, 0.35f, 0.85f);
                break;

            case QuestStatus.TurnedIn:
                statusBackground.color = new Color(0.25f, 0.55f, 0.35f, 0.75f);
                break;

            default:
                statusBackground.color = new Color(0.4f, 0.4f, 0.4f, 0.8f);
                break;
        }
    }
}