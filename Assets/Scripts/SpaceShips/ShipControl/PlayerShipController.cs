using UnityEngine;

public class PlayershipContoller : MonoBehaviour
{
    private InputManager inputManager;
    private Ship playerShip;

    private bool isFiring = false;
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
        ShipControlIntent shipControlIntent = new(movement, torque, isFiring);
        playerShip.SetControlIntent(shipControlIntent);
    }

    void SetFiringTrue(){ isFiring = true; }
    void SetFiringFalse(){ isFiring = false; }

    void OnDestroy()
    {
        inputManager.OnMovementStart -= CreateShipControlIntent;
        inputManager.OnMouseClickStartWithVoid -= SetFiringTrue;
        inputManager.OnMouseClickEndWithVoid -= SetFiringFalse;
    }
}