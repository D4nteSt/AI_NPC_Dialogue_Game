using System.Text;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private TextMeshProUGUI inventoryContentText;

    private void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (inventoryManager == null || inventoryContentText == null)
            return;

        var items = inventoryManager.GetItemNames();

        if (items.Count == 0)
        {
            inventoryContentText.text = "Пусто";
            return;
        }

        StringBuilder builder = new StringBuilder();

        foreach (string itemName in items)
        {
            builder.AppendLine("- " + itemName);
        }

        inventoryContentText.text = builder.ToString();
    }
}