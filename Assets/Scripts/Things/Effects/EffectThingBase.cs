using ModuleSpaceShip.Defs;

namespace ModuleSpaceShip.Runtime
{
    public abstract class EffectThingBase : ThingBase<EffectDefBase>
    {
        protected EffectDefBase _def;
        public EffectDefBase def => _def;

        protected override void OnInitTyped()
        {
            base.OnInitTyped();
            _def = TypedDef;
        }
    }
}