using System;
using System.Xml.Linq;
using ModuleSpaceShip.Runtime;

namespace ModuleSpaceShip.Defs
{
    public class ThrusterDef : ColliderReactiveModuleDef
    {
        public override Type thingType => typeof(ThrusterThing);

        public float thrust = 0f;
        public float damage = 0f;

        public override void LoadFromXml(XElement e)
        {
            base.LoadFromXml(e);
            XElement thruster = e.Element("thruster");
            LoadThrusterData(thruster);
        }

        private void LoadThrusterData(XElement thruster)
        {
            string thrustString = GetTag(thruster, "thrust", "0");
            if(!float.TryParse(thrustString, out thrust)) throw new Exception($"[ThrusterDef] Invalid value for <thrust> : {thrustString}");
            string damageString = GetTag(thruster, "damage", "0");
            if(!float.TryParse(damageString, out damage)) throw new Exception($"[ThrusterDef] Invalid value for <damage> : {damageString}");
            // 추후에 소비 전력, 연산 요구치 등도 추가할 예정
        }

        public override XElement SerializeDef()
        {
            XElement e = base.SerializeDef();

            AddModuleData(e);

            return e;
        }

        protected override XElement AddReactionData()
        {
            XElement reaction = base.AddReactionData();
            reaction.Add(
                AddThrusterData()
            );
            return reaction;
        }

        protected XElement AddThrusterData()
        {
            XElement thruster = new("thruster");
            thruster.Add(
                new XElement("thrust", thrust),
                new XElement("damage", damage)
            );
            return thruster;
        }
    }
}