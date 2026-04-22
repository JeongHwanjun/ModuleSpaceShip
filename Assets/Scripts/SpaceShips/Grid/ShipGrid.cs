using System;
using System.Collections.Generic;
using UnityEngine;
using ModuleSpaceShip.Utilities;

public class ShipGrid : MonoBehaviour
{
    [Header("Grid Config")]
    [SerializeField] private float cellSize = 1f; // 월드에서 한 칸 크기
    [SerializeField] private Transform gridRoot;  // 생성되는 오브젝트 부모(자기 자신)
    [SerializeField] private Transform coreRoot; // 코어 오브젝트 부모(ship)

    [Header("Prefabs")]
    [SerializeField] private GameObject dockingPortPrefab;
    [SerializeField] private GameObject corePrefab; // 코어 프리팹

    private readonly Dictionary<GridPos, GameObject> Modules = new(); // 선체
    private readonly Dictionary<GridPos, GameObject> ports = new(); // 도킹 포트

    private Ship ship;
    private DockReceiver dockReceiver;

    void Awake()
    {
        ship = GetComponentInParent<Ship>();
        dockReceiver = GetComponent<DockReceiver>();
    }

    void Start()
    {


        ship.OnTryDockAtPort += OnTryDockAtPort;
        ship.OnTryUndock += OnTryUndock;

        // 코어 생성
        SpawnCoreModule();
    }

    void OnDestroy()
    {
        ship.OnTryDockAtPort -= OnTryDockAtPort;
        ship.OnTryUndock -= OnTryUndock;
        
    }

    // ---- 좌표 변환 ----
    public Vector3 GridToVector(GridPos p)
    {
        // 2D면 x,y를 x,z로 매핑하는 게 보통임. 필요에 맞게 수정.
        return new Vector3(p.x * cellSize,  p.y * cellSize, 0f);
    }

    // ---- 조회 ----
    public bool HasModule(GridPos p) => Modules.ContainsKey(p);
    public bool HasPort(GridPos p) => ports.ContainsKey(p);
    public bool IsOccupied(GridPos p) => HasModule(p) || HasPort(p);

    // ---- 포트 생성 ----
    public bool TryCreatePort(GridPos p)
    {
        if (IsOccupied(p)) return false;
        if (dockingPortPrefab == null) return false;

        var go = Instantiate(dockingPortPrefab);
        go.transform.SetParent(gridRoot);
        go.transform.localPosition = GridToVector(p);
        go.transform.localRotation = Quaternion.identity;
        go.name = $"DockingPort {p}";
        ports.Add(p, go);
        return true;
    }

    // ---- 포트 삭제 ----
    public bool TryDestroyPort(GridPos p)
    {
        if(HasModule(p) || !HasPort(p)) return false;

        GameObject oldPort = ports[p];
        ports.Remove(p);
        Destroy(oldPort);

        return true;
    }

    // ---- 초기 Module 등록 (시작 선체 등) ----
    public bool TryPlaceModule(GridPos p, GameObject Module)
    {
        if (Module == null) return false;
        if (IsOccupied(p)) return false;

        Module.transform.SetParent(gridRoot, true);
        Module.transform.localPosition = GridToVector(p);
        Module.name = $"Module {p}";
        Module.tag = TagHandle.GetExistingTag("PlayerShip").ToString();
        Module.GetComponent<Module>().OnAttached(gridRoot, Vector3.zero);
        Modules.Add(p, Module);

        // Module 놓인 후 주변 포트 후보 생성
        CreatePortsAroundModule(p);
        return true;
    }

    // ---- 도킹 이벤트 대응 ----
    private void OnTryDockAtPort(Collider2D newModule)
    {
        DockingPort targetPort = dockReceiver.PickBestPort(newModule);
        if(targetPort == null)
        {
            Debug.LogWarning($"[ShipGrid] Can't find proper port for : {newModule.name}");
            return;
        }
        TryDockModuleAtPort(targetPort.mountingPoint, newModule.gameObject);

        // 도킹 후 Module에 대한 후보 port를 제거함
        dockReceiver.ClearPorts(newModule);
    }

    // ---- 도킹: 특정 포트 위치에 새 Module 결합 ----
    private bool TryDockModuleAtPort(GridPos portPos, GameObject ModuleToPlace)
    {
        Debug.Log($"[ShipGrid] Try docking at : {portPos}");
        if (ModuleToPlace == null) return false;

        // 포트가 실제 존재해야 도킹 가능(원하는 룰)
        if (!ports.TryGetValue(portPos, out var portGO) || portGO == null)
            return false;

        // 해당 셀은 포트가 있으니 점유로 간주되지만, 도킹에서는 "포트를 Module로 치환"해야 함
        // 포트 제거
        ports.Remove(portPos);
        Destroy(portGO);

        // Module 배치
        ModuleToPlace.GetComponent<Module>().OnAttached(gridRoot, GridToVector(portPos));
        // Modules 목록에 추가
        Modules.Add(portPos, ModuleToPlace);

        // 새 Module 주변 포트 생성 시도
        CreatePortsAroundModule(portPos);

        return true;
    }

