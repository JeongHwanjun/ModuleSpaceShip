using System;
using System.Xml.Linq;
using ModuleSpaceShip.Runtime;

namespace ModuleSpaceShip.Defs
{
    [Serializable]
    public class HullDef : ModuleDef
    {
        public override Type thingType => typeof(HullThing);
        public uint tier = 0;

        public override void LoadFromXml(XElement e)
        {
            base.LoadFromXml(e);
            string TierString = GetTag(e, "tier", "0");
            if(!uint.TryParse(TierString, out tier)) throw new Exception($"[HullDef] Invalid value for tier : {TierString}");
        }

        public override XElement SerializeDef()
        {
            XElement e = base.SerializeDef();

            AddModuleData(e);

            return e;
        }

        protected override void AddModuleData(XElement e)
        {
            base.AddModuleData(e);
            e.Add(
                new XElement("tier", tier)
            );
        }
    }
} 