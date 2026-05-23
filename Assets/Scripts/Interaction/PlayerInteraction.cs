using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private GameplayUIController gameplayUIController;

    [Header("Look Check")]
    [SerializeField] private Transform viewTransform;
    [SerializeField] private float lookDotThreshold = 0.35f;

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

        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            interactionRange,
            interactionLayer,
            QueryTriggerInteraction.Collide
        );

        float bestScore = Mathf.NegativeInfinity;

        foreach (Collider collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();

            if (interactable == null)
                interactable = collider.GetComponentInParent<IInteractable>();

            if (interactable == null)
                continue;

            Transform targetTransform = GetInteractableTransform(interactable);

            if (targetTransform == null)
                continue;

            float distance = Vector3.Distance(transform.position, targetTransform.position);

            if (distance > interactionRange)
                continue;

            float lookDot = GetLookDot(targetTransform);

            if (lookDot < lookDotThreshold)
                continue;

            float distanceScore = 1f - Mathf.Clamp01(distance / interactionRange);
            float score = lookDot + distanceScore;

            if (score > bestScore)
            {
                bestScore = score;
                currentInteractable = interactable;
            }
        }
    }

    private Transform GetInteractableTransform(IInteractable interactable)
    {
        MonoBehaviour behaviour = interactable as MonoBehaviour;

        if (behaviour == null)
            return null;

        return behaviour.transform;
    }

    private float GetLookDot(Transform target)
    {
        if (target == null)
            return -1f;

        Transform source = viewTransform != null ? viewTransform : transform;

        Vector3 direction = target.position - source.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return 1f;

        direction.Normalize();

        Vector3 forward = source.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.01f)
            return -1f;

        forward.Normalize();

        return Vector3.Dot(forward, direction);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        Transform source = viewTransform != null ? viewTransform : transform;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(source.position, source.forward * interactionRange);
    }
}