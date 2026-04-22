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

        public float GetPhysicalRadius()
        {
            if(_reactiveModuleDef == null) throw new Exception($"[ReactiveModuleThing] Def is not assigned.");
            return reactiveModuleDef.physicalRadius;
        }

        public GridPos[] GetGridRadius()
        {
            if(_reactiveModuleDef == null) throw new Exception($"[ReactiveModuleThing] Def is not assigned.");
            // 각 문자열에 대응하는 값을 반환 - 기본값은 Card4(상하좌우)
            switch (reactiveModuleDef.gridRadiusName)
            {
                case "Card4":
                    return GridRangeUtilities.Card4;
                case "Card8":
                    return GridRangeUtilities.Card8;
                case "Card24":
                    return GridRangeUtilities.Card24;
                default:
                    return GridRangeUtilities.Card4;
            }
        }
    }
}