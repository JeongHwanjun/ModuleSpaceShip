using System;
using ModuleSpaceShip.Defs;
using ModuleSpaceShip.Utilities;

namespace ModuleSpaceShip.Runtime
{
    public abstract class ReactiveModuleThing : ModuleThing
    {
        private ReactiveModuleDef _reactiveModuleDef => (ReactiveModuleDef)_def;
        public ReactiveModuleDef reactiveModuleDef => _reactiveModuleDef;
        public bool usePhysicalRadius = false;
        public bool useGridRadius = false;
        protected override void OnInitTyped()
        {
            base.OnInitTyped();

            // 물리 범위를 사용하는지, 그리드 범위를 사용하는지 확인
            if(_reactiveModuleDef.physicalRadius > 0) usePhysicalRadius = true;
            else useGridRadius = true;
        }

        public float GetPhysicalRadius()
        {
            if(_reactiveModuleDef == null) throw new Exception($"[ReactiveModuleThing] Def is not assigned.");
            if(!usePhysicalRadius) return 0;
            return reactiveModuleDef.physicalRadius;
        }

        public GridPos[] GetGridRadius()
        {
            if(_reactiveModuleDef == null) throw new Exception($"[ReactiveModuleThing] Def is not assigned.");
            if(!useGridRadius) return null;
            // 각 문자열에 대응하는 값을 반환
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