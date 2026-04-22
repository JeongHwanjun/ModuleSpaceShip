using System;
using System.Xml.Linq;
using ModuleSpaceShip.Runtime;

namespace ModuleSpaceShip.Defs
{
    // 초당 지정된 만큼의 체력을 회복하는 Effect
    public class HealingEffectDef : EffectDefBase
    {
        public override Type thingType => typeof(HealingEffectThing);

        public float healAmount = 0;

        protected override void LoadEffectData(XElement effect)
        {
            string healAmountString = GetTag(effect, "healAmount", "0");
            if(!float.TryParse(healAmountString, out healAmount))
                throw new Exception($"[HealingEffectDef] Invalid value for healAmount : {healAmountString}");
        }
    }
}