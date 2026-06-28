using System;
using System.Xml.Linq;

namespace ModuleSpaceShip.Defs
{
    // ---- 모든 효과 def의 원형 ----
    public abstract class EffectDefBase : Def
    {

        public override void LoadFromXml(XElement e)
        {
            base.LoadFromXml(e);
            XElement effect = e.Element("effect");
            if(effect == null)
                throw new Exception($"[EffectDefBase] effect must have <effect> tag, but {defName} has no <effect>.");
            LoadEffectData(effect);
        }
        public override XElement SerializeDef()
        {
            XElement e = new("Def");
            e.Add(
                AddEffectData()
            );
            return e;
        }

        protected abstract void LoadEffectData(XElement effect);
        protected abstract XElement AddEffectData();
    }
}