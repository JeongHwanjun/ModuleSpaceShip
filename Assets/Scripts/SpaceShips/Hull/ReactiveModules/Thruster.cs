using UnityEngine;
using ModuleSpaceShip.Runtime;
using System.Linq;

public class Thruster : ColliderReactiveModule
{
    [DefName("ThrusterDef")]
    [SerializeField] private string def;
    protected override string DefName => def;
    protected ThrusterThing thrusterThing => (ThrusterThing)colliderReactiveModuleThing;

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
        // 도크 포트는 무시
        if(other.CompareTag("DockPort")) return;
        // ship 소속이 아니라면 작동할 일이 없으므로 무시
        if(!ship) return;
        targetModules.Append(other);
        Debug.Log($"[ColliderReactiveModule] TriggerEnter : {other.name}");
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        // 도크 포트는 무시
        if(other.CompareTag("DockPort")) return;
        // ship 소속이 아니라면 작동할 일이 없으므로 무시
        if(!ship) return;
        targetModules = targetModules.Where(module => module != other).ToArray();
        Debug.Log($"[ColliderReactiveModule] TriggerExit : {other.name}");
    }

    public void RequstIgnition()
    {
        // ship(또는 추진 담당 객체)에 AddForce 요청
    }
}