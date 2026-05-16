using System;
using ModuleSpaceShip.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance
    {
        get
        {
            if(_instance == null) return null;
            else return _instance;
        }
    }
    [SerializeField] private LayerMask[] ClickOrders;
    private Camera cam;

    private Vector2 _mousePos;
    public Vector2 mousePos
    {
        get => _mousePos;
    }

    private GameObject grabbedModule;

    // ----Events----
    public event Action<Collider2D> OnMouseReleaseWithNeutralModule;
    public event Action<Collider2D> OnMouseClickWithPlayerModule;
    public event Action OnMouseClickStartWithVoid, OnMouseClickEndWithVoid;
    public event Action<Vector2, float> OnMovementStart;
    

    void Awake()
    {
        if(_instance == null) _instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        cam = Camera.main;
        _mousePos = Vector2.zero;
        grabbedModule = null;
    }

    public void OnMouseMove(InputAction.CallbackContext ctx)
    {
        Vector2 pos = ctx.ReadValue<Vector2>();
        Vector2 target = cam.ScreenToWorldPoint(pos);

        _mousePos = target;
    }

    public void OnClick(InputAction.CallbackContext ctx)
    {
        Collider2D hit = GetHitByOrder();
        //if(hit == null) return;

        if(ctx.phase == InputActionPhase.Started) // 클릭 시작
        {
            Debug.Log($"[InputManager] Hit started : {hit?.transform.name}");
            if (hit != null && hit.CompareTag("PlayerShip") && grabbedModule == null) // 선체 분리를 시도할 경우
            {
                //Debug.LogWarning($"[InputManager] hit : {hit}");
                OnMouseClickWithPlayerModule?.Invoke(hit); // 선체 분리 시도 이벤트 발생
                grabbedModule = hit.gameObject;
                grabbedModule.GetComponent<BaseMonobehaviour>()?.OnClickStart();
            }
            else if(hit != null) // 중립 선체에서 클릭 시작한 경우
            {
                grabbedModule = hit.gameObject;
                grabbedModule.GetComponent<BaseMonobehaviour>()?.OnClickStart();
            }
            else // 빈공간에서 클릭 시작한경우
            {
                OnMouseClickStartWithVoid?.Invoke();
            }
            Debug.Log($"[InputManager] grabbedModule : {grabbedModule}");
        }
        else if(ctx.phase == InputActionPhase.Canceled)
        {
            Debug.Log($"[InputManager] Hit canceled : {hit?.transform.name}");
            if(grabbedModule != null) // 선체를 잡고있는 상태엿다면 선체를 놓는 상호작용
            {
                Collider2D ModuleCol = grabbedModule.GetComponent<Collider2D>();
                if(!ModuleCol) Debug.LogError($"[InputManager] Can't find collider : {grabbedModule.name}");
                OnMouseReleaseWithNeutralModule?.Invoke(ModuleCol);
                grabbedModule.GetComponent<BaseMonobehaviour>()?.OnClickCancel(hit);
                grabbedModule = null;
            }
            else // 아무것도 안잡고 있었다면 해당 이벤트 발생
            {
                OnMouseClickEndWithVoid?.Invoke();
            }
        }
    }

    private Collider2D GetHitByOrder()
    {
        Collider2D hit = null;
        foreach(LayerMask clickOrder in ClickOrders)
        {
            hit = Physics2D.OverlapPoint(_mousePos, clickOrder);
            if(hit != null) break;
        }

        return hit;
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        Vector3 inputVector = ctx.ReadValue<Vector3>();
        // x축 = A, D (A가 -1, D가 1)
        // y축 = W, S (S가 -1, W가 1)
        // z축 = Q, E (Q가 -1, E가 1)
        Debug.Log($"[InputManager] inputVector : {inputVector}");
        // 이 벡터를 전후좌우 Vector2와 회전 Torque로 전환
        Vector2 movement = new Vector2(inputVector.z, inputVector.y).normalized;
        float torque = -inputVector.x;

        OnMovementStart?.Invoke(movement, torque);
    }
}
