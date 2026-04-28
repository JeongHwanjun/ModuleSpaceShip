using System;
using ModuleSpaceShip.Defs;
using ModuleSpaceShip.Utilities;

namespace ModuleSpaceShip.Runtime
{
    public class ExplosiveModuleThing : ReactiveModuleThing
    {
        private ExplosiveModuleDef _explosiveModuleDef => (ExplosiveModuleDef)_def;
        public ExplosiveModuleDef explosiveModuleDef => _explosiveModuleDef;
        public float damage = 0f;
        protected override void OnInitTyped()
        {
            base.OnInitTyped();

            damage = _explosiveModuleDef.damage;
        }
    }
}