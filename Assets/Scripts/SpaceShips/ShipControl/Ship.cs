using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ModuleSpaceShip.Runtime;
using UnityEngine;

// 함선 그 자체를 뜻하는 클래스
public abstract class Ship : MonoBehaviour
{
    // 실행 중 식별자. 저장/불러오기용 영구 ID가 아니다.
    public int ShipId { get; private set; }
    public bool IsDestroyed => isBeginDestroy;
    private ShipManager registeredManager;

    [SerializeField] private FactionId faction = FactionId.Independent;
    public FactionId Faction => faction;

    // 함선별 관계의 원본. 세력 테이블은 생성 시 기본값으로만 사용한다.
    private readonly Dictionary<Ship, RelationType> relations = new();
    private ReadOnlyDictionary<Ship, RelationType> readOnlyRelations;
    public IReadOnlyDictionary<Ship, RelationType> Relations =>
        readOnlyRelations ??= new ReadOnlyDictionary<Ship, RelationType>(relations);
    public event Action RelationsChanged;

    public RelationType GetRelation(Ship other)
    {
        return other != null && relations.TryGetValue(other, out var relation)
            ? relation : RelationType.Neutral;
    }

    /// <summary>이 함선이 상대를 보는 관계만 변경한다. 상대의 관계는 별도로 변경한다.</summary>
    public void SetRelation(Ship other, RelationType relation)
    {
        if (!Enum.IsDefined(typeof(RelationType), relation))
            throw new ArgumentOutOfRangeException(nameof(relation));
        if (IsDestroyed || other == null || other == this || other.IsDestroyed ||
            registeredManager == null || !registeredManager.Contains(this) ||
            !registeredManager.Contains(other)) return;

        if (relations.TryGetValue(other, out RelationType current) && current == relation) return;
        relations[other] = relation;
        NotifyRelationsChanged();
    }

    internal void RemoveRelation(Ship other)
    {
        if (relations.Remove(other)) NotifyRelationsChanged();
    }

    internal void ClearRelations()
    {
        if (relations.Count == 0) return;
        relations.Clear();
        NotifyRelationsChanged();
    }

    private void NotifyRelationsChanged()
    {
        // 구독자 예외가 Manager의 등록/제거 절차를 중단시키지 않게 한다.
        if (RelationsChanged == null) return;
        foreach (Action handler in RelationsChanged.GetInvocationList())
        {
            try { handler(); }
            catch (Exception exception) { Debug.LogException(exception, this); }
        }
    }

    [DefName("ShipDef")]
    [SerializeField] private string def;
    [SerializeField] private ShipThing shipThing;
    [SerializeField] public string shipName = "ship";
    public Vector2 heading
    {
        get
        {
            return (Vector2)transform.up;
        }
    }
    private Rigidbody2D rigid;
    private ShipGrid shipGrid;
    private readonly ThrusterCalculator thrusterCalculator = new();
    private readonly List<Thruster> thrusters = new();
    private Vector2 currentMoveIntent;
    private float currentTurnIntent;
    private bool isBeginDestroy = false;
    public Collider2D[] moduleColliders; // 함선을 구성하고 있는 module의 collider2d;


    // ---- Events ----
    public event Action<GameObject, Collider2D> OnTryDockAtPort;
    public event Action<GameObject, Collider2D> OnTryUndock;
    public event Action OnTryFireStart;
    public event Action OnTryFireStop;

    protected virtual void Awake()
    {
        shipThing = (ShipThing)ThingFactory.CreateFromDefName(def);
        rigid = GetComponent<Rigidbody2D>();
        shipGrid = GetComponentInChildren<ShipGrid>();

        ShipId = GetInstanceID();
        registeredManager = ShipManager.Instance;
        if (registeredManager == null)
        {
            Debug.LogError("[Ship] 함선 생성 전에 활성 ShipManager를 배치해야 합니다.", this);
        }
        else if (!registeredManager.Register(this))
        {
            registeredManager = null;
            Debug.LogError("[Ship] ShipManager 등록 실패.", this);
        }
    }

    protected void OnMouseReleaseWithModule(GameObject module, Collider2D col)
    {
        if (IsDestroyed) return;
        OnTryDockAtPort?.Invoke(module, col);
    }
    protected void OnMouseClickWithPlayerModule(GameObject oldModule, Collider2D col)
    {
        if (IsDestroyed) return;
        OnTryUndock?.Invoke(oldModule, col);
    }
    protected void OnModuleDetachedByDestroy(GameObject oldModule, Collider2D col)
    {
        OnTryUndock?.Invoke(oldModule, col);
    }
    protected void OnMouseClickStartWithVoid()
    {
        if (IsDestroyed) return;
        OnTryFireStart?.Invoke();
    }
    protected void OnMouseClickEndWithVoid()
    {
        OnTryFireStop?.Invoke();
    }

    public void SetControlIntent(ShipControlIntent intent)
    {
        if (IsDestroyed) return;
        currentMoveIntent = intent.movement;
        currentTurnIntent = intent.turn;

        if(intent.fire)
            OnTryFireStart?.Invoke();
        else
            OnTryFireStop?.Invoke();
    }

    public void ClearControlIntent()
    {
        currentMoveIntent = Vector2.zero;
        currentTurnIntent = 0f;
        OnTryFireStop?.Invoke();
    }

