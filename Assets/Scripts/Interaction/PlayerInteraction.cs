using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private GameplayUIController gameplayUIController;

    private IInteractable currentInteractable;

    public IInteractable CurrentInteractable => currentInteractable;

    private void Update()
    {
        bool shouldBlockInteraction =
            (dialogueManager != null && dialogueManager.IsDialogueOpen) ||
            (gameplayUIController != null && gameplayUIController.IsAnyGameplayPanelOpen);

        if (shouldBlockInteraction)
        {
            currentInteractable = null;
            return;
        }

        FindInteractable();

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }

    private void FindInteractable()
    {
        currentInteractable = null;

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, interactionLayer);

        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable == null)
                continue;

            float distance = Vector3.Distance(transform.position, collider.transform.position);

            if (distance <= interactionRange && distance < closestDistance)
            {
                closestDistance = distance;
                currentInteractable = interactable;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}