    // ---- 언도킹 이벤트 대응 ----
    private void OnTryUndock(Collider2D oldModule)
    {
        if (!oldModule)
        {
            Debug.LogError($"[ShipGrid] Null collider received while Undocking");
            return;
        }
        Transform targetModule = oldModule.gameObject.transform;
        GridPos targetGridPos = new GridPos(Mathf.RoundToInt(targetModule.localPosition.x), Mathf.RoundToInt(targetModule.localPosition.y));
        TryUndockModule(targetGridPos);
    }

    private bool TryUndockModule(GridPos ModulePos)
    {
        if(!Modules.TryGetValue(ModulePos, out var targetModule) || !targetModule)
        {
            Debug.LogError($"[ShipGrid] No such Module in Modules : {ModulePos}");
            return false;
        }

        if(ModulePos.Equals(new GridPos(0, 0)))
        {
            Debug.LogWarning($"[ShipGrid] Cannot Undock Core");
            return false;
        }

        // Module 분리
        Debug.Log($"[ShipGrid] Trying detach Module : {targetModule}");
        DetachModule(ModulePos, targetModule.transform, false);

        // core 연결성 확인 및 연쇄 언도킹
        HashSet<GridPos> connectedModules = GetConnectedModulesFromCore(new GridPos(0, 0));
        List<GridPos> disconnectedModules = GetDisconnectedModules(new GridPos(0, 0), connectedModules);
        Debug.LogWarning($"[ShipGrid] ConnectedModules : {connectedModules}");
        foreach(var Module in disconnectedModules)
        {
            DetachModule(Module, Modules[Module].transform, true);
        }

        // 남은 Module에 대해 port 유효성 검사 및 재생성
        foreach(var Module in connectedModules)
        {
            Debug.Log($"[ShipGrid] ConnectedModule element : {Module}");
            CreatePortsAroundModule(Module);
        }

        return true;
    }

    private void CreatePortsAroundModule(GridPos ModulePos)
    {
        foreach (var d in GridRangeUtilities.Card4)
        {
            var candidate = ModulePos + d;
            TryCreatePort(candidate);
        }
    }

    private void DestroyPortsAroundModule(GridPos ModulePos)
    {
        foreach (var d in GridRangeUtilities.Card4)
        {
            var candidate = ModulePos + d;
            if(!TryDestroyPort(candidate)) Debug.LogWarning($"Couldn't Destroy Port : {candidate}");
        }
    }

    // ---- 선체 분리 처리 ----
    private void DetachModule(GridPos ModulePos, Transform targetModule, bool isChained)
    {
        // 선체의 분리 함수 실행, 부모 해제/태그 갱신/상태 갱신/rigidbody 수정 등
        targetModule.GetComponent<Module>().OnDetached(isChained);
        // 주변 포트 파괴
        DestroyPortsAroundModule(ModulePos);
        // Modules 목록에서 삭제
        Modules.Remove(ModulePos);
    }

    // ---- 코어로부터 연결된 항목 반환 ----
    private HashSet<GridPos> GetConnectedModulesFromCore(GridPos corePos)
    {
        var connected = new HashSet<GridPos>();
        if (!Modules.ContainsKey(corePos)) return connected;

        var q = new Queue<GridPos>();
        q.Enqueue(corePos);
        connected.Add(corePos);

        while (q.Count > 0)
        {
            var cur = q.Dequeue();

            foreach (var d in GridRangeUtilities.Card4)
            {
                var next = cur + d;

                // Port는 무시, Module-Module만 연결
                if (!Modules.ContainsKey(next)) continue;

                // 이미 방문했으면 스킵
                if (!connected.Add(next)) continue;

                q.Enqueue(next);
            }
        }

        return connected;
    }

    // ---- 연결 안 된 Module 좌표 목록 반환 ----
    private List<GridPos> GetDisconnectedModules(GridPos corePos)
    {
        var connected = GetConnectedModulesFromCore(corePos);
        return GetDisconnectedModules(corePos, connected);
    }

    private List<GridPos> GetDisconnectedModules(GridPos corePos, HashSet<GridPos> connectedModules)
    {
        var connected = connectedModules;
        var disconnected = new List<GridPos>();

        foreach (var kv in Modules)
        {
            if (!connected.Contains(kv.Key))
                disconnected.Add(kv.Key);
        }

        return disconnected;
    }

    // ---- 코어 생성 ----
    [ContextMenu("Spawn Core Module at (0,0)")]
    private void SpawnCoreModule()
    {
        if (corePrefab == null || coreRoot == null) return;
        var Module = Instantiate(corePrefab);
        Debug.Log($"[ShipGrid] TryPlaceModule result : {TryPlaceModule(new GridPos(0,0), Module)}");
    }
}
