using System;
using System.Xml.Linq;
using ModuleSpaceShip.Runtime;
using UnityEngine;

namespace ModuleSpaceShip.Defs
{
    [Serializable]
    public class ShipDef : Def
    {
        public override Type thingType => typeof(ShipThing);
        public override void LoadFromXml(XElement e)
        {
            base.LoadFromXml(e);
        }
    }
} 