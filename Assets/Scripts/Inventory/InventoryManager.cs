using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private Dictionary<string, string> items = new Dictionary<string, string>();

    public IReadOnlyDictionary<string, string> Items => items;

    public event Action InventoryChanged;

    public void AddItem(string itemId, string itemName)
    {
        itemId = DialogueDataNormalizer.NormalizeId(itemId);
        itemName = DialogueDataNormalizer.NormalizeText(itemName);

        if (string.IsNullOrWhiteSpace(itemId))
            return;

        if (string.IsNullOrWhiteSpace(itemName))
            itemName = itemId;

        if (!items.ContainsKey(itemId))
        {
            items.Add(itemId, itemName);
            Debug.Log("Предмет добавлен в инвентарь: " + itemName);
            InventoryChanged?.Invoke();
        }
    }

    public bool HasItem(string itemId)
    {
        itemId = DialogueDataNormalizer.NormalizeId(itemId);
        return items.ContainsKey(itemId);
    }

    public void RemoveItem(string itemId)
    {
        itemId = DialogueDataNormalizer.NormalizeId(itemId);

        if (items.ContainsKey(itemId))
        {
            Debug.Log("Предмет удален из инвентаря: " + items[itemId]);
            items.Remove(itemId);
            InventoryChanged?.Invoke();
        }
    }

    public List<string> GetItemNames()
    {
        return new List<string>(items.Values);
    }
}