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
    private float viewportModuleSizeInUse;
    [SerializeField] private float maxCameraOffset = 3f;
    private float maxCameraOffsetInUse;
    [SerializeField] private float offsetSmoothTime = 0.15f;
    [Header("Scroll Variables")]
    [SerializeField] private float orthographicSizePerScroll = 1f;
    [SerializeField] private float orthographicSizeCoefficient = 0.8f;
    [SerializeField] private float minScrollValue = 1f;
    [SerializeField] private float maxScrollValue = 10f;
    private float scrollValue = 1f;
    private float originalOrthographicSize;

    [Header("Follow")]
    [SerializeField] private float followSmoothTime = 0.1f;

    private InputManager inputManager;

    private Vector3 followVelocity;
    private Vector3 offsetVelocity;

    private Vector3 currentMouseOffset;
    private bool isCameraFollowingMouse = false;


    private void Start()
    {
        inputManager = InputManager.Instance;

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("PlayerShip").transform;
        }

        viewportModuleSizeInUse = viewportModuleSize;
        maxCameraOffsetInUse = maxCameraOffset;
        originalOrthographicSize = playerCamera.orthographicSize;

        inputManager.OnModeToggleStart += OnModeToggleStart;
        inputManager.OnModeToggleCanceled += OnModeToggleCanceled;
        inputManager.OnMouseWheelStart += OnMouseWheelStart;
    }

    private void OnDestroy()
    {
        inputManager.OnModeToggleStart -= OnModeToggleStart;
        inputManager.OnModeToggleCanceled -= OnModeToggleCanceled;
        inputManager.OnMouseWheelStart -= OnMouseWheelStart;
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

        float pixelWidthPerModuleSize = cameraWidth / viewportModuleSizeInUse;
        float pixelHeightPerModuleSize = cameraHeight / viewportModuleSizeInUse;

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

        targetMouseOffset.x = Mathf.Clamp(targetMouseOffset.x, -maxCameraOffsetInUse, maxCameraOffsetInUse);
        targetMouseOffset.y = Mathf.Clamp(targetMouseOffset.y, -maxCameraOffsetInUse, maxCameraOffsetInUse);

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

    void OnModeToggleStart()
    {
        isCameraFollowingMouse = true;
    }

    void OnModeToggleCanceled()
    {
        StopCamera();
        isCameraFollowingMouse = false;
    }

    void OnMouseWheelStart(float wheelValue)
    {
        // >0이면 위, <0이면 아래
        // Debug.Log($"[CameraRig] Up or Down : {wheelValue}");
        scrollValue = Mathf.Clamp(scrollValue + wheelValue, minScrollValue, maxScrollValue);

        playerCamera.orthographicSize = originalOrthographicSize + orthographicSizePerScroll * scrollValue * orthographicSizeCoefficient;
        viewportModuleSizeInUse = viewportModuleSize * scrollValue;
        maxCameraOffsetInUse = maxCameraOffset * scrollValue;
    }
}