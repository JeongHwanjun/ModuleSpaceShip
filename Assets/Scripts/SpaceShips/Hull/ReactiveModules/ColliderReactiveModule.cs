using System.Linq;
using ModuleSpaceShip.Runtime;
using UnityEngine;
public abstract class ColliderReactiveModule : ReactiveModule
{
    protected ColliderReactiveModuleThing colliderReactiveModuleThing => (ColliderReactiveModuleThing)reactiveModuleThing;

    // public GameObject colliderObject; // 충돌 콜라이더를 포함하는 오브젝트

    protected override void Awake()
    {
        base.Awake();
        targetModuleColliders.Clear();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // 도크 포트는 무시
        if(other.CompareTag("DockPort")) return;
        targetModuleColliders.Add(other);
        Debug.Log($"[ColliderReactiveModule] TriggerEnter : {other.name}, targetModuleColliders Length : {targetModuleColliders.Count}");
        GetTargetModulesFromColliders();
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        // 도크 포트는 무시
        if(other.CompareTag("DockPort")) return;
        targetModuleColliders = targetModuleColliders.Where(collider => collider != other).ToList();
        Debug.Log($"[ColliderReactiveModule] TriggerExit : {other.name}");
        GetTargetModulesFromColliders();
    }
}