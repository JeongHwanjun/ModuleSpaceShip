using System;
using ModuleSpaceShip.Defs;

namespace ModuleSpaceShip.Runtime
{
    [Serializable]
    public abstract class ThingBase
    {
        public Def Def { get; private set; }

        public string Label => Def?.label ?? "(NoDef)";
        public string DefName => Def?.defName ?? "(NoDef)";

        public void Init(Def def)
        {
            Def = def;
            OnInit();
        }

        protected virtual void OnInit() { }
    }
    
    [Serializable]
    public abstract class ThingBase<TDef> : ThingBase where TDef : Def
    {
        protected TDef TypedDef { get; private set; }

        protected override void OnInit()
        {
            TypedDef = (TDef)Def;
            OnInitTyped();
        }

        protected virtual void OnInitTyped() { }
    }
}