using System;
using ModuleSpaceShip.Defs;
using ModuleSpaceShip.Utilities;

namespace ModuleSpaceShip.Runtime
{
    public abstract class ReactiveModuleThing : ModuleThing
    {
        private ReactiveModuleDef _reactiveModuleDef => (ReactiveModuleDef)_def;
        public ReactiveModuleDef reactiveModuleDef => _reactiveModuleDef;
        protected override void OnInitTyped()
        {
            base.OnInitTyped();
        }
    }
}