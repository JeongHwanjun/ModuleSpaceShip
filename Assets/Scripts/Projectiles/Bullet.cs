using ModuleSpaceShip.Runtime;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    /* Def 및 컴포넌트 */
    [DefName("BulletDef")]
    [SerializeField] private string def;
    protected string DefName => def;

    protected BulletThing bulletThing;
    private Ship shooterShip = null;
    private bool isDefDesignated = false;

    public void DesignateDef(string givenBulletDefName)
    {
        // 포탑에서 생성시 def를 지정함
        def = givenBulletDefName;
        isDefDesignated = true;
    }

    public void InformShip(Ship shooter)
    {
        // 생성시 발사자를 지정함
        shooterShip = shooter;
        Collider2D col = GetComponent<Collider2D>();

        // 발사자와의 충돌을 무시함
        foreach(Collider2D module in shooterShip.moduleColliders)
        {
            Physics2D.IgnoreCollision(col, module);
        }

        Debug.Log($"[Bullet] Now Ignore collision with : {shooterShip}");
    }

    void Awake()
    {
        if (!isDefDesignated) {Debug.LogWarning($"[Bullet] No Def received, using fallback");}
        bulletThing = (BulletThing)ThingFactory.CreateFromDefName(def);
    }

    void Update()
    {
        // 항상 전진함
        transform.localPosition += transform.up * bulletThing.def.speed * Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[Bullet] Collision With : {collision.collider}");
        Module other = collision.collider.GetComponent<Module>();
        if(other == null) return;
        other.DeliverDamage(bulletThing.damage);

        Debug.Log($"[Bullet] Delivered damage to {other} : {bulletThing.damage}");

        // 데미지 준 후 즉시 파괴
        Destroy(gameObject);
    }
}
