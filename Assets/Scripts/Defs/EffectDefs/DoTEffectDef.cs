using System;
using System.Xml.Linq;
using ModuleSpaceShip.Runtime;

namespace ModuleSpaceShip.Defs
{
    // 초당 지정된 만큼의 체력을 회복하는 Effect
    public class DoTEffectDef : EffectDefBase
    {
        public override Type thingType => typeof(HealingEffectThing);

        public float damagePerSec = 0f;
        public float durationTime = 0f; // 지속시간, 0 미만 = 영구지속

        protected override void LoadEffectData(XElement effect)
        {
            string damagePerSecString = GetTag(effect, "damagePerSec", "0");
            if(!float.TryParse(damagePerSecString, out damagePerSec))
                throw new Exception($"[DoTEffectDef] Invalid value for damagePerSec : {damagePerSecString}");

            string durationTimeString = GetTag(effect, "durationTime", "0");
            if(!float.TryParse(durationTimeString, out durationTime))
                throw new Exception($"[DoTEffectDef] Invalid value for durationTime : {durationTimeString}");
        }
    }
}