using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private InventoryItemUI itemPrefab;
    [SerializeField] private GameObject emptyTextObject;

    private readonly List<GameObject> spawnedItems = new List<GameObject>();

    private void OnEnable()
    {
        if (inventoryManager != null)
            inventoryManager.InventoryChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (inventoryManager != null)
            inventoryManager.InventoryChanged -= Refresh;
    }

    public void Refresh()
    {
        ClearSpawnedItems();

        if (inventoryManager == null || contentRoot == null || itemPrefab == null)
            return;

        var items = inventoryManager.GetItemNames();

        bool isEmpty = items.Count == 0;

        if (emptyTextObject != null)
            emptyTextObject.SetActive(isEmpty);

        if (isEmpty)
            return;

        foreach (string itemName in items)
        {
            InventoryItemUI itemUI = Instantiate(itemPrefab, contentRoot);
            itemUI.SetData(itemName);
            spawnedItems.Add(itemUI.gameObject);
        }
    }

    private void ClearSpawnedItems()
    {
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            if (spawnedItems[i] != null)
                Destroy(spawnedItems[i]);
        }

        spawnedItems.Clear();
    }
}