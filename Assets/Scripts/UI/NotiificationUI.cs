using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float showDuration = 2.5f;
    [SerializeField] private AutoResizeTextPanel autoResizeTextPanel;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (root != null)
            root.SetActive(false);
    }

    public void Show(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(message));
    }

    private IEnumerator ShowRoutine(string message)
    {
        if (root != null)
            root.SetActive(true);

        if (messageText != null)
            messageText.text = message;

        if (autoResizeTextPanel != null)
            autoResizeTextPanel.Refresh();

        yield return new WaitForSeconds(showDuration);

        if (root != null)
            root.SetActive(false);

        currentRoutine = null;
    }
}