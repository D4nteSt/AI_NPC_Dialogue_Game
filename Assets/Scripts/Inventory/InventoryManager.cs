using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private Dictionary<string, string> items = new Dictionary<string, string>();

    public IReadOnlyDictionary<string, string> Items => items;

    public void AddItem(string itemId, string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            return;

        if (!items.ContainsKey(itemId))
        {
            items.Add(itemId, itemName);
            Debug.Log("Предмет добавлен в инвентарь: " + itemName);
        }
    }

    public bool HasItem(string itemId)
    {
        return items.ContainsKey(itemId);
    }

    public void RemoveItem(string itemId)
    {
        if (items.ContainsKey(itemId))
        {
            Debug.Log("Предмет удален из инвентаря: " + items[itemId]);
            items.Remove(itemId);
        }
    }

    public List<string> GetItemNames()
    {
        return new List<string>(items.Values);
    }
}