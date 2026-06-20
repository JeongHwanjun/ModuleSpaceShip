using System;
using ModuleSpaceShip.Defs;
using ModuleSpaceShip.Runtime;
using UnityEngine;

public class EnergyTurret : TurretBase
{
    [DefName("EnergyTurretDef")]
    [SerializeField] private string def;
    protected override string DefName => def;

    [DefName("RayDef")]
    [SerializeField] private string rayDefName;
    private RayDef rayDef;

    private EnergyTurretThing energyTurretThing => (EnergyTurretThing)moduleThing;
    [SerializeField] LineRenderer line;
    [Header("RayLayers")]
    [SerializeField] private LayerMask hitMask;

    private float heat = 0, maxHeat = 0;
    private float coolPerSec = 0, overHeatCoolCoefficient;
    private bool isOverHeat = false;

    protected override void Awake()
    {
        base.Awake();
        rayDefName = energyTurretThing.energyTurretDef.rayDefName;
        coolPerSec = energyTurretThing.coolPerSec;
        maxHeat = energyTurretThing.maxHeat;
        overHeatCoolCoefficient = energyTurretThing.overHeatCoolCoefficient;

        rayDef = DefDatabase.Get<RayDef>(rayDefName);
        line.enabled = false;
        energyTurretThing.SetReadyToFire(true);
    }

    protected override void Update()
    {
        base.Update();
        float coolAmount = coolPerSec * (isOverHeat ? overHeatCoolCoefficient : 1) * Time.deltaTime;
        float nextHeat = heat - coolAmount;
        heat = Mathf.Max(0, nextHeat);
        Debug.Log($"[EnergyTurret] Current Heat : {heat}");
        if(nextHeat <= 0) OverHeatFinished();
    }

    protected override void TryFire()
    {
        if (!energyTurretThing.ReadyToFire || !isRotationComplete)
        {
            CeaseFire();
            return;
        }

        // 레이저 판정 시작
        FireRay();
        // 초당 열 추가
        heat += rayDef.heatPerSec * Time.deltaTime;
        // 열 초과시 과열상태 시작
        if(heat >= maxHeat) OverHeatStarted();
        heat = Mathf.Min(heat, maxHeat);
    }

    protected override void CeaseFire()
    {
        line.enabled = false;
    }

    void FireRay()
    {
        Vector3 dir =new Vector3(inputManager.mousePos.x, inputManager.mousePos.y, 0) - Gun.transform.position;
        dir = dir.normalized;
        Vector3 endPoint = Gun.transform.position + dir * rayDef.maxDistance;

        RaycastHit2D[] hits = Physics2D.RaycastAll(Gun.transform.position, dir, rayDef.maxDistance, hitMask);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach(RaycastHit2D hit in hits)
        {
            if(hit.collider == null) continue;
            if(hit.collider.transform.IsChildOf(transform)) continue;

            Module target = hit.collider.GetComponentInParent<Module>();

            if(target != null)
            {
                if(target.GetShip() == ship) continue; // 자신과 같은 함선 소속이면 무시

                // 적합한 타겟이라면 데미지 전달 및 정지
                target.DeliverDamage(rayDef.damage * Time.deltaTime);
                endPoint = hit.point;
                break;
            }

            // 기타등등에 부딪히면 정지함
            endPoint = hit.point;
            break;
        }
        // 시각적 옂출
        line.enabled = true;
        line.positionCount = 2;
        line.SetPosition(0, Gun.transform.position);
        line.SetPosition(1, endPoint);
    }

    private void OverHeatStarted()
    {
        Debug.Log($"[EnergyTurret] OverHeat, current heat : {heat}");
        energyTurretThing.SetReadyToFire(false);
        isOverHeat = true;
    }
    private void OverHeatFinished()
    {
        Debug.Log($"[EnergyTurret] OverHeat Finished");
        energyTurretThing.SetReadyToFire(true);
        isOverHeat = false;
    }
}