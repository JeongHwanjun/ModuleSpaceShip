using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public enum ShipRemovalReason
{
    Removed,        // 일반 Destroy, 씬에서 제거
    Destroyed,      // Ship.DestroyShip()으로 전투 파괴
    ManagerShutdown // Manager 자체 종료 (전투 파괴가 아님)
}

/// <summary>
/// 전장 하나의 함선 등록소. 전장에 활성 Manager 하나를 먼저 배치한다.
/// 세력 관계로 함선별 관계를 초기화한다. 목표 선정은 Controller가 담당한다.
/// 모든 API는 Unity 메인 스레드용이다.
/// </summary>
[DefaultExecutionOrder(-1000)]
[DisallowMultipleComponent]
public sealed class ShipManager : MonoBehaviour
{
    public static ShipManager Instance { get; private set; }

    [SerializeField] private FactionRelationTable factionRelations;

    private readonly Dictionary<int, Ship> ships = new();
    private ReadOnlyDictionary<int, Ship> readOnlyShips;
    private bool shuttingDown;

    public int Count => ships.Count;

    // 실시간 읽기 전용 뷰. 순회 중 등록/해제할 때는 CopyShipsTo를 사용한다.
    public IReadOnlyDictionary<int, Ship> Ships =>
        readOnlyShips ??= new ReadOnlyDictionary<int, Ship>(ships);

    public event Action<Ship> ShipRegistered; // Ship 등록시 발생하는 이벤트
    public event Action<int, Ship, ShipRemovalReason> ShipRemoved;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        Instance = null;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("[ShipManager] 전장에 Manager는 하나만 배치해야 합니다.", this);
            Destroy(this); // 같은 오브젝트의 다른 컴포넌트는 보존
            return;
        }

        Instance = this;
    }

    // Ship의 생명주기에서만 호출한다. 외부에서는 DestroyShip/Destroy로 제거한다.
    internal bool Register(Ship ship)
    {
        if (shuttingDown || ship == null || ship.IsDestroyed || ship.ShipId == 0)
            return false;

        if (ships.TryGetValue(ship.ShipId, out Ship existing))
        {
            if (existing == ship) return true; // 중복 알림 방지
            Debug.LogError($"[ShipManager] 중복 ID: {ship.ShipId}", this);
            return false;
        }

        ships.Add(ship.ShipId, ship);
        // 등록 이벤트가 전달될 때에는 양쪽 함선의 관계가 모두 준비되어 있다.
        foreach (Ship other in new List<Ship>(ships.Values))
        {
            if (other == ship || other == null || other.IsDestroyed) continue;
            RelationType relation = factionRelations != null
                ? factionRelations.GetRelation(ship.Faction, other.Faction)
                : (ship.Faction == other.Faction ? RelationType.Friendly : RelationType.Neutral);
            ship.SetRelation(other, relation);
            other.SetRelation(ship, relation);
        }
        NotifyRegistered(ship);
        return true;
    }

    internal bool Unregister(int shipId, Ship ship, ShipRemovalReason reason)
    {
        if (!ships.TryGetValue(shipId, out Ship existing) ||
            !ReferenceEquals(existing, ship))
            return false;

        ships.Remove(shipId); // 이벤트 안에서도 조회 결과가 제거 상태여야 한다.
        foreach (Ship other in new List<Ship>(ships.Values))
            if (other != null) other.RemoveRelation(ship);
        ship.ClearRelations();
        NotifyRemoved(shipId, ship, reason);
        return true;
    }

    public bool TryGetShip(int shipId, out Ship ship)
    {
        if (ships.TryGetValue(shipId, out ship) && ship != null && !ship.IsDestroyed)
            return true;

        ship = null;
        return false;
    }

    public bool Contains(Ship ship)
    {
        return ship != null && TryGetShip(ship.ShipId, out Ship existing) && existing == ship;
    }

    /// <summary>재사용할 List를 전달하면 순회 중 원본의 등록/해제가 가능하다.</summary>
    public void CopyShipsTo(List<Ship> destination)
    {
        if (destination == null) throw new ArgumentNullException(nameof(destination));
        destination.Clear();
        foreach (Ship ship in ships.Values)
            if (ship != null && !ship.IsDestroyed) destination.Add(ship);
    }

    private void OnDestroy()
    {
        if (Instance != this) return;

        shuttingDown = true;
        Instance = null;
        var remaining = new List<KeyValuePair<int, Ship>>(ships);
        ships.Clear();
        foreach (var entry in remaining)
            if (entry.Value != null) entry.Value.ClearRelations();
        foreach (var entry in remaining)
            NotifyRemoved(entry.Key, entry.Value, ShipRemovalReason.ManagerShutdown);

        ShipRegistered = null;
        ShipRemoved = null;
    }

    // 구독자 하나의 예외가 등록/제거 또는 다른 구독자의 처리를 중단시키지 않게 한다.
    private void NotifyRegistered(Ship ship)
    {
        if (ShipRegistered == null) return;
        foreach (Action<Ship> handler in ShipRegistered.GetInvocationList())
        {
            try { handler(ship); }
            catch (Exception exception) { Debug.LogException(exception, this); }
        }
    }

    private void NotifyRemoved(int id, Ship ship, ShipRemovalReason reason)
    {
        if (ShipRemoved == null) return;
        foreach (Action<int, Ship, ShipRemovalReason> handler in ShipRemoved.GetInvocationList())
        {
            try { handler(id, ship, reason); }
            catch (Exception exception) { Debug.LogException(exception, this); }
        }
    }
}
