using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float showDuration = 2.5f;

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

        if (messageText != null)
            messageText.text = message;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        if (root != null)
            root.SetActive(true);

        yield return new WaitForSeconds(showDuration);

        if (root != null)
            root.SetActive(false);

        currentRoutine = null;
    }
}