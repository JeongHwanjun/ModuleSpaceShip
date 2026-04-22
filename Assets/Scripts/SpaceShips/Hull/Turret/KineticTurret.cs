using UnityEngine;
using ModuleSpaceShip.Runtime;

public class KineticTurret : TurretBase
{
    [DefName("KineticTurretDef")]
    [SerializeField] private string def;
    protected override string DefName => def;
    [DefName("BulletDef")]
    [SerializeField] private string bulletDef;
    private float coolTime = 0f;

    private KineticTurretThing kineticTurretThing => (KineticTurretThing)moduleThing;
    [SerializeField] private GameObject BulletPrefab;

    protected override void Awake()
    {
        base.Awake();
        bulletDef = kineticTurretThing.kineticTurretDef.bulletDefName;
    }

    protected override void Update()
    {
        base.Update();
        // 쿨타임 계산
        if(coolTime <= 0f)
        {
            // 사격 가능한 상태가 됨
            kineticTurretThing.SetReadyToFire(true);
        }
        else coolTime -= Time.deltaTime;
    }


    protected override void TryFire()
    {
        // KineticTurret 단의 조건은 이곳에서 검사
        if(!kineticTurretThing.ReadyToFire) return;
        if(!isRotationComplete) return;

        Debug.Log($"Fire!!! Rotation : {Gun.transform.rotation}");
        kineticTurretThing.SetReadyToFire(false);
        coolTime = kineticTurretThing.coolTime;

        GameObject bullet = Instantiate(BulletPrefab, Gun.transform.position, Gun.transform.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.DesignateDef(bulletDef);
        bulletScript.InformShip(ship);
    }
}