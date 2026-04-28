using ModuleSpaceShip.Runtime;
using UnityEngine;
using System;

public class ExplosiveModule : ReactiveModule
{
    [DefName("ExplosiveModuleDef")]
    [SerializeField] private string def;
    protected override string DefName => def;
    protected ExplosiveModuleThing explosiveModuleThing => (ExplosiveModuleThing)reactiveModuleThing;
    private GridPos[] gridRadius;
    private float physicalRadius;
    private float damage = 0;

    public override void Init(ThingBase thingBase)
    {
        base.Init(thingBase);
        // 범위 획득
        if(explosiveModuleThing.useGridRadius) gridRadius = explosiveModuleThing.GetGridRadius();
        else if(explosiveModuleThing.usePhysicalRadius) physicalRadius = explosiveModuleThing.GetPhysicalRadius();
        else throw new Exception("[ExplosiveModule] Undefined Radius");
        // 데미지 획득
        damage = explosiveModuleThing.damage;
    }

    public override void OnAttached(Transform parent, Vector3 position)
    {
        // 이 모듈이 부착되었을 때 발생
        base.OnAttached(parent, position); // Module 단에서 부착 실행
    }

    public override void OnDetached(bool isChained)
    {
        // 이 모듈이 탈착되었을 때 발생
        base.OnDetached(isChained); // Module 단에서 탈착 실행
    }

    public override void OnModuleAttached()
    {
        // 다른 모듈이 같은 ship에 부착되었을 때 발생
    }

    public override void OnModuleDetached()
    {
        // 다른 무듈에 같은 ship에서 탈착되었을 때 발생
    }

    protected override void ModuleDestroyed() // 이 모듈의 파괴가 요청될 경우
    {
        base.ModuleDestroyed();
        Collider2D[] targetModules = explosiveModuleThing.useGridRadius ? ship.GetModulesByGrid(gridRadius) : Physics2D.OverlapCircleAll(transform.position,explosiveModuleThing.GetPhysicalRadius());
        DeliverDamageToTargetModules(targetModules);
    }

    private void DeliverDamageToTargetModules(Collider2D[] targetModules)
    {
        // 주어진 TargetModules에 DeliverDamage
        foreach(Collider2D targetModule in targetModules)
        {
            targetModule.GetComponent<Module>().DeliverDamage(damage);
        }
        // 모션, 효과 출력 등 다양한 작용...
    }
}