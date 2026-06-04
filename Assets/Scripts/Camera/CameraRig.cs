using UnityEngine;

public class PlayerCameraRig : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraZ = -10f;

    [Header("Mouse Look Offset")]
    [SerializeField] private float viewportModuleSize = 10f;
    [SerializeField] private float maxCameraOffset = 3f;
    [SerializeField] private float offsetSmoothTime = 0.15f;

    [Header("Follow")]
    [SerializeField] private float followSmoothTime = 0.1f;

    private InputManager inputManager;

    private Vector3 followVelocity;
    private Vector3 offsetVelocity;

    private Vector3 currentMouseOffset;
    private bool isCameraFollowingMouse = true;


    private void Start()
    {
        inputManager = InputManager.Instance;

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if(target == null)
        {
            target = GameObject.Find("PlayerShip").transform;
        }

        inputManager.OnCameraToggleStart += OnCameraToggleStart;
    }

    private void OnDestroy()
    {
        inputManager.OnCameraToggleStart -= OnCameraToggleStart;
    }

    private void LateUpdate()
    {
        if (target == null || playerCamera == null || inputManager == null)
            return;

        MoveRigToTarget();
        if(isCameraFollowingMouse) MoveCameraByMouse();
        LockRotation();
    }

    private void MoveRigToTarget()
    {
        Vector3 targetPos = new Vector3(
            target.position.x,
            target.position.y,
            0f
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref followVelocity,
            followSmoothTime
        );
    }

    private void MoveCameraByMouse()
    {
        Vector2 mousePos = inputManager.mousePosScreen;

        float cameraWidth = playerCamera.pixelWidth;
        float cameraHeight = playerCamera.pixelHeight;

        float pixelWidthPerModuleSize = cameraWidth / viewportModuleSize;
        float pixelHeightPerModuleSize = cameraHeight / viewportModuleSize;

        Vector2 adjustedMousePos = new Vector2(
            mousePos.x - cameraWidth * 0.5f,
            mousePos.y - cameraHeight * 0.5f
        );

        float targetOffsetX = adjustedMousePos.x / pixelWidthPerModuleSize;
        float targetOffsetY = adjustedMousePos.y / pixelHeightPerModuleSize;

        Vector3 targetMouseOffset = new Vector3(
            targetOffsetX,
            targetOffsetY,
            cameraZ
        );

        targetMouseOffset.x = Mathf.Clamp(targetMouseOffset.x, -maxCameraOffset, maxCameraOffset);
        targetMouseOffset.y = Mathf.Clamp(targetMouseOffset.y, -maxCameraOffset, maxCameraOffset);

        currentMouseOffset = Vector3.SmoothDamp(
            currentMouseOffset,
            targetMouseOffset,
            ref offsetVelocity,
            offsetSmoothTime
        );

        playerCamera.transform.localPosition = currentMouseOffset;
    }

    private void LockRotation()
    {
        transform.rotation = Quaternion.identity;
        playerCamera.transform.rotation = Quaternion.identity;
    }

    private void StopCamera()
    {
        playerCamera.transform.localPosition = new Vector3(0, 0, -10f);
    }

    void OnCameraToggleStart()
    {
        if(isCameraFollowingMouse) StopCamera();
        isCameraFollowingMouse = !isCameraFollowingMouse;
    }
}