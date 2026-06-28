using UnityEngine;
using System;
using System.Xml.Linq;

namespace ModuleSpaceShip.Defs
{
    [Serializable]
    public abstract class ModuleDef : Def
    {
        public string prefabPath;
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
            string prefabPathString = GetTag(e, "prefabPath", "");
            if (string.IsNullOrWhiteSpace(prefabPathString))
                throw new Exception($"[HullBaseDef] Invalid value for HullPoint : {prefabPathString}");
            else prefabPath = prefabPathString.Trim();
            string hpString = GetTag(e, "hullPoint", "10.0");
            if(!float.TryParse(hpString, out hullPoint)) throw new Exception($"[HullBaseDef] Invalid value for HullPoint : {hpString}");
            string massString = GetTag(e, "mass", "1.0");
            if(!float.TryParse(massString, out mass)) throw new Exception($"[HullBaseDef] Invalid value for Mass : {massString}");
            string linearDampingString = GetTag(e, "linearDamping", "0");
            if(!float.TryParse(linearDampingString, out linearDamping)) throw new Exception($"[HullBaseDef] Invalid value for linearDamping : {linearDampingString}");
            string angularDampingString = GetTag(e, "angularDamping", "0");
            if(!float.TryParse(angularDampingString, out angularDamping)) throw new Exception($"[HullBaseDef] Invalid value for angularDamping : {angularDampingString}");
        }

        public override XElement SerializeDef()
        {
            XElement e = new("Def");

            AddModuleData(e);

            return e;
        }

        protected virtual void AddModuleData(XElement e)
        {
            e.Add(
                new XElement("prefabPath", prefabPath),
                new XElement("hullPoint", hullPoint.ToString()),
                new XElement("mass", mass.ToString()),
                new XElement("linearDamping", linearDamping.ToString()),
                new XElement("angularDamping", angularDamping.ToString()),
                new XElement("gravityScale", gravityScale.ToString())
            );
        }
    }
}
