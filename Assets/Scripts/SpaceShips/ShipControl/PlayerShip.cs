using UnityEngine;

public class PlayerShip : Ship
{
    private InputManager inputManager;
    void Start()
    {
        inputManager = InputManager.Instance;
        inputManager.OnMouseReleaseWithNeutralModule += OnMouseReleaseWithModule;
        inputManager.OnMouseClickWithPlayerModule += OnMouseClickWithPlayerModule;
    }

    void OnDestroy()
    {
        inputManager.OnMouseReleaseWithNeutralModule -= OnMouseReleaseWithModule;
        inputManager.OnMouseClickWithPlayerModule -= OnMouseClickWithPlayerModule;
    }
}