using System.Collections;
using UnityEngine;

public class PoliceGateController : MonoBehaviour
{
    [Header("Gate")]
    [SerializeField] private Collider blockerCollider;

    [Header("Police Movement")]
    [SerializeField] private Transform policeTransform;
    [SerializeField] private Transform movePoint;
    [SerializeField] private Animator policeAnimator;

    [Header("Animation")]
    [SerializeField] private string walkingBoolName = "IsWalking";

    [Header("Movement Settings")]
    [SerializeField] private float moveDuration = 1.2f;

    [Header("State")]
    [SerializeField] private bool isOpened;

    private Coroutine moveRoutine;

    public bool IsOpened => isOpened;

    public void OpenGate()
    {
        if (isOpened)
            return;

        isOpened = true;

        if (blockerCollider != null)
            blockerCollider.enabled = false;

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MovePoliceAside());
    }

    private IEnumerator MovePoliceAside()
    {
        if (policeTransform == null || movePoint == null)
            yield break;

        Vector3 startPosition = policeTransform.position;
        Quaternion startRotation = policeTransform.rotation;

        Vector3 targetPosition = movePoint.position;
        Quaternion targetRotation = movePoint.rotation;

        float elapsed = 0f;

        SetWalking(true);

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            policeTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            policeTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        policeTransform.position = targetPosition;
        policeTransform.rotation = targetRotation;

        SetWalking(false);
        moveRoutine = null;
    }

    private void SetWalking(bool value)
    {
        if (policeAnimator == null || string.IsNullOrWhiteSpace(walkingBoolName))
            return;

        policeAnimator.SetBool(walkingBoolName, value);
    }
}