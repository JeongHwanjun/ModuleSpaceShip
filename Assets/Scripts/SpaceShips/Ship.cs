using System;
using System.Collections.Generic;
using ModuleSpaceShip.Runtime;
using UnityEngine;

// 함선 그 자체를 뜻하는 클래스
public class Ship : MonoBehaviour
{
    [DefName("ShipDef")]
    [SerializeField] private string def;
    [SerializeField] private ShipThing shipThing;
    public Vector2 heading
    {
        get
        {
            return (Vector2)transform.up;
        }
    }
    private InputManager inputManager;
    private Rigidbody2D rigid;
    private ShipGrid shipGrid;
    public float torque; // 테스트시 가할 회전힘의 강도
    public Collider2D[] moduleColliders;

    // ---- Events ----
    public event Action<Collider2D> OnTryDockAtPort;
    public event Action<Collider2D> OnTryUndock;
    public event Action OnTryFireStart;
    public event Action OnTryFireStop;

    void Awake()
    {
        shipThing = (ShipThing)ThingFactory.CreateFromDefName(def);
        rigid = GetComponent<Rigidbody2D>();
        shipGrid = GetComponentInChildren<ShipGrid>();
    }

    void Start()
    {
        inputManager = InputManager.Instance;
        inputManager.OnMouseReleaseWithNeutralModule += OnMouseReleaseWithModule;
        inputManager.OnMouseClickWithPlayerModule += OnMouseClickWithPlayerModule;
        inputManager.OnMouseClickStartWithVoid += OnMouseClickStartWithVoid;
        inputManager.OnMouseClickEndWithVoid += OnMouseClickEndWithVoid;
    }

    void OnDestroy()
    {
        inputManager.OnMouseReleaseWithNeutralModule -= OnMouseReleaseWithModule;
        inputManager.OnMouseClickWithPlayerModule -= OnMouseClickWithPlayerModule;
        inputManager.OnMouseClickStartWithVoid -= OnMouseClickStartWithVoid;
        inputManager.OnMouseClickEndWithVoid -= OnMouseClickEndWithVoid;
    }

    private void OnMouseReleaseWithModule(Collider2D col)
    {
        OnTryDockAtPort?.Invoke(col);
    }
    private void OnMouseClickWithPlayerModule(Collider2D col)
    {
        OnTryUndock?.Invoke(col);
    }
    private void OnMouseClickStartWithVoid()
    {
        OnTryFireStart?.Invoke();
    }
    private void OnMouseClickEndWithVoid()
    {
        OnTryFireStop?.Invoke();
    }

    public void OnModuleDestroyed(Module oldModule)
    {
        // 선체가 파괴되었을 때 해당 선체에서 호출함
        // 0. 코어 선체인지 확인(코어 선체일 경우 사망)
        // 1. 분리 진행
        OnModuleDetached(oldModule);
    }

    public void OnModuleAttached(Module newModule)
    {
        // 선체가 부착되었을 때 해당 선체에서 호출함
        // 1. 해당 선체의 정보 참조
        // 2. 정보를 바탕으로 Thing의 정보 갱신
        // 3. rigidbody 정보 갱신
        float m = newModule.GetDefMass();
        float M = rigid.mass;                 // 붙기 전 질량
        Vector3 c = rigid.centerOfMass;       // 붙기 전 COM (로컬)
        Vector3 r = newModule.transform.localPosition; // 로컬 위치

        rigid.mass = M + m;
        rigid.centerOfMass = (M * c + m * r) / (M + m);
        Debug.Log($"[Ship] COM : {rigid.centerOfMass}");
        // 4. 정보 갱신 이벤트 발생
        RefreshShip();
        // 5. 작동 및 기타 이벤트 구독 실행(newModule.OnAttachedToShip(this))
        // 6. 부착 위치에 따른 새로운 DockingPort 생성
    }

    public void OnModuleDetached(Module oldModule)
    {
        float m = oldModule.GetDefMass();
        float M = rigid.mass;                 // 떼기 전 질량
        Vector3 c = rigid.centerOfMass;
        Vector3 r = oldModule.transform.localPosition;

        float newM = M - m;
        rigid.mass = Mathf.Max(0.0001f, newM);
        rigid.centerOfMass = (newM > 0f) ? ((M * c - m * r) / newM) : Vector3.zero;
        Debug.Log($"[Ship] COM : {rigid.centerOfMass}");

        // ShipGrid에 모듈 분리 작업 지시
        OnTryUndock?.Invoke(oldModule.gameObject.GetComponent<Collider2D>());

        RefreshShip();
    }

    private void OnShipDestroyed()
    {
        // 함선 파괴
    }

    private void RefreshShip()
    {
        // 함선 정보 갱신
        // 1. 모듈 콜라이더 갱신
        Module[] modules = GetComponentsInChildren<Module>();
        var list = new List<Collider2D>();

        foreach (var module in modules)
        {
            if (module == null) continue;

            var cols = module.GetComponents<Collider2D>();
            foreach (var col in cols)
            {
                if (col != null) list.Add(col);
            }
        }

        moduleColliders = list.ToArray();
    }

    [ContextMenu("Add AngularForce")]
    void AddAngularForce()
    {
        if(!rigid) return;

        rigid.AddTorque(torque, ForceMode2D.Impulse);
    }
}
