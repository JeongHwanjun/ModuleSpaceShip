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

    // 지정된 원형 거리 내의 모듈 가져오기 - Grid와 상호배타적
    protected void GetModulesInRadius()
    {
        // 이는 ship 소속이 아니라도 관계 없음
        float radius = reactiveModuleThing.GetPhysicalRadius();
        // 물리적 거리를 활용해 탐색
        targetModules = Physics2D.OverlapCircleAll(transform.position, radius);
    }

    // 지정된 그리드에 해당하는 모듈 가져오기 - Radius와 상호배타적
    protected void GetModulesInGrid()
    {
        // ship 소식인지 확인
        if(ship == null) return; // ship 소속이 아니라면 스킵

        // ship 소속이라면 주어진 그리드대로 방문하며 Collider2D 조회
        GridPos[] grids = reactiveModuleThing.GetGridRadius();
        if(grids == null) return;
        targetModules = ship.GetModulesByGrid(grids);
    }
}