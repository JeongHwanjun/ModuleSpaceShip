using ModuleSpaceShip.Defs;

namespace ModuleSpaceShip.Runtime
{
    public class RayThing : ThingBase<RayDef>
    {
        protected RayDef _def;
        public RayDef def => _def;
        private RayDef _rayDef => (RayDef)_def;
        public RayDef rayDef => _rayDef;

        public float damage = 10.0f;
        public float heatPerSec = 1.0f;

        protected override void OnInitTyped()
        {
            base.OnInitTyped();
            
            _def = TypedDef;

            damage = _rayDef.damage;
            heatPerSec = _rayDef.heatPerSec;
        }
    }
}