using System;
using System.Xml.Linq;
using ModuleSpaceShip.Runtime;

namespace ModuleSpaceShip.Defs
{
    public class BulletDef : Def
    {
        public override Type thingType => typeof(BulletThing);

        public float damage = 1.0f;
        public float speed = 5.0f;

        public override void LoadFromXml(XElement e)
        {
            base.LoadFromXml(e);
            string damageString = GetTag(e,"damage","1.0");
            if(!float.TryParse(damageString, out damage))
                throw new Exception($"[BulletDef] Invalid value for damage : {damageString}");

            string speedString = GetTag(e,"speed","5.0");
            if(!float.TryParse(speedString, out speed))
                throw new Exception($"[BulletDef] Invalid value for speed : {speedString}");
        }
    }
}