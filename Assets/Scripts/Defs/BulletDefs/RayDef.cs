using System;
using System.Xml.Linq;
using ModuleSpaceShip.Runtime;

namespace ModuleSpaceShip.Defs
{
    public class RayDef : Def
    {
        public override Type thingType => typeof(RayThing);
        public float damage = 10.0f;
        public float heatPerSec = 1.0f;
        public float maxDistance = 10.0f;

        public override void LoadFromXml(XElement e)
        {
            base.LoadFromXml(e);

            string damageString = GetTag(e, "damage", "10.0");
            if(!float.TryParse(damageString, out damage))
                throw new Exception($"[RayDef] Invalid value for <damage> : {damageString}");
            string heatPerSecString = GetTag(e, "heatPerSec", "1.0");
            if(!float.TryParse(heatPerSecString, out heatPerSec))
                throw new Exception($"[RayDef] Invalid value for <heatPerSec> : {heatPerSecString}");
            string maxDistanceString = GetTag(e, "maxDistance", "1.0");
            if(!float.TryParse(maxDistanceString, out maxDistance))
                throw new Exception($"[RayDef] Invalid value for <maxDistance> : {maxDistanceString}");
        }

        public override XElement SerializeDef()
        {
            XElement ray = new("bullet");
            ray.Add(
                new XElement("damage", damage),
                new XElement("heatPerSec", heatPerSec),
                new XElement("maxDistance", maxDistance)
            );
            return ray;
        }
    }
}