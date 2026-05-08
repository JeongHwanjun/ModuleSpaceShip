using System;
using ModuleSpaceShip.Defs;
using ModuleSpaceShip.Utilities;

namespace ModuleSpaceShip.Runtime
{
    public abstract class GridReactiveModuleThing : ReactiveModuleThing
    {
        private GridReactiveModuleDef _gridReactiveModuleDef => (GridReactiveModuleDef)_def;
        public GridReactiveModuleDef gridReactiveModuleDef => _gridReactiveModuleDef;

        public GridPos[] GetGrid()
        {
            if(_gridReactiveModuleDef == null) throw new Exception($"[ReactiveModuleThing] Def is not assigned.");
            // 각 문자열에 대응하는 값을 반환
            switch (gridReactiveModuleDef.grid)
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