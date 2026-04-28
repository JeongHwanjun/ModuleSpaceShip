using UnityEngine;
using System;
using System.Xml.Linq;

namespace ModuleSpaceShip.Defs
{
    // ---- 인접 모듈 또는 일정 범위 내의 타 모듈과 상호작용하는 모듈Def의 기본형 ----
    [Serializable]
    public abstract class ReactiveModuleDef : ModuleDef
    {
        public float physicalRadius = 0; // 물리 상호작용 범위
        public string gridRadiusName; // 효과 범위 이름 지정. 4칸 - card4 8칸 - card8 이런식

        public override void LoadFromXml(XElement e)
        {
            base.LoadFromXml(e);
            LoadReactionData(e);
        }

        protected void LoadReactionData(XElement e)
        {
            string physicalRadiusString = GetTag(e, "physicalRadius", "0");
            if(!float.TryParse(physicalRadiusString, out physicalRadius)) throw new Exception($"[ReactiveModuleDef] Invalid value for physicalRadius : {physicalRadius}");
            
            string gridRadiusNameString = GetTag(e, "gridRadiusName", "none");
            if(string.IsNullOrWhiteSpace(gridRadiusNameString)) throw new Exception($"[ReactiveModuleDef] Invalid value for gridRadiusName : {gridRadiusNameString}");
            else gridRadiusName = gridRadiusNameString.Trim();
        }
    }
}
