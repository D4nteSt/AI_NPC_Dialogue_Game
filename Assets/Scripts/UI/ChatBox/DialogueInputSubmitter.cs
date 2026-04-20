using TMPro;
using UnityEngine;

public class DialogueInputSubmitter : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private DialogueChatUI chatUI;

    private void Update()
    {
        if (inputField == null || chatUI == null)
            return;

        if (!inputField.isFocused)
            return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                return;

            chatUI.SubmitInput();
        }
    }
}