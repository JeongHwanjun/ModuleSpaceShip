using UnityEngine;

public class PlayershipContoller : MonoBehaviour
{
    private InputManager inputManager;
    private Ship playerShip;

    private bool isFiring = false;
    private Vector2 currentMovement= Vector2.zero;
    private float currentTorque = 0f;
    void Start()
    {
        playerShip = GetComponent<Ship>();
        inputManager = InputManager.Instance;
        inputManager.OnMovementStart += CreateShipControlIntent;
        inputManager.OnMouseClickStartWithVoid += SetFiringTrue;
        inputManager.OnMouseClickEndWithVoid += SetFiringFalse;
    }

    void CreateShipControlIntent(Vector2 movement, float torque)
    {
        if(playerShip == null)
        {
            Debug.LogError($"[PlayerShipController] 'playerShip' is not assigned, but tried to access it.");
            return;
        }
        currentMovement = movement;
        currentTorque = torque;
        refreshShipControlIntent();
    }

    void SetFiringTrue()
    {
        isFiring = true;
        refreshShipControlIntent();
    }
    void SetFiringFalse()
    {
        isFiring = false;
        refreshShipControlIntent();
    }

    void refreshShipControlIntent()
    {
        ShipControlIntent controlIntent = new(currentMovement, currentTorque, isFiring);
        playerShip.SetControlIntent(controlIntent);
    }

    void OnDestroy()
    {
        inputManager.OnMovementStart -= CreateShipControlIntent;
        inputManager.OnMouseClickStartWithVoid -= SetFiringTrue;
        inputManager.OnMouseClickEndWithVoid -= SetFiringFalse;
    }
}