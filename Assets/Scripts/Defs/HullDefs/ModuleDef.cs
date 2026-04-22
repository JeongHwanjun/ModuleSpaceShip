using UnityEngine;
using System;
using System.Xml.Linq;

namespace ModuleSpaceShip.Defs
{
    [Serializable]
    public abstract class ModuleDef : Def
    {
        public float hullPoint;
        public float mass;
        public float linearDamping = 0f; // 저항
        public float angularDamping = 0f; // 회전 저항
        public float gravityScale = 0f; // 중력 영향 X

        public override void LoadFromXml(XElement e)
        {
            base.LoadFromXml(e);
            LoadModuleData(e);
        }

        protected void LoadModuleData(XElement e)
        {
            string hpString = GetTag(e, "hullPoint", "10.0");
            if(!float.TryParse(hpString, out hullPoint)) throw new Exception($"[HullBaseDef] Invalid value for HullPoint : {hpString}");
            string massString = GetTag(e, "mass", "1.0");
            if(!float.TryParse(massString, out mass)) throw new Exception($"[HullBaseDef] Invalid value for Mass : {massString}");
            string linearDampingString = GetTag(e, "linearDamping", "0");
            if(!float.TryParse(linearDampingString, out linearDamping)) throw new Exception($"[HullBaseDef] Invalid value for linearDamping : {linearDampingString}");
            string angularDampingString = GetTag(e, "angularDamping", "0");
            if(!float.TryParse(angularDampingString, out angularDamping)) throw new Exception($"[HullBaseDef] Invalid value for angularDamping : {angularDampingString}");
        }
    }
}
