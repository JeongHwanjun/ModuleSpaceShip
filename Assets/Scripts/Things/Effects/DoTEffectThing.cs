using ModuleSpaceShip.Defs;

namespace ModuleSpaceShip.Runtime
{
    public class DoTEffectThing : EffectThingBase
    {
        protected DoTEffectDef _DotEffectDef => (DoTEffectDef)_def;
        public DoTEffectDef _dotEffectDef => _DotEffectDef;

        public float damagePerSec = 0;
        public float durationTime = 0;

        protected override void OnInitTyped()
        {
            base.OnInitTyped();
            damagePerSec = _DotEffectDef.damagePerSec;
            durationTime = _DotEffectDef.durationTime;
        }
    }
}