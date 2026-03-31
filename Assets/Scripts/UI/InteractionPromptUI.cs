using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    private void Update()
    {
        if (playerInteraction.CurrentInteractable != null)
        {
            promptPanel.SetActive(true);
            promptText.text = playerInteraction.CurrentInteractable.GetInteractionText();
            //Debug.Log("Interactable найден");
        }
        else
        {
            promptPanel.SetActive(false);
            //Debug.Log("Interactable не найден");
        }
    }
}