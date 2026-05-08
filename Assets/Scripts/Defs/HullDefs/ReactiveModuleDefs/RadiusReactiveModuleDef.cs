using System;
using System.Xml.Linq;

namespace ModuleSpaceShip.Defs
{
    public abstract class RadiusReactiveModuleDef : ReactiveModuleDef
    {
        public float radius = 0;
        protected override void LoadReactionData(XElement e)
        {
            string radiusString = GetTag(e,"radius", "0");
            if(!float.TryParse(radiusString, out radius)) throw new Exception($"[RadiusReactiveModuleDef] Invalid value for radius : {radiusString}");
        }
    }
}