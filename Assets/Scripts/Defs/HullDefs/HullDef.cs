using System;
using System.Xml.Linq;
using ModuleSpaceShip.Runtime;

namespace ModuleSpaceShip.Defs
{
    [Serializable]
    public class HullDef : ModuleDef
    {
        public override Type thingType => typeof(HullThing);
        public uint Tier = 0;

        public override void LoadFromXml(XElement e)
        {
            base.LoadFromXml(e);
            string TierString = GetTag(e, "tier", "0");
            if(!uint.TryParse(TierString, out Tier)) throw new Exception($"[HullDef] Invalid value for tier : {TierString}");
        }
    }
} 