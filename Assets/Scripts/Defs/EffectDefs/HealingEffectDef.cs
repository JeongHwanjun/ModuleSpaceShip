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
        public float durationTime = 0f; // 지속시간, 0 미만 = 영구지속

        protected override void LoadEffectData(XElement effect)
        {
            string healAmountString = GetTag(effect, "healAmount", "0");
            if(!float.TryParse(healAmountString, out healAmount))
                throw new Exception($"[HealingEffectDef] Invalid value for healAmount : {healAmountString}");

            string durationTimeString = GetTag(effect, "durationTime", "0");
            if(!float.TryParse(durationTimeString, out durationTime))
                throw new Exception($"[HealingEffectDef] Invalid value for durationTime : {durationTimeString}");
        }

        protected override XElement AddEffectData()
        {
            XElement effect = new("effect");
            effect.Add(
                new XElement("healAmount", healAmount),
                new XElement("durationTime", durationTime)
            );
            return effect;
        }
    }
}