using UnityEngine;

// 코어 담당 스크립트. 탈부착 대부분의 기능이 동작하지 않도록 막음.
public class Core : Module
{
    [DefName("HullDef")]
    [SerializeField] private string def;
    protected override string DefName => def;

    public override void OnClickStart()
    {
        // do nothing...
    }

    public override void OnClickCancel(Collider2D hit)
    {
        // do nothing...
    }

    // ---- 부착시 ShipGrid에서 실행되어 초기화 등 담당 ----
    public override void OnAttached(Transform parent, Vector3 position)
    {
        base.OnAttached(parent, position);
    }

    // ---- 탈착시 ShipGrid에서 실행되어 초기화 등 담당 ----
    public override void OnDetached(bool isChained)
    {
        // do nothing...
    }
}
