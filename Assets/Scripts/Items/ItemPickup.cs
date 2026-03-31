using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemId = "artifact_01";
    [SerializeField] private string itemName = "Артефакт";
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private string relatedQuestId = "quest_find_artifact";

    public void Interact()
    {
        if (inventoryManager != null)
        {
            inventoryManager.AddItem(itemId, itemName);
        }

        if (questManager != null)
        {
            questManager.CheckQuestProgress(relatedQuestId);
        }

        Debug.Log("Игрок подобрал предмет: " + itemName);
        Destroy(gameObject);
    }

    public string GetInteractionText()
    {
        return "Нажмите E, чтобы подобрать " + itemName;
    }
}