    public void OnModuleDestroyed(Module oldModule)
    {
        // 선체가 파괴되었을 때 해당 선체에서 호출함
        // 0. 코어 선체인지 확인(코어 선체일 경우 사망)
        // 1. 분리 진행
        OnModuleDetachedByDestroy(oldModule.gameObject, null);
        // oldModule.OnDetached(false);
        OnModuleDetached(oldModule);
    }

    public void OnModuleAttached(Module newModule)
    {
        // 선체가 부착되었을 때 해당 선체에서 호출함
        // 1. 해당 선체의 정보 참조
        // 2. 정보를 바탕으로 Thing의 정보 갱신
        // 3. rigidbody 정보 갱신
        if(!rigid) rigid = GetComponent<Rigidbody2D>();
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
        // 함선 정보만 갱신
        float m = oldModule.GetDefMass();
        float M = rigid.mass;                 // 떼기 전 질량
        Vector3 c = rigid.centerOfMass;
        Vector3 r = oldModule.transform.localPosition;

        float newM = M - m;
        rigid.mass = Mathf.Max(0.0001f, newM);
        rigid.centerOfMass = (newM > 0f) ? ((M * c - m * r) / newM) : Vector3.zero;
        Debug.Log($"[Ship] COM : {rigid.centerOfMass}");

        RefreshShip();
    }

    public void OnShipDestroyed()
    {
        if (IsDestroyed) return;

        // Grid 정리 중 재진입하더라도 파괴/제거 알림을 중복 처리하지 않는다.
        RequestShipDestroy();
        UnregisterFromManager(ShipRemovalReason.Destroyed);
        try
        {
            ClearControlIntent();
        }
        finally
        {
            try
            {
                // 최신 코드의 모듈 정리 흐름을 유지한다.
                if (shipGrid != null) shipGrid.OnShipDestroyed();
            }
            finally
            {
                // 활성 함선은 기존처럼 Update에서 실제 파괴한다.
                // 비활성 함선에는 Update가 없으므로 여기서 파괴를 예약한다.
                if (!isActiveAndEnabled) FinalizeDestroy();
            }
        }
    }

    // 이전 ShipManager 사용 예제와도 호환되는 진입점.
    public void DestroyShip()
    {
        OnShipDestroyed();
    }

    private void RequestShipDestroy()
    {
        isBeginDestroy = true;
    }

    private void FinalizeDestroy()
    {
        Destroy(gameObject);
    }

    protected virtual void OnDisable()
    {
        // 파괴 요청 뒤 Update 전에 비활성화된 경우에도 파괴를 완료한다.
        // 단순 비활성화는 등록을 유지한다.
        if (IsDestroyed) FinalizeDestroy();
    }

    protected virtual void OnDestroy()
    {
        // 일반 Destroy/씬 제거 대응. 전투 파괴로 이미 해제했다면 아무 일도 하지 않는다.
        UnregisterFromManager(ShipRemovalReason.Removed);
    }

    private void UnregisterFromManager(ShipRemovalReason reason)
    {
        ShipManager manager = registeredManager;
        registeredManager = null;
        if (manager != null)
            manager.Unregister(ShipId, this, reason);
    }

    public void OnSetCurrentIntent(Vector2 movement, float torque)
    {
        if (IsDestroyed) return;
        Debug.Log($"[Ship] Received data : {movement}, {torque}");
        currentMoveIntent = movement;
        currentTurnIntent = torque;
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

            var cols = module.GetComponentsInChildren<Collider2D>();
            foreach (var col in cols)
            {
                if (col != null) list.Add(col);
            }
        }
        moduleColliders = list.ToArray();
        
        // 2. 추진기 정보 갱신
        thrusters.Clear();
        GetComponentsInChildren(thrusters);
        foreach(Thruster thruster in thrusters) thruster.MarkTargetsDirty(); // 목표 모듈 최신화 필요성 표시
        thrusterCalculator.Rebuild(
            thrusters,
            rigid.centerOfMass,
            moveWeight: 1f,
            turnWeight: 1f
        );
    }

    // ---- 주어진 Grid를 ShipGrid에 전달하여 해당 위치에 존재하는 Module List반환 ----
    public Collider2D[] GetModulesByGrid(GridPos[] requestGrid)
    {
        if(!shipGrid) return null;

        List<Collider2D> gridColliders = new();
        foreach(GridPos grid in requestGrid)
        {
            if(!shipGrid.HasModule(grid)) continue;
            
            GameObject targetModule = shipGrid.GetModuleByGridPos(grid);
            if(!targetModule) continue;

            gridColliders.Add(targetModule.GetComponent<Collider2D>());
        }
        
        return gridColliders.ToArray();
    }

    protected virtual void FixedUpdate()
    {
        if (IsDestroyed || !rigid) return;

        var commands = thrusterCalculator.GetCommands(currentMoveIntent, currentTurnIntent);

        foreach(Thruster t in thrusters)
        {
            t.SetThrusterFlameVisibility(0f);
        }
        foreach (var command in commands)
        {
            command.thruster.Ignite(rigid, command.throttle);
            command.thruster.SetThrusterFlameVisibility(command.throttle);
        }
    }

    protected virtual void Update()
    {
        if(isBeginDestroy) FinalizeDestroy();
    }

    [ContextMenu("Save This Ship as shipName")]
    void SaveThisShip()
    {
        ShipBlueprintSerializer.SerializeBlueprint(this);
    }

    [ContextMenu("Print Deserialized ship 'playerShip'")]
    void PrintShip()
    {
        Debug.Log($"[Ship] ship XML : {ShipBlueprintSerializer.DeserializeBlueprintByName("playerShip")}");
    }
}
