using ModuleSpaceShip.Defs;

namespace ModuleSpaceShip.Runtime
{
    public class BulletThing : ThingBase<BulletDef>
    {
        protected BulletDef _def;
        public BulletDef def => _def;

        public float damage;

        protected override void OnInitTyped()
        {
            base.OnInitTyped();
            
            _def = TypedDef;

            damage = _def.damage;
        }
    }
}