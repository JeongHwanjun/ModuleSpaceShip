using UnityEngine;
using ModuleSpaceShip.Defs;
using ModuleSpaceShip.Runtime;
using System;

namespace ModuleSpaceShip.Runtime
{
    public class HullThing : ModuleThing
    {
        private HullDef _hullDef => (HullDef)_def;
        public HullDef hullDef => _hullDef;

        protected override void OnInitTyped()
        {
            base.OnInitTyped();
        }
    }
}