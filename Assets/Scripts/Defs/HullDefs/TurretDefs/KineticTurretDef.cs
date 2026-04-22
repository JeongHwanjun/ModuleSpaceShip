using System;
using System.Xml.Linq;
using ModuleSpaceShip.Runtime;

namespace ModuleSpaceShip.Defs
{
    public class KineticTurretDef : TurretDefBase
    {
        public override Type thingType => typeof(KineticTurretThing);

        public float coolTime = 1.0f;
        public string bulletDefName = null;

        protected override void LoadTurretData(XElement TurretXml)
        {
            // 기타 포탑 데이터를 읽어옴...
            // 아마 질량병기에만 있는 애들을 읽어와야 할듯. 공통적인 부분은 이미 부모에서 다 읽어옴.

            string coolTimeString = GetTag(TurretXml, "coolTime", "1.0");
            if(!float.TryParse(coolTimeString, out coolTime))
                throw new Exception($"[KineticTurretDef] Invalid value for coolTime : {coolTimeString}");
            string bulletDefNameString = GetTag(TurretXml, "bulletDefName", "Bullet");
            if(string.IsNullOrWhiteSpace(bulletDefNameString))
                throw new Exception($"[KineticTurretDef] Invalid value for bulletDefName : {bulletDefNameString}");
            else bulletDefName = bulletDefNameString.Trim();
        }
    }
}