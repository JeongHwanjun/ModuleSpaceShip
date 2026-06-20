using ModuleSpaceShip.Defs;

namespace ModuleSpaceShip.Runtime
{
    public class EnergyTurretThing : TurretThingBase
    {
        private EnergyTurretDef _energyTurretDef => (EnergyTurretDef)_def;
        public EnergyTurretDef energyTurretDef => _energyTurretDef;

        public float maxHeat = 10.0f;
        public float coolPerSec = 1.0f;
        public float overHeatCoolCoefficient = 0.5f;

        protected override void OnInitTyped()
        {
            base.OnInitTyped();

            maxHeat = _energyTurretDef.maxHeat;
            coolPerSec = _energyTurretDef.coolPerSec;
            overHeatCoolCoefficient = _energyTurretDef.overHeatCoolCoefficient;
        }
    }
}