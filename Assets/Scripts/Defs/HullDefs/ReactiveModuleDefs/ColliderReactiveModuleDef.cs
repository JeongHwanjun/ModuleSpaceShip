using System.Xml.Linq;

namespace ModuleSpaceShip.Defs
{
    public abstract class ColliderReactiveModuleDef : ReactiveModuleDef
    {
        protected override void LoadReactionData(XElement e)
        {
            // 지정된 Collider를 사용하기 때문에 범위를 xml에서 가져오지는 않음
        }
    }
}