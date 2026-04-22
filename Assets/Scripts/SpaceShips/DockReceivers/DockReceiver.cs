using System.Collections.Generic;
using UnityEngine;

public class DockReceiver : MonoBehaviour
{
    private Ship ship;

    // 다수의 Port에 동시에 겹쳐있을 경우 거리로 우선순위를 정하기 위한 dict
    private readonly Dictionary<Collider2D, HashSet<DockingPort>> overlaps = new();

    void Start()
    {
        ship = GetComponentInParent<Ship>();
    }

    public void NotifyTriggerEnter(DockingPort port, Collider2D other)
    {
        Module module = other.GetComponentInParent<Module>();
        if(module == null) return;

        Collider2D key = other;
        if (!overlaps.TryGetValue(key, out var set))
        {
            set = new HashSet<DockingPort>();
            overlaps[key] = set;
        }
        set.Add(port);

        Debug.Log($"[DockReceiver] Enlist new module : {other.name}");
    }

    public void NotifyTriggerExit(DockingPort port, Collider2D other)
    {
        Collider2D key = other;
        if (!overlaps.TryGetValue(key, out var set)) return;

        set.Remove(port);
        if (set.Count == 0) overlaps.Remove(key);

        Debug.Log($"[DockReceiver] Release old hull : {other.name}");
    }

    public DockingPort PickBestPort(Collider2D targetHull, IEnumerable<DockingPort> candidatePorts)
    {
        // 클릭을 끝냈을 때 실행되어 TryDockHullAtPort로 해당 도킹 포트의 GridPos를 넘겨줘야 함
        DockingPort best = null;
        float bestScore = float.NegativeInfinity;

        foreach (var p in candidatePorts)
        {
            if (!p)
            {
                Debug.LogWarning($"[DockReceiver] Adjust port is already destroyed. skipping check");
                continue;
            }
            var d = targetHull.Distance(p.Col);
            if (!d.isOverlapped) continue;

            float score = -d.distance; // 깊게 겹칠수록 큼
            if (score > bestScore)
            {
                bestScore = score;
                best = p;
            }
        }
        return best;
    }

    public DockingPort PickBestPort(Collider2D targetHull)
    {
        if(!overlaps.TryGetValue(targetHull, out var ports))
        {
            Debug.LogWarning($"[DockReceiver] Collider is not in candidate : {targetHull.name}");
            return null;
        }
        return PickBestPort(targetHull, ports);
    }

    public void ClearPorts(Collider2D targetHull)
    {
        if (!overlaps.ContainsKey(targetHull))
        {
            Debug.LogWarning($"[DockReceiver] Collider is already removed : {targetHull.name}");
            return;
        }
        Debug.Log($"[DockReceiver] Remove Ports for {targetHull.name}");

        overlaps.Remove(targetHull);
    }
}
