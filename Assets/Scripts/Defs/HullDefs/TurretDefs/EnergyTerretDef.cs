using System;
using System.Diagnostics;
using System.Xml.Linq;
using ModuleSpaceShip.Runtime;

namespace ModuleSpaceShip.Defs
{
    public class EnergyTurretDef : TurretDefBase
    {
        public override Type thingType => typeof(EnergyTurretThing);
        public float maxHeat = 10.0f;
        public float coolPerSec = 1.0f;
        public float overHeatCoolCoefficient = 0.5f;
        public string rayDefName = null;

        protected override void LoadTurretData(XElement TurretXml)
        {
            string maxHeatString = GetTag(TurretXml, "maxHeat", "1.0");
            if(!float.TryParse(maxHeatString, out maxHeat))
                throw new Exception($"[EnergyTurretDef] Invalid value for <maxHeat> : {maxHeatString}");
            string coolPerSecString = GetTag(TurretXml, "coolPerSec", "1.0");
            if(!float.TryParse(coolPerSecString, out coolPerSec))
                throw new Exception($"[EnergyTurretDef] Invalid value for <coolPerSec> : {coolPerSecString}");
            string overHeatCoolCoefficientString = GetTag(TurretXml, "overHeatCoolCoefficient", "0.5");
            if(!float.TryParse(overHeatCoolCoefficientString, out overHeatCoolCoefficient))
                throw new Exception($"[EnergyTurretDef] Invalid Value for <overHeatCoolCoefficient> : {overHeatCoolCoefficientString}");
            string rayDefNameString = GetTag(TurretXml, "rayDefName", "ray");
            if(string.IsNullOrWhiteSpace(rayDefNameString))
                throw new Exception($"[EnergyTurretDef] Invalid value for <rayDefName> : {rayDefNameString}");
            else rayDefName = rayDefNameString.Trim();
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
                AddTurretData()
            );
        }

        protected override XElement AddTurretData()
        {
            XElement turret = new("turret");
            turret.Add(
                new XElement("maxHeat",maxHeat),
                new XElement("coolPerSec",coolPerSec),
                new XElement("overHeatCoolCoefficient",overHeatCoolCoefficient),
                new XElement("rayDefName",rayDefName)
            );
            return turret;
        }
    }
}