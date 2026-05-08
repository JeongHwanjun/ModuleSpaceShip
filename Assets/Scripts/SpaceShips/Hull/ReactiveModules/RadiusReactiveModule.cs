using System.Collections.Generic;
using ModuleSpaceShip.Runtime;
using Unity.VisualScripting;
using UnityEngine;

// ---- 주어진 범위 내의 Module을 획득하여 해당 모듈에 특정한 처리를 하기 위한 기반 클래스 ----
public abstract class RadiusReactiveModule : ReactiveModule
{
    protected RadiusReactiveModuleThing radiusReactiveModuleThing => (RadiusReactiveModuleThing)moduleThing;
    [SerializeField] private GameObject radiusIndicator;

    protected override void Awake()
    {
        base.Awake();
        radiusIndicator.transform.localScale *= radiusReactiveModuleThing.GetRadius() * 2f;
    }

    // 지정된 원형 거리 내의 모듈 가져오기 - Grid와 상호배타적
    protected void GetModulesInRadius()
    {
        // 이는 ship 소속이 아니라도 관계 없음
        float radius = radiusReactiveModuleThing.GetRadius();
        // 물리적 거리를 활용해 탐색
        targetModules = Physics2D.OverlapCircleAll(transform.position, radius);
    }

    [ContextMenu("Toggle Radius Visibility")]
    void ToggleRadiusVisibility()
    {
        radiusIndicator.SetActive(!radiusIndicator.activeSelf);
    }
}