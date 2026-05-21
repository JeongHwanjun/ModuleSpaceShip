using UnityEngine;
using ModuleSpaceShip.Runtime;
using System.Linq;

public class Thruster : ColliderReactiveModule
{
    [DefName("ThrusterDef")]
    [SerializeField] private string def;
    protected override string DefName => def;
    protected ThrusterThing thrusterThing => (ThrusterThing)colliderReactiveModuleThing;

    public Vector2 localPosition => transform.localPosition;

    public Vector2 localForceDirection
    {
        get
        {
            // Thruster의 up방향의 *반대방향*으로 힘을 가함.
            return transform.localRotation * Vector2.down;
        }
    }

    public override void OnModuleAttached()
    {
        // 추진기는 뭐 없음... collider기반이라
    }

    public override void OnModuleDetached()
    {
        // 추진기는 뭐 없음... collider기반이라
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // ship 소속이 아니라면 작동할 일이 없으므로 무시
        if(!ship) return;
        base.OnTriggerEnter2D(other);

    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        // ship 소속이 아니라면 작동할 일이 없으므로 무시
        if(!ship) return;
        base.OnTriggerExit2D(other);
    }

    public void Ignite(Rigidbody2D shipRigid, float throttle)
    {
        Debug.Log("[Thruster] Ignite!");
        if (!shipRigid) return;

        Vector2 worldPosition = transform.position;
        Vector2 worldForce = -transform.up * thrusterThing.thrust * throttle;

        shipRigid.AddForceAtPosition(worldForce, worldPosition, ForceMode2D.Force);

        Debug.Log($"[Thruster] targetModule Length : {targetModules.Count}");
        // 범위내 모듈에 데미지 발생
        foreach(Module targetModule in targetModules)
        {
            targetModule.DeliverDamage(thrusterThing.damage * Time.deltaTime);
        }
    }
}