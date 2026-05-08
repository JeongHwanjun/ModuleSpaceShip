using System;
using ModuleSpaceShip.Defs;

namespace ModuleSpaceShip.Runtime
{
    public abstract class RadiusReactiveModuleThing : ReactiveModuleThing
    {
        private RadiusReactiveModuleDef _radiusReactiveModuleDef => (RadiusReactiveModuleDef)_def;
        public RadiusReactiveModuleDef radiusReactiveModuleDef => _radiusReactiveModuleDef;
        public float GetRadius()
        {
            if(_radiusReactiveModuleDef == null) throw new Exception($"[ReactiveModuleThing] Def is not assigned.");
            return radiusReactiveModuleDef.radius;
        }
    }
}