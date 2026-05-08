using System.Collections.Generic;
using ModuleSpaceShip.Runtime;
using Unity.VisualScripting;
using UnityEngine;

// ---- 주어진 범위 내의 Module을 획득하여 해당 모듈에 특정한 처리를 하기 위한 기반 클래스 ----
public abstract class ReactiveModule : Module
{
    protected ReactiveModuleThing reactiveModuleThing => (ReactiveModuleThing)moduleThing;
    protected Collider2D[] targetModules;
    public abstract void OnModuleAttached(); // 다른 Module이 도킹됨
    public abstract void OnModuleDetached(); // 다른 Module이 언도킹됨

    protected override void Awake()
    {
        base.Awake();
    }
}