using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChatUI : MonoBehaviour
{
    public event Action<string> PlayerMessageSubmitted;

    [Header("UI References")]
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;

    [Header("Message Prefabs")]
    [SerializeField] private DialogueMessageUI npcMessagePrefab;
    [SerializeField] private DialogueMessageUI playerMessagePrefab;
    [SerializeField] private DialogueMessageUI systemMessagePrefab;

    private void Awake()
    {
        if (sendButton != null)
            sendButton.onClick.AddListener(SubmitInput);
    }

    public void SetNpcName(string npcName)
    {
        if (npcNameText != null)
            npcNameText.text = npcName;
    }

    public void AddNpcMessage(string text)
    {
        AddMessage(npcMessagePrefab, text);
    }

    public void AddPlayerMessage(string text)
    {
        AddMessage(playerMessagePrefab, text);
    }

    public void AddSystemMessage(string text)
    {
        if (systemMessagePrefab != null)
            AddMessage(systemMessagePrefab, text);
    }

    public void ClearMessages()
{
    if (contentRoot == null)
        return;

    for (int i = contentRoot.childCount - 1; i >= 0; i--)
    {
        Transform child = contentRoot.GetChild(i);
        child.gameObject.SetActive(false);
        Destroy(child.gameObject);
    }

    Canvas.ForceUpdateCanvases();
    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)contentRoot);
}

    public void SetInputInteractable(bool value)
    {
        if (inputField != null)
            inputField.interactable = value;

        if (sendButton != null)
            sendButton.interactable = value;
    }

    public void ClearInput()
    {
        if (inputField != null)
            inputField.text = "";
    }

    public void FocusInput()
    {
        if (inputField != null)
            inputField.ActivateInputField();
    }

    public void SubmitInput()
    {
        if (inputField == null)
            return;

        string text = inputField.text.Trim();
        if (string.IsNullOrEmpty(text))
            return;

        inputField.text = "";
        PlayerMessageSubmitted?.Invoke(text);
        inputField.ActivateInputField();
    }

    private void AddMessage(DialogueMessageUI prefab, string text)
    {
        if (prefab == null || contentRoot == null)
            return;

        DialogueMessageUI item = Instantiate(prefab, contentRoot);
        item.SetText(text);

        Canvas.ForceUpdateCanvases();

        RectTransform itemRect = item.GetComponent<RectTransform>();
        if (itemRect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemRect);

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)contentRoot);

        StartCoroutine(ScrollToBottomNextFrame());
    }

    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)contentRoot);
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void RemoveLastMessage()
    {
        if (contentRoot == null || contentRoot.childCount == 0)
            return;

        Transform lastChild = contentRoot.GetChild(contentRoot.childCount - 1);
        Destroy(lastChild.gameObject);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)contentRoot);
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void Update()
    {
        if (inputField == null || !inputField.isFocused || !inputField.interactable)
            return;

        bool enterPressed =
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter);

        if (!enterPressed)
            return;

        bool shiftHeld =
            Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift);

        if (shiftHeld)
        {
            InsertNewLineAtCaret();
            return;
        }

        SubmitInput();
    }

    private void InsertNewLineAtCaret()
    {
        if (inputField == null)
            return;

        string current = inputField.text ?? string.Empty;
        int caret = inputField.stringPosition;

        current = current.Insert(caret, "\n");
        inputField.text = current;
        inputField.stringPosition = caret + 1;
        inputField.caretPosition = caret + 1;
        inputField.ActivateInputField();
    }
}