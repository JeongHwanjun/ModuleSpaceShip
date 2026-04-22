using System;
using System.Collections.Generic;
using System.Linq;
using ModuleSpaceShip.Defs;
using ModuleSpaceShip.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public abstract class Module : BaseMonobehaviour
{
    /* Def 및 컴포넌트 */
    protected abstract string DefName { get; }
    protected InputManager inputManager;
    protected ModuleThing moduleThing;
    private Rigidbody2D rigid;
    protected Ship ship; // 부착된 함선 스크립트
    private Collider2D col;

    /* 적용 효과들 */
    private List<EffectBase> effects;

    /* 디버깅 요소들 */
    [SerializeField] private bool ShowDirection = false;
    public GameObject DirectionIndicator;

    /* 물리 속성 스케일 */
    public float DetachForceScale = 1.0f;
    public float GrabMaxSpeed = 30.0f;
    public float GrabArriveRadius = 3.0f;
    public float GrabAccel = 60.0f;

    /* 플래그 */
    private bool initialized = false;
    private bool isBeginDestroy = false;

    public virtual void Init(ThingBase thingBase)
    {
        // 생성될 때 실행되는 초기화
        moduleThing = (ModuleThing)thingBase;
        initialized = true;
        effects = new();
    }
    protected virtual void Awake()
    {
        // 만약 Init이 실행되지 않았다면 여기서 실행함
        if (!initialized)
        {
            Debug.LogWarning($"[Module] No Def received, using fallback.");
            Init(ThingFactory.CreateFromDefName(DefName));
        }
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    protected virtual void Start()
    {
        inputManager = InputManager.Instance;
        // ship = null;

        // rigidbody 초기화
        InitializeRigidbody(rigid);
    }

    protected virtual void OnValidate()
    {
        if (DirectionIndicator != null) DirectionIndicator.SetActive(ShowDirection);
    }

    private void Update()
    {
        // 매초 적용되는 버프를 적용
        for(int i = effects.Count -1; i >= 0; i--)
        {
            if(i >= effects.Count) continue; // 도중 삭제시 대응
            effects[i].WorkOnUpdate(this);
        }

        if(isBeginDestroy) FinalizeDestroy();
    }

    protected virtual void FixedUpdate()
    {
        if(moduleThing.state == ModuleThing.State.Grabbed && rigid != null)
        {
            MoveTowardMouse();
        }
        else
        {
            // do nothing;
        }
    }

    // ---- 모듈을 클릭했을 경우 호출됨 ----
    public override void OnClickStart()
    {
        Debug.Log($"[Module] OnClickStart");
        // 부모 해제
        transform.SetParent(null);
        // Thing에서 현재 상태 판단
        moduleThing.state = ModuleThing.State.Grabbed;
        gameObject.layer = LayerMask.NameToLayer("GrabbedModule");
        // 질량 변경
        rigid.mass = 0.0001f;
        Debug.Log($"[Module] moduleThing.State : {moduleThing.state}");
    }

    // ---- 마우스로 모듈을 잡고 있던 상태에서 놓을 때 호출됨 ----
    public override void OnClickCancel(Collider2D hit)
    {
        Debug.Log($"[Module] OnClickCancel, hit : {hit}");
        // Thing에서 현재 상태 판단 및 변화
        // hit = 마우스를 놓을 당시 마우스 위치에 있던 object임. 
        if(moduleThing.state == ModuleThing.State.Grabbed) // 1. 연결되지 않은 상태
        {
            moduleThing.state = ModuleThing.State.Floating;
        }
        else if(moduleThing.state == ModuleThing.State.Connected) // 2. 연결된 상태
        {
            // do nothing;
        }
        // 질량 변경
        rigid.mass = moduleThing.def.mass;

        gameObject.layer = LayerMask.NameToLayer("Module");
    }

    // ---- 부착시 ShipGrid에서 실행되어 초기화 등 담당 ----
    public virtual void OnAttached(Transform parent, Vector3 position)
    {
        Debug.Log($"[Module] Trying Destroy rigid");
        // 함선에 부착되었을 때 실행(마우스를 놓은 상태)
        // 0. 부모 설정, 위치/이름/태그 변경
        transform.SetParent(parent, true);
        transform.localPosition = position;
        transform.localRotation = Quaternion.identity;
        ship = GetComponentInParent<Ship>();
        gameObject.name = $"Module ({position.x}, {position.y})"; // 디버깅용 이름변경
        gameObject.tag = TagHandle.GetExistingTag("PlayerShip").ToString();
        // 마우스 위치 참조해 방향 설정
        SetHeading();
        // 1. 입력 이벤트 구독
        // 2. Thing 상태 갱신
        moduleThing.state = ModuleThing.State.Connected;
        // 3. rigidbody 파괴
        Destroy(rigid);
        // 4. 만약 모듈이라면 자신의 상태에 맞게 코드를 변형함...
        ship.OnModuleAttached(this);
    }

    // ---- 탈착시 ShipGrid에서 실행되어 초기화 등 담당 ----
    public virtual void OnDetached(bool isChained)
    {
        // 함선에서 떨어져나갈 때 실행(아직 마우스로 잡고있는 상태)
        // 0. 부모 해제 및 태그 갱신
        transform.SetParent(null);
        gameObject.tag = TagHandle.GetExistingTag("NeutralModule").ToString();
        // 1. 입력 이벤트 구독해제
        // 2. Thing 상태 갱신
        moduleThing.state = isChained ? ModuleThing.State.Floating : ModuleThing.State.Grabbed;
        // 3. rigidbody 생성 및 초기화
        rigid = gameObject.AddComponent<Rigidbody2D>();
        if(rigid) InitializeRigidbody(rigid);
        Vector2 detachForce = transform.position - ship.transform.position;
        Debug.Log($"[Module] AddForce : {detachForce}");
        // 파괴되는 중이라면 rigid가 없을 수 있음.
        if(rigid) rigid.AddForce(detachForce * DetachForceScale, ForceMode2D.Impulse);
        // 4. 만약 모듈이라면 자신의 상태에 맞게 코드를 변형함...
        ship = null;
    }

    void InitializeRigidbody(Rigidbody2D rb)
    {
        if (!rb)
        {
            Debug.LogWarning($"[Module] No Rigidbody2D for : {gameObject.name}");
            return;
        }
        ModuleDef def = moduleThing.def;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale =  def.gravityScale;
        rb.mass = def.mass;
        rb.linearDamping = def.linearDamping; // 마찰계수, 우주이므로 기본적으로 0f
        rb.angularDamping = def.angularDamping; // 회전마찰계수
    }

    void MoveTowardMouse()
    {
        Vector2 error = inputManager.mousePos - new Vector2(transform.position.x, transform.position.y);
        float dist = error.magnitude;

        if (dist < 0.001f) return;

        // 가까울수록 목표 속도를 0으로 줄임
        float t = Mathf.Clamp01(dist / GrabArriveRadius);
        float desiredSpeed = GrabMaxSpeed * t;

        Vector2 desiredVel = error.normalized * desiredSpeed;

        // 현재 속도를 desiredVel로 “맞추기 위한” 가속
        Vector2 dv = desiredVel - rigid.linearVelocity;
        Vector2 force = dv * GrabAccel * rigid.mass; // mass 고려하면 rb.mass 곱/나눔 조절 가능

        rigid.AddForce(force, ForceMode2D.Force);
    }

    // ---- 부착시 마우스 방향 기준으로 방향 결정 ----
    void SetHeading()
    {
        Vector2 desiredDirection = inputManager ? (inputManager.mousePos - (Vector2)transform.position).normalized : Vector2.up;
        Debug.Log($"[Module] heading to : {ship.heading}");
        Debug.Log($"[Module] desiredDirection : {desiredDirection}");
        float dot = Vector2.Dot(ship.heading, desiredDirection); // 벡터 내적값, 앞뒤 판단(앞 -1..1 뒤)
        float cross = ship.heading.x * desiredDirection.y - ship.heading.y * desiredDirection.x; // 좌우 판단

        if(Mathf.Abs(dot) >= Mathf.Abs(cross))
        {
            Debug.Log($"[Module] dot direction is : {dot}");
            moduleThing.direction = dot >= 0 ? ModuleThing.Direction.down : ModuleThing.Direction.up;
            // 방향 반전
            transform.localRotation = moduleThing.direction == ModuleThing.Direction.up ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 180);
        }
        else
        {
            Debug.Log($"[Module] cross direction is : {cross}");
            moduleThing.direction = cross >= 0 ? ModuleThing.Direction.right : ModuleThing.Direction.left;
            // 방향 반전
            transform.localRotation = moduleThing.direction == ModuleThing.Direction.left ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(0, 0, 270);
        }

    }

    // ---- 데미지 처리 ----
    public void DeliverDamage(float damage)
    {
        float currentHullPoint = moduleThing.AddHullPoint(damage * -1);
        Debug.Log($"[Module] Damage received : {damage}, current HP is : {currentHullPoint}");
        // 체력이 0 이하가 되었을 경우 파괴 처리
        if(currentHullPoint <= 0)
        {
            Debug.Log($"[Module] Destroying Module : {gameObject}");
            // 1. Detach 이전 처리(주변 모듈에 특정 효과 부여, ship에 특정 효과 부여 등)
            // 2. Detach 실행
            // 3. Destroy
            ModuleDestroyed();
        }
    }

    // ---- 파괴 시 호출 ----
    protected void ModuleDestroyed()
    {
        // 파괴 플래그 표시
        RequestDestroy();
    }

    public float GetDefMass()
    {
        Debug.Log($"[Module] GetDefMass : {gameObject.name}");
        // Def상 정의된 질량을 획득함
        return moduleThing.def.mass;
    }

    public void RequestDestroy()
    {
        if(isBeginDestroy) return;
        isBeginDestroy = true;
    }

    private void FinalizeDestroy()
    {
        if (moduleThing.state == ModuleThing.State.Connected)
            ship.OnModuleDestroyed(this);

        ClearEffect();
        Destroy(gameObject);
    }

    // ---- 파괴시 

    // ---- 디버그 ----
    [ContextMenu("Print Current State")]
    void PrintCurrentState()
    {
        Debug.Log($"[Module] {name} state : {moduleThing?.state}");
    }

    [ContextMenu("Add HealingEffect")]
    void AddHealingEffect()
    {
        HealingEffectThing newEffectThing = (HealingEffectThing)ThingFactory.CreateFromDefName("HealingEffect");
        HealingEffect newEffect = new HealingEffect(newEffectThing);
        newEffect.WorkOnAttach(this);
        effects.Add(newEffect);
    }

    [ContextMenu("Clear Effects")]
    void ClearEffect()
    {
        foreach(EffectBase effect in effects) effect.WorkOnDetach(this);
        effects.Clear();
    }
}
