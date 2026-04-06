using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private GameplayUIController gameplayUIController;
    [SerializeField] private Animator characterAnimator;

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        bool shouldBlockMovement =
            (dialogueManager != null && dialogueManager.IsDialogueOpen) ||
            (gameplayUIController != null && gameplayUIController.IsAnyGameplayPanelOpen);

        if (shouldBlockMovement)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        bool isMoving = inputDirection.magnitude >= 0.1f;

        if (characterAnimator != null)
        {
            characterAnimator.SetBool("IsMoving", isMoving);
        }

        Vector3 moveDirection = Vector3.zero;


        if (inputDirection.magnitude >= 0.1f)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            moveDirection = (cameraForward * inputDirection.z + cameraRight * inputDirection.x).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -0.05f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 finalMove = moveDirection * moveSpeed;
        finalMove.y = verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);
    }
}