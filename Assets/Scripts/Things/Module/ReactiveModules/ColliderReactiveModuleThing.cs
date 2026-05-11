using System;
using ModuleSpaceShip.Defs;

namespace ModuleSpaceShip.Runtime
{
    public abstract class ColliderReactiveModuleThing : ReactiveModuleThing
    {
        private ColliderReactiveModuleDef _colliderReactiveModuleDef => (ColliderReactiveModuleDef)_def;
        public ColliderReactiveModuleDef colliderReactiveModuleDef => _colliderReactiveModuleDef;
    }
}