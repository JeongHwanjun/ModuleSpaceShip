using ModuleSpaceShip.Defs;

namespace ModuleSpaceShip.Runtime
{
    public abstract class TurretThingBase : ModuleThing
    {
        private TurretDefBase _turretDefBase => (TurretDefBase)_def;
        public TurretDefBase TurretDefBase => _turretDefBase;
        public bool ReadyToFire = false;
        public float AngleThreshold = 3.0f;

        protected override void OnInitTyped()
        {
            base.OnInitTyped();
            AngleThreshold = _turretDefBase.angleThreshold;
        }

        public void SetReadyToFire(bool isReady)
        {
            ReadyToFire = isReady;
        }
    }
}