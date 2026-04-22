using UnityEngine;
using System;
using System.Xml.Linq;

/*
    abstract class는 def가 직접 자신의 클래스로 선언할 수 없음.
*/

namespace ModuleSpaceShip.Defs
{
    [Serializable]
    public abstract class Def
    {
        public abstract Type thingType {get;}
        public string defName;
        public string label;
        public string displayName;

        public override string ToString() => $"{GetType().Name}({defName})";

        // 제공된 xml로부터 tag 로드
        public virtual void LoadFromXml(XElement e)
        {
            // 공통 필드
            defName = GetTag(e, "defName");
            label   = GetTag(e, "label", defaultValue: defName);
            displayName = GetTag(e, "displayName", defaultValue : label);
        }

        // xml에서 이름 기반으로 속성 로드
        protected static string GetTag(XElement elem, string tag, string defaultValue = "")
        {
            var child = elem.Element(tag);
            if (child == null) return defaultValue;
            return (child.Value ?? "").Trim();
        }
    }
}