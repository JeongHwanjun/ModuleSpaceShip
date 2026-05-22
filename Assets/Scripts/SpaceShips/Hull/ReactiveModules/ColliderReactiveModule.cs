using System;
using System.Collections.Generic;
using System.Linq;
using ModuleSpaceShip.Runtime;
using UnityEngine;
public abstract class ColliderReactiveModule : ReactiveModule
{
    protected ColliderReactiveModuleThing colliderReactiveModuleThing => (ColliderReactiveModuleThing)reactiveModuleThing;

    //[SerializeField] protected Collider2D reactiveTriggerCollider;
    protected bool targetDirty = true;
    [SerializeField] private Vector2 overlapBoxLocalCenter = Vector2.up;
    [SerializeField] private Vector2 overlapBoxSize = Vector2.one;


    protected override void Awake()
    {
        base.Awake();

        //if (!reactiveTriggerCollider)
        //    reactiveTriggerCollider = GetComponent<Collider2D>();

        targetModuleColliders.Clear();
    }

    public override void OnAttached(Transform parent, Vector3 position)
    {
        base.OnAttached(parent, position);
        RefreshTargetsByOverlap();
        MarkTargetsDirty();
    }

    public override void OnDetached(bool isChained)
    {
        base.OnDetached(isChained);
        RefreshTargetsByOverlap();
        MarkTargetsDirty();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DockPort")) return;
        if (ship == null) return;
        

        if (!targetModuleColliders.Contains(other))
            targetModuleColliders.Add(other);

        RefreshTargetsByOverlap();
        MarkTargetsDirty();
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DockPort")) return;

        targetModuleColliders.Remove(other);
        RefreshTargetsByOverlap();
        MarkTargetsDirty();
    }

    public void MarkTargetsDirty()
    {
        targetDirty = true;
    }

    protected void RefreshTargetsByOverlap()
    {
        Debug.Log($"[ColliderReactiveModules] RefreshTargetsByOverlap");
        targetModuleColliders.Clear();

        targetModuleColliders = Physics2D.OverlapBoxAll(transform.TransformPoint(Vector3.up), Vector2.one, transform.eulerAngles.z).ToList(); // 사이즈를 def에서 가져와야 할듯;;
        Debug.Log($"[ColliderReactiveModule] {name} : OverlapBox position : {transform.position + Vector3.up}");
        GetTargetModulesFromColliders();

        targetDirty = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.TransformPoint(Vector3.up), overlapBoxSize);
        Gizmos.DrawLine(transform.position, transform.TransformPoint(Vector3.up));
    }
}