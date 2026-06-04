using UnityEngine;

public class PlayerShip : Ship
{
    private InputManager inputManager;
    void Start()
    {
        inputManager = InputManager.Instance;
        inputManager.OnMouseReleaseWithNeutralModule += OnMouseReleaseWithModule;
        inputManager.OnMouseClickWithPlayerModule += OnMouseClickWithPlayerModule;
        inputManager.OnMouseClickStartWithVoid += OnMouseClickStartWithVoid;
        inputManager.OnMouseClickEndWithVoid += OnMouseClickEndWithVoid;
        inputManager.OnMovementStart += OnMovementStart;
    }

    void OnDestroy()
    {
        inputManager.OnMouseReleaseWithNeutralModule -= OnMouseReleaseWithModule;
        inputManager.OnMouseClickWithPlayerModule -= OnMouseClickWithPlayerModule;
        inputManager.OnMouseClickStartWithVoid -= OnMouseClickStartWithVoid;
        inputManager.OnMouseClickEndWithVoid -= OnMouseClickEndWithVoid;
        inputManager.OnMovementStart -= OnMovementStart;
    }
}