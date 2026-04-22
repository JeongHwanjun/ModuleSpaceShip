using ModuleSpaceShip.Defs;

namespace ModuleSpaceShip.Runtime
{
    public class HealingEffectThing : EffectThingBase
    {
        protected HealingEffectDef _healingEffectDef => (HealingEffectDef)_def;
        public HealingEffectDef healingEffectDef => _healingEffectDef;

        public float healAmount = 0;

        protected override void OnInitTyped()
        {
            base.OnInitTyped();
            healAmount = _healingEffectDef.healAmount;
        }
    }
}