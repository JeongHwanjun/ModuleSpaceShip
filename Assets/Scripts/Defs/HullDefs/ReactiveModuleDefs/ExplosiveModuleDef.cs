using System;
using System.Xml.Linq;
using ModuleSpaceShip.Runtime;

namespace ModuleSpaceShip.Defs
{
    public class ExplosiveModuleDef : RadiusReactiveModuleDef
    {
        public override Type thingType => typeof(ExplosiveModuleThing);
        public float damage = 0f;

        public override void LoadFromXml(XElement e)
        {
            base.LoadFromXml(e);
            XElement explosion = e.Element("explosion");
            LoadExplosion(explosion);
        }

        private void LoadExplosion(XElement explosion)
        {
            string damageString = GetTag(explosion, "damage", "0");
            if(!float.TryParse(damageString, out damage))
                throw new Exception($"[ExplosiveModuleDef] Invalid value for damage : {damageString}");
        }
    }
}