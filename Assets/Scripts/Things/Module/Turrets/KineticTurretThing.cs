using ModuleSpaceShip.Runtime;
using ModuleSpaceShip.Defs;

namespace ModuleSpaceShip.Runtime
{
    public class KineticTurretThing : TurretThingBase
    {
        private KineticTurretDef _kineticTurretDef => (KineticTurretDef)_def;
        public KineticTurretDef kineticTurretDef => _kineticTurretDef;

        public float coolTime = 1.0f;

        protected override void OnInitTyped()
        {
            base.OnInitTyped();

            coolTime = _kineticTurretDef.coolTime;
        }
    }
}
