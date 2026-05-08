using UnityEngine;
using System;
using System.Xml.Linq;

namespace ModuleSpaceShip.Defs
{
    // ---- 인접 모듈 또는 일정 범위 내의 타 모듈과 상호작용하는 모듈Def의 기본형 ----
    [Serializable]
    public abstract class ReactiveModuleDef : ModuleDef
    {
        public override void LoadFromXml(XElement e)
        {
            base.LoadFromXml(e);
            XElement reaction = e.Element("reaction");
            if(reaction == null) throw new Exception($"[ReactiveModuleDef] ReactiveModuleDef requires <reaction> tag, but {defName} has no <reaction>.");
            LoadReactionData(reaction);
        }

        protected abstract void LoadReactionData(XElement e);
    }
}
