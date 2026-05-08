using System;
using System.Xml.Linq;
using ModuleSpaceShip.Runtime;

namespace ModuleSpaceShip.Defs
{
    [Serializable]
    public abstract class TurretDefBase : ModuleDef
    {
        public uint Tier = 0;
        public float rotationSpeed = 1.0f;
        public float angleThreshold = 3.0f;

        public override void LoadFromXml(XElement e)
        {
            base.LoadFromXml(e);
            string TierString = GetTag(e, "tier", "0");
            if(!uint.TryParse(TierString, out Tier)) throw new Exception($"[TurretDefBase] Invalid value for Tier : {TierString}");
            
            XElement Turret = e.Element("turret");
            if(Turret == null)
                throw new Exception($"[TurretDefBase] Turret must have <turret> tag, but '{defName}' has no <turret>.");
            
            // 밑의 얘들도 다 LoadTurretData로 옮겨야 하는 것이 아닌지?
            string rotationSpeedString = GetTag(Turret,"rotationSpeed","1.0");
            if(!float.TryParse(rotationSpeedString, out rotationSpeed))
                throw new Exception($"[TurretDefBase] Invalid value for rotationSpeed : {rotationSpeedString}");

            string angleThresholdString = GetTag(Turret,"angleThreshold","3.0");
            if(!float.TryParse(angleThresholdString, out angleThreshold))
                throw new Exception($"[TurretDefBase] Invalid value for angleThreshold : {angleThreshold}");
            
            LoadTurretData(Turret);
        }

        protected abstract void LoadTurretData(XElement TurretXml);
    }
} 