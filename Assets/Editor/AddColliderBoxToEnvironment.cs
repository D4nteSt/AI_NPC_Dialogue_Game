using UnityEditor;
using UnityEngine;

public static class AddCollidersToEnvironment
{
    [MenuItem("Tools/Environment/Add Box Colliders To Selected Hierarchy")]
    private static void AddBoxCollidersToSelectedHierarchy()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            Debug.LogWarning("Выбери родительский объект окружения в Hierarchy.");
            return;
        }

        MeshRenderer[] renderers = selected.GetComponentsInChildren<MeshRenderer>(true);

        int addedCount = 0;

        foreach (MeshRenderer renderer in renderers)
        {
            GameObject obj = renderer.gameObject;

            if (obj.GetComponent<Collider>() != null)
                continue;

            Undo.AddComponent<BoxCollider>(obj);
            addedCount++;
        }

        Debug.Log("Добавлено BoxCollider: " + addedCount);
    }
}