using TMPro;
using UnityEngine;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;

    public void SetData(string itemName)
    {
        if (itemNameText != null)
            itemNameText.text = itemName;
    }
}