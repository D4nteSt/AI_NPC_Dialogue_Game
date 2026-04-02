using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -5f);
    [SerializeField] private float mouseSensitivity = 150f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private GameplayUIController gameplayUIController;

    private float yaw;
    private float pitch;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        bool canRotateCamera =
            (dialogueManager == null || !dialogueManager.IsDialogueOpen) &&
            (gameplayUIController == null || !gameplayUIController.IsAnyGameplayPanelOpen);

        if (canRotateCamera)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * mouseSensitivity * Time.deltaTime;
            pitch -= mouseY * mouseSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}