using System;
using System.Xml.Linq;

namespace ModuleSpaceShip.Defs
{
    public abstract class GridReactiveModuleDef : ReactiveModuleDef
    {
        public string grid = null;
        protected override void LoadReactionData(XElement e)
        {
            string gridString = GetTag(e, "grid", "card4");
            if(string.IsNullOrWhiteSpace(gridString)) throw new Exception($"[GridReactiveModuleDef] Invalid value for grid : {gridString}");
            else grid = gridString.Trim();
        }
    }
}