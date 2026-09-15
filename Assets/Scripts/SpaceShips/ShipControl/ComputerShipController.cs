using System.Collections.Generic;
using UnityEngine;

/// <summary>함선별 관계를 분류하고 적대 표적까지만 선정한다.</summary>
[RequireComponent(typeof(ComputerShip))]
public class ComputerShipController : MonoBehaviour
{
    private ComputerShip computerShip;
    private bool relationsDirty = true;

    [SerializeField] private List<Ship> friendlyShips = new();
    [SerializeField] private List<Ship> neutralShips = new();
    [SerializeField] private List<Ship> hostileShips = new();
    [SerializeField] private Ship currentTarget;

    public Ship CurrentTarget => currentTarget;
    public bool HasTarget => IsHostileCandidate(currentTarget);

    private void Awake()
    {
        computerShip = GetComponent<ComputerShip>();
    }

    private void OnEnable()
    {
        computerShip.RelationsChanged += MarkRelationsDirty;
        relationsDirty = true;
    }

    private void OnDisable()
    {
        computerShip.RelationsChanged -= MarkRelationsDirty;
        friendlyShips.Clear();
        neutralShips.Clear();
        hostileShips.Clear();
        currentTarget = null;
        computerShip.ClearControlIntent();
    }

    private void MarkRelationsDirty()
    {
        relationsDirty = true;
    }

    private void LateUpdate()
    {
        if (relationsDirty || (currentTarget != null && !HasTarget))
            RefreshTarget();
    }

    public void RefreshTarget()
    {
        relationsDirty = false;
        friendlyShips.Clear();
        neutralShips.Clear();
        hostileShips.Clear();

        ShipManager manager = ShipManager.Instance;
        if (computerShip != null && !computerShip.IsDestroyed &&
            manager != null && manager.Contains(computerShip))
        {
            foreach (var entry in computerShip.Relations)
            {
                Ship candidate = entry.Key;
                if (candidate == computerShip || candidate == null || candidate.IsDestroyed ||
                    !manager.Contains(candidate)) continue;

                switch (entry.Value)
                {
                    case RelationType.Friendly: friendlyShips.Add(candidate); break;
                    case RelationType.Neutral: neutralShips.Add(candidate); break;
                    case RelationType.Hostile: hostileShips.Add(candidate); break;
                }
            }
        }

        // 유효한 기존 표적은 유지한다.
        if (!hostileShips.Contains(currentTarget))
        {
            currentTarget = null;
            foreach (Ship candidate in hostileShips)
                if (currentTarget == null)
                    currentTarget = candidate;
        }

        // 정지는 추력·회전·사격 명령 해제이다. 물리 속도(관성)는 변경하지 않는다.
        if (currentTarget == null && computerShip != null)
            computerShip.ClearControlIntent();
    }

    private bool IsHostileCandidate(Ship candidate)
    {
        ShipManager manager = ShipManager.Instance;
        return computerShip != null && !computerShip.IsDestroyed &&
            candidate != null && candidate != computerShip && !candidate.IsDestroyed &&
            manager != null && manager.Contains(computerShip) && manager.Contains(candidate) &&
            computerShip.GetRelation(candidate) == RelationType.Hostile;
    }

    [ContextMenu("Print Target Name")]
    public void PrintTargetName()
    {
        if (HasTarget)
        {
            Debug.Log($"[ComputerShipController] Target Name : {currentTarget.name}");
        }
        else
        {
            Debug.Log($"[ComputerShipController] {gameObject.name} has no currentTarget");
        }
    }
}
